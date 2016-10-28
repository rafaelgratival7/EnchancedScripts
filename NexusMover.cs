using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Net;
using System.Security.Cryptography;

public class NexusMover : NetworkBehaviour
{
	#region Declaration
	private AudioSource NexusSource;
	private Animator NexusAnimator;
	private NavMeshAgent NexusAgent;

	[HideInInspector]
	public Collider[] Invasores;
	[HideInInspector]
	public Collider[] ArvoreDestroyer;

	public LayerMask Inimigos;
	public LayerMask Arvores;

	private Transform PointToFollow;

	private NexusSound Clip;
	private NexusHand _leftHandAtack;
	private NexusHead _headAtack;
	private NexusFeet _feetAtack;
	private NexusBody _areaAtack;

	private float DistanciaEntreNexusEInvasor;

	[SyncVar][Range(0,10)]public float VelocidadeInicial;
	[SyncVar][Range(0,10)]public float VelocidadeDeMovimento;

	[Range(0,1)]public int Identify;
	[Range(0,5)]public int MinimoParaAtack;
	[Range(0,50)]public int Raio;

	[SyncVar]private bool AtackCompleto;
	#endregion

	public void Start()
	{
		NexusAgent = GetComponent<NavMeshAgent> ();
		NexusAnimator = GetComponent<Animator> ();
		NexusSource = GetComponent<AudioSource> ();
		Clip = GetComponent <NexusSound> ();
		_leftHandAtack = Utilities.GetChildren (this.gameObject,"HandContactPoint").GetComponent <NexusHand>();
		_feetAtack = Utilities.GetChildren (this.gameObject, "FeetContactPoint").GetComponent <NexusFeet>();
		_headAtack = Utilities.GetChildren (this.gameObject,"HeadContactPoint").GetComponent <NexusHead>();
		_areaAtack = Utilities.GetChildren (this.gameObject, "BoddyContactPoint").GetComponent <NexusBody>();

		if (Identify == 0)
			PointToFollow = Utilities.FindInHierarchyNetworked ("PFR").GetComponent <Transform>();
		else
			PointToFollow = Utilities.FindInHierarchyNetworked ("PFB").GetComponent <Transform>();

		if (!isServer)
			return;
		
		AtackCompleto = true;
		VelocidadeInicial = NexusAgent.speed;
	}

	public void Update()
	{
		if (!isServer)
			return;

		Comportamento();
		Atributos ();
	}

	public void Atributos()
	{

		//Checker de Arvore Proxima
		ArvoreDestroyer = Physics.OverlapSphere (transform.position, Raio, Arvores);

		for(int i = 0; i < ArvoreDestroyer.Length; i++)
		{
			Transform Alvo = ArvoreDestroyer [i].transform;

			if(Alvo.tag == "Barreira")
			{
				Alvo.GetComponent <CaixaScript>().TakeDamageToDrop (3,Vector3.forward);
			}
		}

		//Checker de Inimigo Proximo
		Invasores = Physics.OverlapSphere (transform.position, Raio ,Inimigos);
	}

	public void Comportamento()
	{
		if(Invasores.Length == 0)
		{
			Parado ();
			//Point to Follower
		}

		if(Invasores.Length > 0)
		{
			Perseguindo ();
			Debug.Log ("inimigo Localizado");
		}
	}
		
	public void Parado()
	{
		NexusAnimator.SetInteger ("Atacando",0);
		NexusAgent.speed = VelocidadeInicial;
		StopCoroutine (Atacando ());

		if (NexusAgent.remainingDistance <= 1)
		{
			NexusAgent.velocity = Vector3.zero;
			int randomizer = Random.Range (5, 10);
			StopCoroutine (CantReach (10));
			StartCoroutine (StopTime (randomizer));
			Debug.Log ("Pausa ao Chegar a um Local");
		}
		else
		{
			NexusAnimator.SetBool ("Andando",true);
			StartCoroutine (CantReach (10));
		}
	}

	public void Perseguindo()
	{
		DistanciaEntreNexusEInvasor = Vector3.Distance (transform.position, Invasores[0].transform.position);

		StopCoroutine (CantReach (10));

		if(DistanciaEntreNexusEInvasor < MinimoParaAtack)
		{
			if(AtackCompleto)
			{
				AtackCompleto = false;
				Debug.Log ("Atacking");
				StartCoroutine (Atacando ());
			}
		}
		else
		{
			Debug.Log ("Chasing");
			StopCoroutine (Atacando ());
			Chasing ();
		}
	}

	public void Chasing()
	{
		AtackCompleto = true;
		NexusAnimator.SetInteger ("Atacando", 0);
		NexusAnimator.SetBool ("Andando", true);
		NexusAgent.speed = VelocidadeInicial;
		NexusAgent.SetDestination (Invasores[0].transform.position);
	}

	public void AudioSync1()
	{
		if (!isServer)
			return;

		NexusSource.PlayOneShot (Clip.NexusSoms[0]);
		RpcReproduceAudio (0);
	}

	public void AudioSync2()
	{
		if (!isServer)
			return;

		NexusSource.PlayOneShot (Clip.NexusSoms[1]);
		RpcReproduceAudio (1);
	}

	public void AudioSync3()
	{
		if (!isServer)
			return;

		NexusSource.PlayOneShot (Clip.NexusSoms[2]);
		RpcReproduceAudio (2);
	}

	public void AudioSync4()
	{
		if (!isServer)
			return;

		NexusSource.PlayOneShot (Clip.NexusSoms[3]);
		RpcReproduceAudio (3);
	}

	public IEnumerator CantReach(int Delay)
	{
		yield return new WaitForSeconds (Delay);

		NexusAgent.destination = PointToFollow.position;
	}

	public IEnumerator StopTime(int Amount)
	{
		NexusAnimator.SetBool ("Andando",false);

		yield return new WaitForSeconds (Amount);

		NexusAnimator.SetBool ("Andando",true);

		NexusAgent.destination = PointToFollow.position;
	}

	public IEnumerator Atacando()
	{
		NexusAnimator.SetBool ("Andando", false);
		int RandomAtack = Random.Range (1, 5);
		NexusAnimator.SetInteger ("Atacando",RandomAtack);
		NexusAgent.velocity = Vector3.zero;
		NexusAgent.speed = 0;
		transform.LookAt (Invasores [0].transform.position);

		yield return new WaitForSeconds(NexusAnimator.GetCurrentAnimatorClipInfo(0).Length + 1.2f);

		if(RandomAtack == 1)
		{
			if(_feetAtack.isHere)
			{
				_feetAtack.isThis.GetComponent <Health> ().TakeDamage (10, this.gameObject);
				Debug.Log ("Damage From Feet");
			}
		}

		if(RandomAtack == 2)
		{
			if(_leftHandAtack.isHere)
			{
				_leftHandAtack.isThis.GetComponent <Health> ().TakeDamage (10, this.gameObject);
				Debug.Log ("Damage From Hand");
			}
		}

		if(RandomAtack == 4)
		{
			if(_headAtack.isHere)
			{
				Vector3 Direction = (transform.position - _headAtack.isThis.transform.position).normalized;
				_headAtack.isThis.GetComponent <Rigidbody> ().constraints = RigidbodyConstraints.None;
				_headAtack.isThis.GetComponent <Rigidbody> ().constraints = RigidbodyConstraints.FreezeRotation /*| RigidbodyConstraints.FreezePositionY*/;
				_headAtack.isThis.GetComponent <Rigidbody> ().useGravity = true;
				_headAtack.isThis.GetComponent <Rigidbody> ().mass = 5;
				_headAtack.isThis.GetComponent <Rigidbody> ().AddForce (-Direction * 50,ForceMode.Impulse);
				_headAtack.isThis.GetComponent <Health> ().TakeDamage(10,this.gameObject);
				RpcPushPlayerHead (_headAtack.isThis,Direction);
				Debug.Log ("Damage From Head");
			}
		}

		if(RandomAtack == 3)
		{
			if(_areaAtack.isHere)
			{
				foreach(GameObject Player in _areaAtack.isThoses)
				{
					Vector3 Direction = (transform.position - Player.transform.position).normalized;
					Player.GetComponent <Rigidbody> ().constraints = RigidbodyConstraints.None;
					Player.GetComponent <Rigidbody> ().constraints = RigidbodyConstraints.FreezeRotation /*| RigidbodyConstraints.FreezePositionY*/;
					Player.GetComponent <Rigidbody> ().useGravity = true;
					Player.GetComponent <Rigidbody> ().mass = 5;
					Player.GetComponent <Rigidbody> ().AddForce (-Direction * 150,ForceMode.Impulse);
					Player.GetComponent <Health> ().TakeDamage(10,this.gameObject);
					RpcPushPlayers (Player,Direction);
					Debug.Log ("Damage Done");
				}
			}
		}

		AtackCompleto = true;
	}

	[ClientRpc]
	public void RpcPushPlayerHead(GameObject thisPlayer, Vector3 thisDirection)
	{
		thisDirection = (transform.position - thisPlayer.transform.position).normalized;
		thisPlayer.GetComponent <Rigidbody> ().constraints = RigidbodyConstraints.None;
		thisPlayer.GetComponent <Rigidbody> ().constraints = RigidbodyConstraints.FreezeRotation /*| RigidbodyConstraints.FreezePositionY*/;
		thisPlayer.GetComponent <Rigidbody> ().useGravity = true;
		thisPlayer.GetComponent <Rigidbody> ().mass = 5;
		thisPlayer.GetComponent <Rigidbody> ().AddForce (-thisDirection * 50,ForceMode.Impulse);
		thisPlayer.GetComponent <Health> ().TakeDamage(10,this.gameObject);
	}

	[ClientRpc]
	public void RpcPushPlayers (GameObject thisPlayer, Vector3 thisDirection)
	{
		thisDirection = (transform.position - thisPlayer.transform.position).normalized;
		thisPlayer.GetComponent <Rigidbody> ().constraints = RigidbodyConstraints.None;
		thisPlayer.GetComponent <Rigidbody> ().constraints = RigidbodyConstraints.FreezeRotation /*| RigidbodyConstraints.FreezePositionY*/;
		thisPlayer.GetComponent <Rigidbody> ().useGravity = true;
		thisPlayer.GetComponent <Rigidbody> ().mass = 5;
		thisPlayer.GetComponent <Rigidbody> ().AddForce (-thisDirection * 150,ForceMode.Impulse);
		thisPlayer.GetComponent <Health> ().TakeDamage(10,this.gameObject);
	}

	[ClientRpc]
	public void RpcReproduceAudio(int Number)
	{
		NexusSource.PlayOneShot (Clip.NexusSoms[Number]);
	}
}
