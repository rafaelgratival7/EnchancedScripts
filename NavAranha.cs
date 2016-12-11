using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

[NetworkSettings(channel = 3,sendInterval = 0.01F)]
public class NavAranha : NetworkBehaviour
{
	#region Declaração
	private Animator AnimatorDaAranha;

	private AudioSource SonsDaAranha;

	private Collider[] PercebidosNaPercepção;

	[Tooltip("Layer's dos Inimigos")]
	public LayerMask Inimigos;

	[Tooltip("Layer's dos Obstaculos")]
	public LayerMask Obstaculos;

	[HideInInspector]
	public GameObject Projectil;

	private GameObject Teia;
	private GameObject Ninho;

	[HideInInspector]
	public Transform PontoDeTiro;

	private NavMeshAgent AgenteDaAranha;

	private NavAranhaSound Clip;

	[HideInInspector]
	public NavAranhaDamager Damage;

	private Quaternion Rotação;
	private Quaternion RotaçãoParaAtack;

	[HideInInspector]
	public List<Transform> Percebidos = new List<Transform>();

	private Transform Alvo;

	[HideInInspector]
	public Transform InimigoMaisProximo;

	[HideInInspector]
	public Vector3 NormalNinho;

	private Vector3 DireçãoDoInimigo;
	private Vector3 PosiçãoAtual;
	private Vector3 Normal;
	private Vector3 VelocidadeLocal;
	private Vector3 VelocidadeGlobal;
	private Vector3 Posição;

	private bool AtacandoLonge;
	private bool AtacandoPerto;

	[HideInInspector]
	public float DireçãoDaNormal;
	[HideInInspector]
	public float DireçãoDoNinho;

	private float Seno;
	private float Tangente;
	private float Cosseno;
	private float DistanciaDoinimigo;
	private float Distancia;
	private float DistanciaEmRadius;
	private float DistanciaDoMaisProximo;
	private float DistanciaDoInimigoParaNinho;

	[Tooltip("Angulo de visão da Area da Aranha")]
	[Range(0,360)]public float AnguloDeVisão;

	[Tooltip("Angulo em que o Projectil sera lançado")]
	[Range(0,360)]public float AnguloDeTiro = 15;

	[Tooltip("Distancia entre o Inimigo e a Aranha")]
	[Range(2,100)]public float DistanciaEntreAlvoEAranha = 5;

	[Tooltip("Ponto inicial da Area da Aranha")]
	[Range(0.01F,2F)]public float PontoInicial = 1F;

	[Tooltip("Ponto máximo que a Aranha pode se afastar")]
	[Range(0,100)]public float PontoMaximo = 40;

	[Tooltip("Raio da Area de Percepção da Aranha")]
	[Range(0,100)]public float RaioDevisão;

	private int index;
	private int RandomInteger;

	[Tooltip("Velocidade em que a Aranha vai rotacionar ao andar")]
	[Range(0,15)]public int VelocidadeDeRotação;

	[Tooltip("Velocidade em que a Aranha se move")]
	[Range(0,10)]public int Velocidade;

	[Range(0.1F,2F)]public int DetectionTime;
	#endregion

	#region Parte-1 Comportamento da Aranha
	public void Start () 
	{
		AgenteDaAranha = GetComponent<NavMeshAgent> ();
		AnimatorDaAranha = GetComponent<Animator> ();
		SonsDaAranha = GetComponent<AudioSource> ();
		Clip = GetComponent <NavAranhaSound>();
		Teia = Resources.Load ("Prefabs/Npcs/Aranha/Teia") as GameObject;

		if (!isServer)
			return;

		PosiçãoDoNinho ();

		AtacandoLonge = true;
		AtacandoPerto = true;

		StartCoroutine (Perception (DetectionTime));
	}

	public void PosiçãoDoNinho()
	{
		Ninho = new GameObject ("Ninho Da Aranha");
		Vector3 _ninhoPosition = transform.position;
		_ninhoPosition.y = 0;
		Quaternion _ninhoRotation = transform.rotation;
		Ninho.transform.position = _ninhoPosition;
		Ninho.transform.rotation = _ninhoRotation;
	}

	public void FixedUpdate()
	{
		if (!isServer)
			return;

		Comportamento ();
		Atributos ();
	}

	public void Comportamento ()
	{
		if(Percebidos.Count == 0)
		{
			Parado ();
		}

		if(Percebidos.Count > 0)
		{
			Perseguindo ();
		}
	}

	public void Atributos()
	{
		//Normal da Aranha
		Normal = transform.forward;
		Normal.y = 0;
		DireçãoDaNormal = Mathf.Round(Quaternion.LookRotation(Normal).eulerAngles.y);

		//Normal do Ninho
		NormalNinho = Ninho.transform.forward;
		NormalNinho.y = 0;
		DireçãoDoNinho = Mathf.Round (Quaternion.LookRotation (NormalNinho).eulerAngles.y);

		//Distancia Da Aranha para Ninho
		DistanciaEmRadius = (transform.position - Ninho.transform.position).sqrMagnitude;
	}

	public void Parado ()
	{

		AgenteDaAranha.SetDestination (Ninho.transform.position);

		AgenteDaAranha.speed = Velocidade;
		AgenteDaAranha.stoppingDistance = 0;

		AnimatorDaAranha.SetBool ("Atack_Longo", false);
		AnimatorDaAranha.SetInteger("Atack_Curto", 0);

		InimigoMaisProximo = null;


		if(DistanciaEmRadius <= PontoInicial)
		{

			if(DireçãoDaNormal != DireçãoDoNinho)
			{
				transform.rotation = Quaternion.Slerp (transform.rotation, Ninho.transform.rotation, Time.deltaTime * 3);
			}

			if(DireçãoDaNormal == DireçãoDoNinho)
			{
				AnimatorDaAranha.SetBool ("Andando",false);
			}

		}
		else
		{
			AnimatorDaAranha.SetBool ("Andando",true);
		}
	}

	public void Perseguindo ()
	{

		DistanciaDoMaisProximo = Vector3.Distance (transform.position, InimigoMaisProximo.position);

		DistanciaDoInimigoParaNinho = (InimigoMaisProximo.position - Ninho.transform.position).sqrMagnitude;

		AgenteDaAranha.SetDestination (InimigoMaisProximo.position);

		AnimatorDaAranha.SetBool ("Andando",true);

		if(DistanciaEmRadius > PontoMaximo)
		{

			StopCoroutine ("AtackCurtoAtivo");
			AtacandoPerto = true;

			if(DistanciaDoInimigoParaNinho > PontoMaximo)
			{

				AgenteDaAranha.speed = 0;
				AgenteDaAranha.stoppingDistance = RaioDevisão;
				AgenteDaAranha.velocity = Vector3.zero;
				AnimatorDaAranha.SetBool ("Atack_Longo", true);
				transform.LookAt (InimigoMaisProximo.position);

				if(AtacandoLonge)
				{
					StartCoroutine ("AtackLongoAtivo");
					AtacandoLonge = false;
				}
			}

			if (DistanciaDoInimigoParaNinho <= PontoMaximo)
			{

				AnimatorDaAranha.SetBool ("Atack_Longo", false);

				AgenteDaAranha.speed = Velocidade;
				AgenteDaAranha.stoppingDistance = 0;

			}
		}

		if(DistanciaEmRadius <= PontoMaximo)
		{

			StopCoroutine ("AtackLongoAtivo");
			AtacandoLonge = true;

			AnimatorDaAranha.SetBool ("Atack_Longo", false);

			if(DistanciaDoMaisProximo < DistanciaEntreAlvoEAranha)
			{
				RandomInteger = Random.Range (1, 3);
				AnimatorDaAranha.SetInteger("Atack_Curto", RandomInteger);

				AgenteDaAranha.stoppingDistance = DistanciaEntreAlvoEAranha - 1;
				AgenteDaAranha.speed = 0;
				AgenteDaAranha.velocity = Vector3.zero;
				transform.LookAt (InimigoMaisProximo.position);

				if(AtacandoPerto)
				{
					StartCoroutine ("AtackCurtoAtivo");
					AtacandoPerto = false;
				}
			}

			if(DistanciaDoMaisProximo >= DistanciaEntreAlvoEAranha)
			{

				AnimatorDaAranha.SetInteger("Atack_Curto", 0);
				AgenteDaAranha.stoppingDistance = 0;
				AgenteDaAranha.speed = Velocidade;
			}
		}

	}


	public IEnumerator AtackLongoAtivo()
	{
		yield return new WaitForSeconds (AnimatorDaAranha.GetCurrentAnimatorStateInfo(0).length - 0.2F);

		AtacandoLonge = true;

		if(InimigoMaisProximo != null && DistanciaEmRadius > PontoMaximo)
		{
			Vector3 Direction = (transform.position - InimigoMaisProximo.position).normalized;

			Projectil = Instantiate (Teia, PontoDeTiro.position, Quaternion.LookRotation(-Direction)) as GameObject;

			NetworkServer.Spawn (Projectil);

			Projectil.GetComponent<Rigidbody>().AddForce(-Direction * 30,ForceMode.Impulse); // ADD PROJECTIL AQUI

			RpcSendShoot (Projectil,Direction);
		}
	}

	[ClientRpc]
	public void RpcSendShoot(GameObject thisOne,Vector3 Direction)
	{
		thisOne.transform.rotation = Quaternion.LookRotation (-Direction);
		thisOne.GetComponent<Rigidbody>().AddForce(-Direction * 30,ForceMode.Impulse);
	}


	public IEnumerator AtackCurtoAtivo()
	{

		yield return new WaitForSeconds (AnimatorDaAranha.GetCurrentAnimatorStateInfo(0).length - 0.2F);

		if(Damage.isHere)
		{
			Damage.isThis.GetComponent <PlayerStats>().TakeDamage (10F);
		}

		AtacandoPerto = true;

		//Chamada de atack curto + Efeito
		//Utilities.PlayEffectAt ()
	}
	#endregion

	#region Parte-2 Detectar o mais próximo inimigo
	public IEnumerator Perception(float Time)		
	{
		while(true)
		{
			yield return new WaitForSeconds (Time);
			Percebido();
		}
	}

	void Percebido()
	{
		Percebidos.Clear ();

		PercebidosNaPercepção = Physics.OverlapSphere (transform.position, RaioDevisão,Inimigos);

		for(index = 0; index < PercebidosNaPercepção.Length; index++)
		{
			Alvo = PercebidosNaPercepção [index].transform;

			DireçãoDoInimigo = (Alvo.position - transform.position).normalized;

			if (Vector3.Angle (transform.forward, DireçãoDoInimigo) < AnguloDeVisão / 2)
			{
				DistanciaDoinimigo = Vector3.Distance (transform.position, Alvo.position);

				if(!Physics.Raycast(transform.position,DireçãoDoInimigo,DistanciaDoinimigo,Obstaculos))
				{
					Percebidos.Add (Alvo);
					PegarInimigoMaisProximo (Percebidos);
				}
			}
		}
	}

	public Transform PegarInimigoMaisProximo (List<Transform> InimigosPercebidos)
	{
		InimigoMaisProximo = null;

		Distancia = Mathf.Infinity;

		PosiçãoAtual = transform.position;

		foreach(Transform InimigoNaVisão in InimigosPercebidos)
		{
			DireçãoDoInimigo = InimigoNaVisão.position - PosiçãoAtual;

			DistanciaDoinimigo = DireçãoDoInimigo.sqrMagnitude;

			if(DistanciaDoinimigo < Distancia)
			{
				Distancia = DistanciaDoinimigo;
				InimigoMaisProximo = InimigoNaVisão;
			}
		}
		return InimigoMaisProximo;
	}

	public Vector3 DireçãoDoAngulo (float AngulosEmGraus,bool AnguloGlobal)
	{
		if(!AnguloGlobal)
		{
			AngulosEmGraus += transform.eulerAngles.y;
		}
		return new Vector3 (Mathf.Sin (AngulosEmGraus * Mathf.Deg2Rad), 0, Mathf.Cos (AngulosEmGraus * Mathf.Deg2Rad));
	}
	#endregion

	#region Parte-3 Som Sync
	public void AudioSync1()
	{
		if (!isServer)
			return;

		SonsDaAranha.PlayOneShot (Clip.AranhaSoms[0]);
		RpcReproduceAudio (0);
	}

	public void AudioSync2()
	{
		if (!isServer)
			return;

		SonsDaAranha.PlayOneShot (Clip.AranhaSoms[1]);
		RpcReproduceAudio (1);
	}

	public void AudioSync3()
	{
		if (!isServer)
			return;

		SonsDaAranha.PlayOneShot (Clip.AranhaSoms[2]);
		RpcReproduceAudio (2);
	}

	[ClientRpc]
	public void RpcReproduceAudio(int Number)
	{
		SonsDaAranha.PlayOneShot (Clip.AranhaSoms[Number]);
	}
	#endregion
}