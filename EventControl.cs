using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

[NetworkSettings(channel = 1,sendInterval = 0.1F)]
public class EventControl : NetworkBehaviour {

	public List<GameObject> Players  = new List<GameObject>();

	public GameObject[] ListaDeEventos;

	private GameObject Evento;
	private GameObject Local;
	private EventSync EventoSync;
	private Vector3 EventoPosition;
	private Vector3 PosFix;

	[HideInInspector]
	public int NumeroDeEventos;

	[Range(1,5)]public int MaximoDeEventos;
	[Range(2,50)]public int TempoEntreEventos;

	private int Altura;
	private int Index;
	private int Counter;

	[Range(10F,100F)]public float Distanciamento;

	private bool Ready;

	public void Start()
	{

		Evento = Utilities.GetChildren (this.gameObject, "Spotter");
		EventoSync = Evento.GetComponent <EventSync> ();
		EventoPosition = Vector3.zero;

		if (!isServer)
			return;


		Ready = true;

	}

	public void Update()
	{
		if (!isServer)
			return;

		if(Ready && MainSystem.isWorking)
		{
			Ready = false;
			StartCoroutine (SearchPlaceToSpawn ());
		}

	}

	public IEnumerator SearchPlaceToSpawn()
	{
		yield return new WaitForSeconds (0.5F);

		while(true)
		{
			if(Players.Count > 1)
			{
				EventoPosition = GetCenter ();
				Evento.transform.position = EventoPosition;
			}

			yield return new WaitForSeconds (1);

			if(EventoSync.LocalOcupado.Length == 0)
			{
				Instantiator ();
			}
			else
			{
				while(EventoSync.LocalOcupado.Length > 0)
				{
					EventoPosition =  GetPosition () + GetCenter ();
					Evento.transform.position = EventoPosition;
					yield return new WaitForSeconds (2);
				}
				Instantiator ();
			}
			yield return new WaitForSeconds (TempoEntreEventos);
		}
	}

	public void Instantiator()
	{
		if(NumeroDeEventos < MaximoDeEventos)
		{
			Index = Random.Range (0, ListaDeEventos.Length);

			switch(Index)
			{
			case 0://Rocha Andantes Spawn em Massa
				
				Counter = 0;
				Altura = 0;

				for(int i = 0; i < 5; i++) 
				{
					int RandomfiX = Random.Range (-5, 5);
					int RandomfiZ = Random.Range (-5, 5);
					PosFix = new Vector3 (RandomfiX, Altura, RandomfiZ);
					Local = (GameObject)Instantiate (ListaDeEventos [0], Evento.transform.position + PosFix, Evento.transform.rotation);
					Local.AddComponent <EventDismiss>();
					Local.transform.SetParent (Utilities.GetChildren (this.gameObject, "Event").transform);
					Local.name = "Rocha Group" + Counter;
					Counter++;
					NetworkServer.Spawn (Local);
				}

				NumeroDeEventos++;
				break;
			case 1:// Aranha Spawn
				Altura = 50;
				PosFix = new Vector3 (0, Altura, 0);
				Local = (GameObject)Instantiate (ListaDeEventos [1], Evento.transform.position + PosFix, Evento.transform.rotation);
				Local.AddComponent <EventDismiss>();
				Local.transform.SetParent (Utilities.GetChildren (this.gameObject, "Event").transform);
				Local.name = "Aranha Single" + NumeroDeEventos;
				NetworkServer.Spawn (Local);
				NumeroDeEventos++;
				break;
			case 2://Morcego spawn
				Altura = 0;

				for(int i = 0; i < 3;i++)
				{
					int RandomfiX = Random.Range (-2, 2);
					int RandomfiZ = Random.Range (-2, 2);
					PosFix = new Vector3 (RandomfiX, Altura, RandomfiZ);
					Local = (GameObject)Instantiate (ListaDeEventos [2], Evento.transform.position + PosFix, Evento.transform.rotation);
					Local.AddComponent <EventDismiss>();
					Local.transform.SetParent (Utilities.GetChildren (this.gameObject, "Event").transform);
					Local.name = "Morcego Single" + NumeroDeEventos;
					NetworkServer.Spawn (Local);
				}

				NumeroDeEventos++;
				break;
			case 3://Life Pool Spawn
				Altura = 0;
				PosFix = new Vector3 (0, Altura, 0);
				Local = (GameObject)Instantiate (ListaDeEventos [3], Evento.transform.position + PosFix, Evento.transform.rotation);
				Local.AddComponent <EventDismiss>();
				Local.transform.SetParent (Utilities.GetChildren (this.gameObject, "Event").transform);
				Local.name = "Life Pool Damage" + NumeroDeEventos;
				NetworkServer.Spawn (Local);
				NumeroDeEventos++;
				break;
			case 4://Mana Pool Spawn
				Altura = 0;
				PosFix = new Vector3 (0, Altura, 0);
				Local = (GameObject)Instantiate (ListaDeEventos [4], Evento.transform.position + PosFix, Evento.transform.rotation);
				Local.AddComponent <EventDismiss>();
				Local.transform.SetParent (Utilities.GetChildren (this.gameObject, "Event").transform);
				Local.name = "Mana Pool Suck" + NumeroDeEventos;
				NetworkServer.Spawn (Local);
				NumeroDeEventos++;
				break;
			case 5://Tentacle Nest
				Altura = 0;

				for(int i = 0; i < 4;i++)
				{
					int RandomfiX = Random.Range (-7, 7);
					int RandomfiZ = Random.Range (-7, 7);
					PosFix = new Vector3 (RandomfiX, Altura, RandomfiZ);
					Local = (GameObject)Instantiate (ListaDeEventos [5], Evento.transform.position + PosFix, Evento.transform.rotation);
					Local.AddComponent <EventDismiss>();
					Local.transform.SetParent (Utilities.GetChildren (this.gameObject, "Event").transform);
					Local.name = "Tentacle Nest" + NumeroDeEventos;
					NetworkServer.Spawn (Local);
				}

				NumeroDeEventos++;
				break;
			case 6://Aranha Nest
				Altura = 0;
				PosFix = new Vector3 (0, Altura, 0);
				Local = (GameObject)Instantiate (ListaDeEventos [6], Evento.transform.position + PosFix, Evento.transform.rotation);
				Local.AddComponent <EventDismiss>();
				Local.transform.SetParent (Utilities.GetChildren (this.gameObject, "Event").transform);
				Local.name = "Aranha Nest" + NumeroDeEventos;
				NetworkServer.Spawn (Local);
				NumeroDeEventos++;
				break;
			}
		}
	}

	#region Matemathics
	Vector3 GetPosition()
	{
		Vector3 Reposicionador = new Vector3(transform.position.x  + Random.Range(-Distanciamento, Distanciamento), transform.position.y + Random.Range(0f, 0f),transform.position.z +  Random.Range(-Distanciamento, Distanciamento));
		return Reposicionador;
	}

	Vector3 GetCenter()
	{
		Players.Clear ();
		Players.AddRange (GameObject.FindGameObjectsWithTag ("Player"));

		Vector3 VetoresDoGrupo = Vector3.zero;
		Vector3 Correção = Vector3.zero;
		Vector3 CentroDoGrupo = Vector3.zero;

		foreach(GameObject Jogadores in Players)
			VetoresDoGrupo += Jogadores.transform.position;

		CentroDoGrupo = VetoresDoGrupo / Players.Count;
		Correção = CentroDoGrupo;
		Correção.y = 0;

		return Correção;
	}
	#endregion
}
