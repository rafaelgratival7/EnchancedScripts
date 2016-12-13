using UnityEngine;
using System.Collections;

public class NavLarva : MonoBehaviour 
{
	#region Declaração
	private Animator LarvaAnimator;

	private SoundManager Clip;

	private AudioSource LarvaAudio;

	private NavMeshAgent LarvaAgent;

	private PlayerScript Player; // Player Script << Para Acessar Inimigo

	private Vector3 Parametros;
	private Vector3 PosiçãoNoNinho;
	private Vector3 Normal;
	private Vector3 Posição;

	private Quaternion Rotação;

	private Transform PlayerLocal;
	private Transform Avo;

	public float DireçãoDaNormal;
	private float DistanciaEntreMinionEInimigo;
	private float DistanciaEntreMinionEPosiçãoInicial;
	private float DistanciaEntreInvasorEPosiçãoInicial;
	private float DistanciaEntreInimigoEPosiçãoInicial;
	private float VelocidadeInicial;

	[Range(0.01F,2F)]public float PontoInicial = 0.01F;
	[Range(10,100)]public float PontoMaximo = 50;

	[Range(2.6F,4F)]public int MinimoParaAtack = 3;

	private bool Atacando = true;
	#endregion

	#region Parte-1 Larva Configuração
	void Start () 
	{
		LarvaAgent = GetComponent<NavMeshAgent> ();
		LarvaAnimator = GetComponent<Animator> ();
		LarvaAudio = GetComponent<AudioSource> ();

		Avo = Utilities.GetFather(this.transform);

		Player = Avo.GetComponent<PlayerScript> (); // PlayerScript
		PlayerLocal = Avo.GetComponent<Transform> (); // PlayerLocal

		Clip = GameObject.Find ("SoundSystem").GetComponent<SoundManager> ();

		Gravar ();
	}

	void Gravar ()
	{
		VelocidadeInicial = LarvaAgent.speed;
	}
	#endregion

	#region Parte-2 Larva Comportamento
	void Update ()
	{
		ComportamentoMinion ();
		Atribuidores ();
		ControleSom ();
	}

	void Atribuidores()
	{
		Rotação = PlayerLocal.rotation;

		//Direção Da Face
		Normal = transform.forward;
		Normal.y = 0;
		DireçãoDaNormal = Mathf.Round(Quaternion.LookRotation(Normal).eulerAngles.y);

		PosiçãoNoNinho = transform.parent.transform.position;
	}

	void ComportamentoMinion()
	{
		if(Player.Invasores.Length == 0)
		{
			ParadoMinion ();
		}

		if(Player.Invasores.Length > 0)
		{
			PerseguindoMinion ();
		}

		DistanciaEntreMinionEPosiçãoInicial = Mathf.Round((transform.position - PosiçãoNoNinho).sqrMagnitude);
	}

	void ParadoMinion()
	{
		LarvaAgent.SetDestination (PosiçãoNoNinho);
		LarvaAnimator.SetBool ("Atacando", false);
		LarvaAnimator.SetInteger ("Atack",0);

		if(DistanciaEntreMinionEPosiçãoInicial <= PontoInicial)
		{

			if(DireçãoDaNormal != Player.DireçãoDaNormalPlayer)
			{
				transform.rotation = Quaternion.Slerp (transform.rotation,Rotação, Time.deltaTime * 4);
			}

			if(DireçãoDaNormal == Player.DireçãoDaNormalPlayer)
			{
				LarvaAudio.Stop ();
				LarvaAnimator.SetBool ("Andando", false);
			}
		}

		if(DistanciaEntreMinionEPosiçãoInicial > PontoInicial)
		{
			LarvaAnimator.SetBool ("Andando", true);
		}
	}

	void PerseguindoMinion()
	{
		LarvaAnimator.SetBool ("Andando", true);

		Posição = Player.Invasores[0].transform.position - transform.position;

		Posição.y = 0;

		Rotação = Quaternion.LookRotation(Posição);

		transform.rotation = Quaternion.Slerp(transform.rotation, Rotação, Time.deltaTime * 3);


		// Inimigo
		if(Player.Invasores != null)
		{
			LarvaAgent.SetDestination (Player.Invasores[0].transform.position);

			DistanciaEntreMinionEInimigo = Vector3.Distance (transform.position, Player.Invasores[0].transform.position);

			if(DistanciaEntreMinionEInimigo <= MinimoParaAtack)
			{
				LarvaAgent.speed = 0;
				LarvaAgent.stoppingDistance = MinimoParaAtack;
				LarvaAgent.velocity = Vector3.zero;
				LarvaAnimator.SetBool ("Andando", false);
				LarvaAnimator.SetBool ("Atacando", true);

				if(Atacando)
				{
					StartCoroutine ("AtackMinion");
					Atacando = false;
				}
			}

			if(DistanciaEntreMinionEInimigo > MinimoParaAtack)
			{
				LarvaAgent.speed = VelocidadeInicial;
				LarvaAgent.stoppingDistance = 0;
				LarvaAnimator.SetBool ("Andando", true);
				LarvaAnimator.SetBool ("Atacando", false);
				LarvaAnimator.SetInteger ("Atack",0);

				Atacando = true;
				StopCoroutine ("AtackMinion");
			}
		}
	}

	IEnumerator AtackMinion()
	{
		LarvaAudio.PlayOneShot (Clip.NPC_Larva [1]);

		LarvaAnimator.SetInteger ("Atack",2);

		yield return new WaitForSeconds (LarvaAnimator.GetCurrentAnimatorStateInfo(0).length);

		//Dano Here

		LarvaAudio.Stop ();

		Atacando = true;
	}
	#endregion

	#region Parte-3 Controle Do Som
	void ControleSom()
	{
		if (LarvaAgent.velocity.magnitude > 2F && !LarvaAudio.isPlaying)
		{
//			LarvaAudio.PlayOneShot (Clip.Sons [3]);
			LarvaAudio.loop = true;
		}
		else
		{
			LarvaAudio.loop = false;
		}
	}
	#endregion
}
