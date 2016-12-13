using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class EventChecker : NetworkBehaviour {

	#region Declaração
	[HideInInspector]
	public Collider[] LocalOcupado;

	[Header("Configuração")]
	[Tooltip("Adicionar LayerMask que vão ser detectadas pelo evento.")]
	public LayerMask Mask;

	private float Raio;

	[SyncVar(hook = "OnPositionChanger")]
	private Vector3 myTransform;
	#endregion

	#region Controle
	void Awake()
	{
		Raio = GetComponent<SphereCollider> ().radius;
	}

	void Update()
	{
		LocalOcupado = Physics.OverlapSphere (transform.position,Raio,Mask);

		myTransform = transform.position;
	}
		
	void OnPositionChanger(Vector3 Pos)
	{
		myTransform = Pos;
	}
	#endregion
}
