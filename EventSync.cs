using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[NetworkSettings(channel = 2,sendInterval = 0.1F)]
public class EventSync : NetworkBehaviour {

	#region Declaração
	public Collider[] LocalOcupado;
	public LayerMask MaskToGet;
	private float Raio;
	#endregion

	public void Start()
	{
		if (!isServer)
			return;

		Raio = GetComponent<SphereCollider> ().radius;
	}

	public void Update()
	{
		if (!isServer)
			return;

		LocalOcupado = Physics.OverlapSphere (transform.position,Raio,MaskToGet);
	}
}
