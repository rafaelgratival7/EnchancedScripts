using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NavAranha : MonoBehaviour {

	#region Declaração
	private Animator AnimatorDaAranha;

	private AudioSource[] SonsDaAranha;

	private Collider[] PercebidosNaPercepção;

	public LayerMask Inimigos;
	public LayerMask Obstaculos;

	[HideInInspector]
	public GameObject Projectil;

	private GameObject Teia;
	private GameObject PontoDeTiro;

	private NavMeshAgent AgenteDaAranha;

	private Quaternion Rotação;

	[HideInInspector]
	public List<Transform> Percebidos = new List<Transform>();

	private Transform Alvo;

	[HideInInspector]
	public Transform InimigoMaisProximo;
	[HideInInspector]
	public Transform Ninho;

	private SoundManager Clip;

	[HideInInspector]
	public Vector3 NormalNinho;

	private Vector3 DireçãoDoInimigo;
	private Vector3 PosiçãoAtual;
	private Vector3 Normal;
	private Vector3 VelocidadeLocal;
	private Vector3 VelocidadeGlobal;

	private bool AtacandoLonge;
	private bool AtacandoPerto;
	private bool InimigoPercebido;

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

	[Range(0,360)]public float AnguloDeVisão;
	[Range(0,360)]public float AnguloDeTiro = 15;
	[Range(2,1000)]public float DistanciaEntreAlvoEAranha = 5;
	[Range(0.01F,2F)]public float PontoInicial = 1F;
	[Range(0,1000)]public float PontoMaximo = 40;
	[Range(0,1000)]public float RaioDevisão;

	private int index;
	private int Temporizador;

	[Range(0,10)]public int Velocidade; 
	#endregion

	#region Parte-1 Comportamento da Aranha
	void Awake () 
	{
		AgenteDaAranha = GetComponent<NavMeshAgent> ();
		AnimatorDaAranha = GetComponent<Animator> ();
		Ninho = transform.parent.GetComponentInParent<Transform> ();
		SonsDaAranha = GetComponents<AudioSource> ();

		AtacandoLonge = true;
		AtacandoPerto = true;
		InimigoPercebido = false;

		Teia = Resources.Load ("Objetos/Teia") as GameObject;

		PontoDeTiro = GameObject.Find ("PontoDeLançamento");

		Clip = GameObject.Find ("SoundSystem").GetComponent<SoundManager> ();

		StartCoroutine ("Percepção", .2f);
		StartCoroutine ("IddleON", 1F);
	}

	void Update()
	{
		Comportamento ();
	}

	void Comportamento ()
	{
		DireçãoDaFace ();

		ControllerDoSom ();

		if(Percebidos.Count == 0)
		{
			Parado ();
		}

		if(Percebidos.Count > 0)
		{
			Perseguindo ();
		}

		DistanciaEmRadius = (transform.position - Ninho.position).sqrMagnitude;
	}

	IEnumerator AtackLongoAtivo()
	{
		SonsDaAranha [0].PlayOneShot (Clip.NPC_Aranha [4]);

		yield return new WaitForSeconds (AnimatorDaAranha.GetCurrentAnimatorStateInfo(0).length);

		SonsDaAranha [0].Stop ();

		AtacandoLonge = true;

		if(InimigoMaisProximo != null && DistanciaEmRadius > PontoMaximo)
		{
			Projectil = Instantiate (Teia, PontoDeTiro.transform.position, Quaternion.LookRotation(InimigoMaisProximo.position)) as GameObject;

			Projectil.transform.LookAt (InimigoMaisProximo.position);

			Seno = Mathf.Sqrt(DistanciaDoMaisProximo* -Physics.gravity.y / (Mathf.Sin(Mathf.Deg2Rad * AnguloDeTiro * 2)));

			Tangente = Seno * Mathf.Sin(Mathf.Deg2Rad * AnguloDeTiro);
			Cosseno = Seno * Mathf.Cos(Mathf.Deg2Rad * AnguloDeTiro);

			VelocidadeLocal = new Vector3(0f, Tangente, Cosseno);

			VelocidadeGlobal = transform.TransformVector(VelocidadeLocal);

			Projectil.GetComponent<Rigidbody>().velocity = VelocidadeGlobal;
		}
	}

	IEnumerator AtackCurtoAtivo()
	{
		SonsDaAranha [0].PlayOneShot (Clip.NPC_Aranha [3]);

		yield return new WaitForSeconds (AnimatorDaAranha.GetCurrentAnimatorStateInfo(0).length);

		SonsDaAranha [0].Stop ();

		AtacandoPerto = true;

		//Dano do atack curto aqui
	}

	void DireçãoDaFace()
	{
		Normal = transform.forward;
		Normal.y = 0;
		DireçãoDaNormal = Mathf.Round(Quaternion.LookRotation(Normal).eulerAngles.y);

		NormalNinho = Ninho.transform.forward;
		NormalNinho.y = 0;
		DireçãoDoNinho = Mathf.Round (Quaternion.LookRotation (NormalNinho).eulerAngles.y);
	}

	void Parado ()
	{

		AgenteDaAranha.SetDestination (Ninho.position);

		AgenteDaAranha.speed = Velocidade;
		AgenteDaAranha.stoppingDistance = 0;

		AnimatorDaAranha.SetBool ("Atack_Longo", false);
		AnimatorDaAranha.SetBool ("Atack_Curto", false);

		InimigoMaisProximo = null;


		if(DistanciaEmRadius <= PontoInicial)
		{

			if(DireçãoDaNormal != DireçãoDoNinho)
			{
				transform.rotation = Quaternion.Slerp (transform.rotation, Ninho.rotation, Time.deltaTime * 3);
			}

			if(DireçãoDaNormal == DireçãoDoNinho)
			{
				SonsDaAranha [1].Stop ();
				AnimatorDaAranha.SetBool ("Andando",false);
			}
				
		}
		else
		{
			AnimatorDaAranha.SetBool ("Andando",true);
		}
	}

	void Perseguindo ()
	{

		DistanciaDoMaisProximo = Vector3.Distance (transform.position, InimigoMaisProximo.position);

		DistanciaDoInimigoParaNinho = (InimigoMaisProximo.position - Ninho.position).sqrMagnitude;

		AgenteDaAranha.SetDestination (InimigoMaisProximo.position);
	
		AnimatorDaAranha.SetBool ("Andando",true);


		if(DistanciaEmRadius > PontoMaximo)
		{

			StopCoroutine ("AtackCurtoAtivo");
			AtacandoPerto = true;

			if(DistanciaDoInimigoParaNinho > PontoMaximo)
			{

				SonsDaAranha [1].Stop ();
				AgenteDaAranha.speed = 0;
				AgenteDaAranha.stoppingDistance = RaioDevisão;
				AgenteDaAranha.velocity = Vector3.zero;

				AnimatorDaAranha.SetBool ("Atack_Longo", true);

				transform.LookAt (InimigoMaisProximo);

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
				AnimatorDaAranha.SetBool ("Atack_Curto", true);

				SonsDaAranha [1].Stop ();
				AgenteDaAranha.stoppingDistance = DistanciaEntreAlvoEAranha - 1;
				AgenteDaAranha.speed = 0;
				AgenteDaAranha.velocity = Vector3.zero;


				transform.LookAt (InimigoMaisProximo);

				if(AtacandoPerto)
				{
					StartCoroutine ("AtackCurtoAtivo");
					AtacandoPerto = false;
				}
			}

			if(DistanciaDoMaisProximo >= DistanciaEntreAlvoEAranha)
			{

				AnimatorDaAranha.SetBool ("Atack_Curto", false);

				AgenteDaAranha.stoppingDistance = 0;
				AgenteDaAranha.speed = Velocidade;
			}
		}
			
	}
	#endregion

	#region Parte-2 Detectar o mais próximo inimigo
	IEnumerator Percepção(float Atraso)		
	{
		while(true)
		{
			yield return new WaitForSeconds (1.2f);
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

	Transform PegarInimigoMaisProximo (List<Transform> InimigosPercebidos)
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

	#region Parte-3 Controlador de Som da Aranha
	void ControllerDoSom()
	{
		if (AgenteDaAranha.velocity.magnitude > 2F && !SonsDaAranha[1].isPlaying)
		{
			SonsDaAranha[1].PlayOneShot (Clip.NPC_Aranha [1]);
			SonsDaAranha[1].loop = true;
		}
		else
		{
			SonsDaAranha[1].loop = false;
		}

		if(InimigoMaisProximo != null)
		{
			if(InimigoPercebido)
			{
				InimigoPercebido = false;
				SonsDaAranha[1].PlayOneShot (Clip.NPC_Aranha [2]);
			}
		}
		else
		{
			InimigoPercebido = true;
		}
	}
		
	IEnumerator IddleON()
	{
		while(true)
		{
			Temporizador = Random.Range (8, 15);

			yield return new WaitForSeconds (Temporizador);

			if(AnimatorDaAranha.GetCurrentAnimatorStateInfo(0).IsName("Aranha_Iddle") && !AnimatorDaAranha.IsInTransition(0))
			{
				SonsDaAranha[0].PlayOneShot (Clip.NPC_Aranha [0]);
				SonsDaAranha [0].volume = 0.2F;
			}
		}
	}
	#endregion
}
