using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Security.Cryptography;

public class NavMorcego : NetworkBehaviour 
{
	private AudioSource MorcegoSource;
	private Animator MorcegoAnimator;
	private NavMeshAgent MorcegoAgent;

	[HideInInspector]
	public Collider[] Invasores;

	public LayerMask Inimigos;

	private NavMorcegoSound Clip;
	private Health MorcegoHealth;
	private Mana MorcegoMana;

	[HideInInspector]
	public NavMorcegoDamager MorcegoDamage;

	private float DistanciaEntreMorcegoEInvasor;

	[Range(0F,15F)]public float VelocidadeInicial;

	[Range(0,10)]public int MinimoParaAtack;
	[Range(0,50)]public int Raio;

	private bool Complete;
	private bool AtackCompleto_1;
	private bool AtackCompleto_2;
	private int _counterAtack;

	public void Start () 
	{
		MorcegoAgent = GetComponent <NavMeshAgent> ();
		MorcegoAnimator = GetComponent <Animator> ();
		MorcegoSource = GetComponent <AudioSource> ();
		MorcegoHealth = GetComponent <Health> ();
		MorcegoMana = GetComponent <Mana> ();
		Clip = GetComponent <NavMorcegoSound> ();

		if(!isServer)
			return;

		Complete = true;
		AtackCompleto_1 = true;
		AtackCompleto_2 = false;
	}

	public void FixedUpdate () 
	{
		if (!isServer)
			return;
		
		Atributos ();
		Comportamento ();
	}

	public void Atributos()
	{
		Invasores = Physics.OverlapSphere (transform.position, Raio ,Inimigos);
	}

	public void Comportamento()
	{
		if(Invasores.Length == 0)
		{
			Parado ();
		}

		if(Invasores.Length > 0)
		{
			Perseguindo ();
		}
	}

	public void Parado()
	{
		StopCoroutine (Atack_1 ());
		StopCoroutine (Atack_2 ());
		MorcegoAnimator.SetInteger ("Atacando",0);
		MorcegoAnimator.SetBool ("Andando",false);
		MorcegoAgent.velocity = Vector3.zero;
		MorcegoAgent.speed = 0;
		AtackCompleto_1 = true;
		Complete = true;
	}

	public void Perseguindo()
	{
		DistanciaEntreMorcegoEInvasor = Vector3.Distance (transform.position, Invasores[0].transform.position);

		if(DistanciaEntreMorcegoEInvasor < MinimoParaAtack)
		{
			if (_counterAtack == 3 && Complete) {
				AtackCompleto_2 = true;
				Complete = false;
			}

			if(AtackCompleto_1 && _counterAtack != 3)
			{
				AtackCompleto_1 = false;
				StartCoroutine (Atack_1 ());
			}

			if(AtackCompleto_2)
			{
				AtackCompleto_2 = false;
				StartCoroutine (Atack_2 ());
			}
		}
		else
		{
			MorcegoAgent.speed = VelocidadeInicial;
			MorcegoAnimator.SetBool ("Andando",true);
			MorcegoAnimator.SetInteger("Atacando",0);
			MorcegoAgent.SetDestination (Invasores[0].transform.position);
		}
	}

	public void AudioSync1()
	{
		if (!isServer)
			return;

		MorcegoSource.PlayOneShot (Clip.MorcegoSoms[0]);
		RpcReproduceAudio (0);
	}

	public void AudioSync2()
	{
		if (!isServer)
			return;

		MorcegoSource.PlayOneShot (Clip.MorcegoSoms[1]);
		RpcReproduceAudio (1);
	}

	public void AudioSync3()
	{
		if (!isServer)
			return;

		MorcegoSource.PlayOneShot (Clip.MorcegoSoms[2]);
		RpcReproduceAudio (2);
	}

	public IEnumerator Atack_1()
	{
		MorcegoAnimator.SetInteger ("Atacando",2);
		MorcegoAgent.speed = 0;
		MorcegoAgent.velocity = Vector3.zero;
		transform.LookAt (Invasores[0].transform.position);

		yield return new WaitForSeconds (MorcegoAnimator.GetCurrentAnimatorClipInfo (0).Length + 0.4F);

		if(MorcegoDamage.isHere)
		{

			if(MorcegoMana.CurrentMana >= 80)
				MorcegoMana.GainMana (20F);
			else
				MorcegoMana.GainMana (40F);

			if (MorcegoHealth.CurrentHealth != 100)
				MorcegoHealth.RecoveryHealth (10F);

			MorcegoDamage.isThis.GetComponent<Health>().TakeDamage(10F,this.gameObject);

			_counterAtack++;
		}
			
		AtackCompleto_1 = true;

	}

	public IEnumerator Atack_2()
	{
		MorcegoAnimator.SetInteger ("Atacando",1);
		MorcegoAgent.speed = 0;
		MorcegoAgent.velocity = Vector3.zero;
		transform.LookAt (Invasores[0].transform.position);

		yield return new WaitForSeconds (MorcegoAnimator.GetCurrentAnimatorClipInfo (0).Length + 1F);

		if(MorcegoDamage.isHere)
		{
			MorcegoMana.CurrentMana = 0;
			MorcegoDamage.isThis.GetComponent<Health>().TakeDamage(20F,this.gameObject);
			_counterAtack = 0;
			AtackCompleto_2 = false;
			Complete = true;
		}
	}

	[ClientRpc]
	public void RpcReproduceAudio(int Number)
	{
		MorcegoSource.PlayOneShot (Clip.MorcegoSoms[Number]);
	}
}
