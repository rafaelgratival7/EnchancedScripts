using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class EventManager : NetworkBehaviour {

	#region Declaração
	public List<GameObject> Players  = new List<GameObject>();
	public GameObject[] ListaDeEventos;
	public GameObject MegaEvento;
	public GameObject Evento;
	public EventSync EventoScript;
	public Vector3 PositionChange;
	public int MaximoDeEventos;
	public int TempoEntreEventos;
	public int Altura;
	public int NumeroDeEventos;
	public int Index;
	public float Distanciamento;
	[SyncVar]public bool Ready;
	#endregion

	public override void OnStartServer()
	{
		Evento = Utilities.GetChildren (this.gameObject, "Spotter");
		EventoScript = Evento.GetComponent <EventSync> ();
		Ready = true;
		Evento.GetComponent <SphereCollider>().isTrigger = true;
		PositionChange = Vector3.zero;
	}

	public void Update()
	{
		if (!isServer)
			return;

		if(Ready && MainSystem.isWorking)
		{
			Ready = false;
			StartCoroutine (SearchAndDestroy ());
		}
	}

	public IEnumerator SearchAndDestroy()
	{
		while(true)
		{
			PositionChange = GetCenter ();
			Evento.transform.position = PositionChange;
			yield return new WaitForSeconds (1);

			if(EventoScript.LocalOcupado.Length == 1)
				Instantiator ();
			else
			{
				while(EventoScript.LocalOcupado.Length > 1)
				{
					PositionChange =  GetPosition () + GetCenter ();
					Evento.transform.position = PositionChange;
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
			RpcTriggerOff (Evento);
			Index = Random.Range (0, ListaDeEventos.Length);
			Vector3 Pos = new Vector3 (0, Altura, 0);
			MegaEvento = Instantiate (ListaDeEventos[Index],Evento.transform.position + Pos, Evento.transform.rotation) as GameObject;
			MegaEvento.transform.SetParent (Utilities.GetChildren (this.gameObject,"Events").transform);
			MegaEvento.name = "Evento_" + NumeroDeEventos;
			NetworkServer.Spawn (MegaEvento);
			NumeroDeEventos++;
		}
	}

	[ClientRpc]
	public void RpcTriggerOff(GameObject Obj)
	{
		Obj.GetComponent <SphereCollider>().isTrigger = false;
	}

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
}
