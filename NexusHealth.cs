using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Net;

public class NexusHealth : NetworkBehaviour {

	#region Declaration
	[SyncVar (hook = "OnChangeHealth")]public float CurrentHealth;

	private Animator NexusAnimator;
	private RectTransform HealthBar;
	private Coroutine Suffering;
	private NavMeshAgent NexusAgent;
	private NexusMover Nexus;
	private GameObject[] NexusCristais;
	private NexusKills nKills;

	[Range(0,1)]public int Identify;

	private int _counterRed;
	private int _counterBlue;

	[HideInInspector]
	[SyncVar]public bool isRunning;

	[SyncVar]public bool canBeDamaged;

	private bool Complete;
	private bool _lowLife;
	#endregion

	public void Start()
	{
		HealthBar = Utilities.GetChildren (gameObject, "ForegroundHealth").GetComponent <RectTransform> ();
		NexusAnimator = GetComponent <Animator> ();
		NexusAgent = GetComponent <NavMeshAgent> ();
		nKills = Utilities.FindInHierarchyNetworked ("Manager").GetComponent <NexusKills> ();
		NexusCristais = new GameObject[5];
		Nexus = GetComponent <NexusMover> ();

		for(int i =0; i < 5; i++)
		{
			NexusCristais [i] = Utilities.GetChildren (this.gameObject, "Crystal_0" + (i + 1));
		}

		CurrentHealth = Stats.Health;
		isRunning = false;
		canBeDamaged = false;
		Complete = true;
		_lowLife = false;
		_counterRed = 1;
		_counterBlue = 1;
	}

	public void OnChangeHealth (float Amount) // Sync Health
	{
		CurrentHealth = Amount;
		HealthBar.sizeDelta = new Vector2 (Amount * 2, HealthBar.sizeDelta.y);
	}
		
	public void TakeDamage (float Amount,GameObject From)
	{
		if (!isServer)
			return;

		float Divided = (Amount / 5);

		if(canBeDamaged)
			CurrentHealth -= Divided;

	}

	public void TakeDamageOverTime (float Amount, float Duration, float Rate, GameObject From)
	{
		if (!isServer)
			return;

		if (!canBeDamaged)
			return;
		
		if (!isRunning)
			Suffering = StartCoroutine (OverTime (Amount /5, Duration, Rate));
		else {
			StopCoroutine(Suffering);
			Suffering = StartCoroutine (OverTime (Amount /5, Duration, Rate));
		}
	}

	public void Update ()
	{
		if (!isServer)
			return;

		if(CurrentHealth <= 20 && !_lowLife)
		{
			_lowLife = true;
			NexusAnimator.SetInteger ("HP",2);
			Nexus.VelocidadeInicial = 2;
			Nexus.VelocidadeDeMovimento = 2;
		}

		if (CurrentHealth <= 0) 
		{
			CurrentHealth = 0;
			NexusAnimator.SetTrigger ("Morto");
			NexusAgent.velocity = Vector3.zero;
			NexusAgent.speed = 0;

			if(Identify == 0)
			{
				Debug.Log ("TimeRed Perdeu");
				SendMensageToFinishGame ();
			}
			else
			{
				Debug.Log ("TimeAzul Perdeu");
				SendMensageToFinishGame ();
			}
		}

		if (Identify == 0 && nKills.TimeVermelhoKills >= nKills.necessaryKills)
		{
			if(Complete)
			{
				Complete = false;
				canBeDamaged = true;
				RpcCanBeDamaged (true);
				StartCoroutine (TimeUntillShieldBack (30 * _counterRed));
			}
		}

		if (Identify == 1 && nKills.TimeAzulKills >= nKills.necessaryKills) {

			if(Complete)
			{
				Complete = false;
				canBeDamaged = true;
				RpcCanBeDamaged (true);
				StartCoroutine (TimeUntillShieldBack (30 * _counterBlue));
			}
		}

		if (Identify == 0 && nKills.TimeVermelhoKills > 0) {
			NexusCristais [nKills.TimeVermelhoKills - 1].SetActive (false);
			RpcCristalOff (nKills.TimeVermelhoKills - 1);
		}

		if (Identify == 1 && nKills.TimeAzulKills > 0) {
			NexusCristais [nKills.TimeAzulKills - 1].SetActive (false);
			RpcCristalOff (nKills.TimeAzulKills - 1);
		}
	}

	public void SendMensageToFinishGame()
	{
		//Acha os Players
		//Invoca uma função neles para fechar o game e abrir a tela;
	}

	public IEnumerator OverTime (float ReceiveAmount, float ReceiveDuration, float ReceiveRate)
	{
		float Contador = 0;

		isRunning = true;
		RpcIsSuffering (true);

		while (Contador < ReceiveDuration) 
		{
			CurrentHealth -= ReceiveAmount;

			yield return new WaitForSeconds (ReceiveRate);

			Contador++;
		}

		isRunning = false;
		RpcIsSuffering(false);
	}

	public IEnumerator TimeUntillShieldBack(int AmountTime)
	{
		yield return new WaitForSeconds (AmountTime);

		if(Identify == 0)
		{
			nKills.TimeVermelhoKills = 0;
			canBeDamaged = false;
			_counterRed++;
			RpcCanBeDamaged (false);

			for (int i = 0; i < NexusCristais.Length; i++) {
				NexusCristais [i].SetActive (true);
				RpcCristalOn (i);

			}
			
		}
		else
		{
			nKills.TimeAzulKills = 0;
			canBeDamaged = false;
			_counterBlue++;
			RpcCanBeDamaged (false);

			for (int i = 0; i < NexusCristais.Length; i++) {
				NexusCristais [i].SetActive (true);
				RpcCristalOn (i);
			}
		}

		Complete = true;
	}

	[ClientRpc]
	public void RpcCanBeDamaged(bool isTrue)
	{
		canBeDamaged = isTrue;
	}

	[ClientRpc]
	public void RpcCristalOn(int Which)
	{
		NexusCristais [Which].SetActive (true);
	}

	[ClientRpc]
	public void RpcCristalOff(int Which)
	{
		NexusCristais [Which].SetActive (false);
	}

	[ClientRpc]
	public void RpcIsSuffering(bool isSuffer) //This Player is burning for all.
	{
		isRunning = isSuffer;
	}
		
}
