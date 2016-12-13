using UnityEngine;
using System.Collections;

public class NavCastor : MonoBehaviour
{
	#region Declaração
	private Animator CastorAnimator;

	private AudioSource CastorSource;

	private SoundManager Clip;

	[HideInInspector]
	public Collider[] Invasores;

	public LayerMask Inimigos;

	private NavMeshAgent CastorAgent;

	private Transform CastorNinho;

	private Quaternion NinhoRotação;
	private Quaternion Rotação;

	private Vector3 NormalDoCastor;
	private Vector3 NormalDoNinho;
	private Vector3 VelocidadeLocal;
	private Vector3 VelocidadeGlobal;
	private Vector3 Posição;

	private GameObject TroncoNaBoca;
	private GameObject TroncoNoAr;
	private GameObject TroncoNoCastor;
	private GameObject PontoDeLançamento;

	private float DistanciaEntreCastorENinho;
	private float DistanciaEntreCastorEInvasor;
	private float DistanciaEntreInvasorENinho;
	private float DireçãoDaNormal;
	private float DireçãoDoNinho;
	private float Seno;
	private float Cosseno;
	private float Tangente;
	private float VelocidadeInicial;

	[Range (15,45)]public float AnguloDeTiro = 15;
	[Range (0.01F,2F)]public float NinhoInicial;
	[Range(50,1000)]public float NinhoFinal;

	private int Index;
	private int Temporizador;

	[Range(3,10)]public int MinimoParaAtack;
	[Range(0,1000)]public int raio;
	[Range(1,30)]public int VelocidadeDeRotação;

	private bool AtackLongoON;
	private bool AtackCurtoON;
	#endregion

	#region Parte-1 Comportamento Castor
	void Awake()
	{
		CastorAgent = GetComponent<NavMeshAgent> ();
		CastorAnimator = GetComponentInChildren<Animator> ();
		CastorNinho = transform.parent.GetComponentInParent<Transform> ();
		CastorSource = GetComponent<AudioSource> ();

		TroncoNoAr = Resources.Load ("Objetos/Tronco") as GameObject;

		Clip = GameObject.Find ("SoundSystem").GetComponent<SoundManager> ();

		AtackCurtoON = true;
		AtackCurtoON = true;

		Procurador ();
	}

	void Procurador()
	{
		TroncoNaBoca = Utilities.GetChildren (this.gameObject, "Arvore01_Tronco");
		PontoDeLançamento = Utilities.GetChildren (this.gameObject, "PontoDeLançamento");
	}

	void Start () 
	{
		TroncoNaBoca.SetActive (false);
		VelocidadeInicial = CastorAgent.speed;
		NinhoRotação = CastorNinho.rotation;

		StartCoroutine ("Iddle");
		StartCoroutine ("Atack");
	}

	void Update () 
	{
		Comportamento ();
	}

	void Comportamento()
	{
		DireçãoDaFace ();
		InimigoPercebido ();

		if(Invasores.Length == 0)
		{
			Parado ();
		}

		if(Invasores.Length > 0)
		{
			Perseguindo ();
		}

		DistanciaEntreCastorENinho = Mathf.Round((transform.position - CastorNinho.position).sqrMagnitude);
	}

	void Parado ()
	{

		CastorAgent.SetDestination (CastorNinho.position);

		CastorAnimator.SetBool ("Atack_Curto", false);
		CastorAnimator.SetBool ("Atack_Longo", false);

		TroncoNaBoca.SetActive (false);

		if(DistanciaEntreCastorENinho <= NinhoInicial)
		{

			if(DireçãoDaNormal != DireçãoDoNinho)
			{
				transform.rotation = Quaternion.Slerp (transform.rotation, NinhoRotação, Time.deltaTime * 3);
			}

			if(DireçãoDaNormal == DireçãoDoNinho)
			{
				CastorAnimator.SetBool ("Andando", false);
			}
		}
		else
		{
			CastorAnimator.SetBool ("Andando", true);
		}
	}

	void Perseguindo()
	{

		DistanciaEntreCastorEInvasor = Vector3.Distance (transform.position, Invasores[0].transform.position);

		DistanciaEntreInvasorENinho = (Invasores[0].transform.position - CastorNinho.position).sqrMagnitude;

		CastorAgent.SetDestination (Invasores[0].transform.position);

		CastorAnimator.SetBool ("Andando", true);

		Posição = Invasores[0].transform.position - transform.position;

		Posição.y = 0;

		Rotação = Quaternion.LookRotation(Posição);

		transform.rotation = Quaternion.Slerp(transform.rotation, Rotação, Time.deltaTime * VelocidadeDeRotação);

		if(DistanciaEntreCastorENinho > NinhoFinal)
		{

			StopCoroutine ("Atack_Curto_Ativo");
			AtackCurtoON = true;

			if(DistanciaEntreInvasorENinho > NinhoFinal)
			{

				CastorAgent.velocity = Vector3.zero;

				CastorAnimator.SetBool ("Atack_Longo", true);

				if(AtackLongoON)
				{
					StartCoroutine ("Atack_Longo_Ativo");
					AtackLongoON = false;
				}
			}

			if (DistanciaEntreInvasorENinho <= NinhoFinal)
			{
				CastorAnimator.SetBool ("Atack_Longo", false);
			}
		}

		if(DistanciaEntreCastorENinho <= NinhoFinal)
		{
			StopCoroutine ("Atack_Longo_Ativo");
			AtackLongoON = true;

			CastorAnimator.SetBool ("Atack_Longo", false);

			if(DistanciaEntreCastorEInvasor < MinimoParaAtack)
			{

				CastorAnimator.SetBool ("Atack_Curto", true);

				CastorAgent.velocity = Vector3.zero;

				if(AtackCurtoON)
				{
					StartCoroutine ("Atack_Curto_Ativo");
					AtackCurtoON = false;
				}

			}

			if(DistanciaEntreCastorEInvasor >= MinimoParaAtack)
			{
				CastorAnimator.SetBool ("Atack_Curto", false);
			}
		}
	}

	IEnumerator Atack_Curto_Ativo()
	{

		yield return new WaitForSeconds (CastorAnimator.GetCurrentAnimatorStateInfo(0).length);

		//Damage here

		AtackCurtoON = true;
	}

	IEnumerator Atack_Longo_Ativo()
	{
		yield return new WaitForSeconds (CastorAnimator.GetCurrentAnimatorStateInfo(0).length);

		//Daamge here

		AtackLongoON = true;
	}
	#endregion

	#region Parte-2 Controle de Animações
	public void PegarArvore()
	{
		TroncoNaBoca.SetActive(true);
	}

	public void LargarArvore()
	{
		TroncoNaBoca.SetActive(false);

		Voador ();
	}

	void Voador()
	{
		if(Invasores != null)
		{
			new WaitForSeconds (1);

			TroncoNoCastor = Instantiate (TroncoNoAr,TroncoNaBoca.transform.position, TroncoNaBoca.transform.rotation) as GameObject;
			TroncoNoCastor.transform.LookAt (Invasores [0].transform.position);

			Seno = Mathf.Sqrt(DistanciaEntreCastorEInvasor* -Physics.gravity.y / (Mathf.Sin(Mathf.Deg2Rad * AnguloDeTiro * 2)));

			Tangente = Seno * Mathf.Sin(Mathf.Deg2Rad * AnguloDeTiro);
			Cosseno = Seno * Mathf.Cos(Mathf.Deg2Rad * AnguloDeTiro);

			VelocidadeLocal = new Vector3(0f, Tangente, Cosseno);

			VelocidadeGlobal = transform.TransformVector(VelocidadeLocal);

			TroncoNoCastor.GetComponent<Rigidbody>().velocity = VelocidadeGlobal;
			TroncoNoCastor.GetComponent<Rigidbody>().AddTorque(Vector3.one * 32) ;
		}
	}

	void InimigoPercebido()
	{
		Invasores = Physics.OverlapSphere (transform.position, raio ,Inimigos);
	}

	void DireçãoDaFace()
	{
		NormalDoCastor = transform.forward;
		NormalDoCastor.y = 0;
		DireçãoDaNormal = Mathf.Round(Quaternion.LookRotation(NormalDoCastor).eulerAngles.y);

		NormalDoNinho = CastorNinho.transform.forward;
		NormalDoNinho.y = 0;
		DireçãoDoNinho = Mathf.Round (Quaternion.LookRotation (NormalDoNinho).eulerAngles.y);
	}
	#endregion

	#region Parte-3 Controle do Som
	IEnumerator Iddle()
	{
		while(true)
		{
			Temporizador = Random.Range (5, 10);

			yield return new WaitForSeconds (Temporizador);

			if(CastorAnimator.GetCurrentAnimatorStateInfo(0).IsName("Castor_Iddle") && !CastorAnimator.IsInTransition(0))
			{
				CastorSource.PlayOneShot (Clip.NPC_Castor[0]);
			}
		}
	}

	IEnumerator Atack()
	{
		while(true)
		{
			yield return new WaitForSeconds (CastorAnimator.GetCurrentAnimatorStateInfo(0).length);

			if(CastorAnimator.GetCurrentAnimatorStateInfo(0).IsName("Castor_Atack_Curto") && !CastorAnimator.IsInTransition(0))
			{
				CastorSource.PlayOneShot (Clip.NPC_Castor[1]);
			}
		}
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere (transform.position,raio);
	}
	#endregion
}
