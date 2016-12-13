using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

[NetworkSettings(channel = 3,sendInterval = 0.01F)]
public class Health : NetworkBehaviour
{
	#region Declaration
	[SyncVar (hook = "OnChangeHealth")]public float CurrentHealth;
	[SyncVar (hook = "OnChangePosition")]private Vector3 RespawnPosition;

	[HideInInspector]
	[SyncVar]public bool isRunning;

	private Coroutine Suffering;

	[HideInInspector]
	public List<GameObject> Players = new List<GameObject> ();

	private RectTransform HealthBar;

	[HideInInspector]
	public bool isProcessing;

	public bool isEnemy;

	[Range(10,300)]public int NpcExp;

	[Range(2,10)]public int TimeUntillRespawn;
	#endregion

	public void Awake ()
	{
		HealthBar = Utilities.GetChildren (gameObject, "ForegroundHealth").GetComponent <RectTransform> ();
		CurrentHealth = Stats.Health;
		isProcessing = true;
		isRunning = false;
	}

	public void Start ()
	{
		if (isEnemy)
			return;
		
		TimeUntillRespawn = 10;
		RespawnPosition = new Vector3 (0, 0, 0);
	}

	public void RecoveryHealth (float Amount)
	{
		if (!isServer)
			return;

		CurrentHealth += Amount;
	}
		
	public void TakeDamage (float Amount,GameObject From)
	{
		if (!isServer)
			return;

		PlayersWhoIsAtacking (From);

		CurrentHealth -= Amount;
	}

	public void TakeDamageOverTime (float Amount, float Duration, float Rate, GameObject From)
	{
		if (!isServer)
			return;

		PlayersWhoIsAtacking (From);

		if (!isRunning)
			Suffering = StartCoroutine (OverTime (Amount, Duration, Rate));
		else {
			StopCoroutine(Suffering);
			Suffering = StartCoroutine (OverTime (Amount, Duration, Rate));
		}
	}

	public void PlayersWhoIsAtacking(GameObject Player)
	{
		if(!Players.Contains (Player) && isEnemy)
		{
			Players.Add (Player);
		}
	}

	public void EnemyDead (int Amount)
	{
		GetComponent <Animator>().SetTrigger ("Morto");

		StartCoroutine (DeathTime (2));
	}

	public IEnumerator DeathTime(int Amount)
	{
		yield return new WaitForSeconds (Amount);

		NetworkServer.Destroy (this.gameObject);
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

	public void Update ()
	{
		if (!isServer)
			return;

		if (HealthBar.sizeDelta.x <= 0) {
			Vector2 BarSize = new Vector2 (0, HealthBar.sizeDelta.y);
			HealthBar.sizeDelta = BarSize;
		}

		if (CurrentHealth <= 0 && isProcessing) 
		{
			CurrentHealth = 0;

			if (!isEnemy) 
			{
				RpcDead (this.gameObject);
				StartCoroutine (RespawnTime (TimeUntillRespawn));
				isProcessing = false;
			} 
			else 
			{
				EnemyDead (NpcExp);
			}
		}

		if(CurrentHealth >= Stats.Health)
		{
			CurrentHealth = Stats.Health;
		}
	}

	public void OnChangeHealth (float Amount) // Sync Health
	{
		CurrentHealth = Amount;
		HealthBar.sizeDelta = new Vector2 (Amount * 2, HealthBar.sizeDelta.y);
	}

	public void OnChangePosition (Vector3 Position) // Sync Position
	{
		RespawnPosition = Position;
	}

	public IEnumerator RespawnTime (int Time) // Tempo Até o Respawn
	{
		yield return new WaitForSeconds (Time);
		RpcRespawn (this.gameObject);
		isProcessing = true;
	}

	[ClientRpc]
	public void RpcIsSuffering(bool isSuffer) //This Player is burning for all.
	{
		isRunning = isSuffer;
	}

	[ClientRpc]
	public void RpcDead (GameObject Player)
	{
		Player.GetComponent <MeshRenderer> ().enabled = false; //Player Dead For Others.
	}

	[ClientRpc]
	public void RpcRespawn (GameObject Player)
	{
		GetComponent <MeshRenderer> ().enabled = true;  //Player Respawn For Others.
		Player.transform.position = RespawnPosition;
		CurrentHealth = Stats.Health;
		this.gameObject.GetComponent <Mana> ().CurrentMana = Stats.Mana;
	}

	[ClientRpc]
	public void RpcExperienceDivision(GameObject Player,int Amount)
	{
		if(Players.Count > 1)
			Player.GetComponent <Experience>().SendExperience (Amount / Players.Count);
	}
}
