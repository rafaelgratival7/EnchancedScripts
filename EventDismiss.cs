using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[NetworkSettings(channel = 2,sendInterval = 0.1F)]
public class EventDismiss : NetworkBehaviour{

	[SyncVar]private float TimeToBeDestroyed;
	private EventControl Event;
	private float Timer;
	public int WillBeDestroyedAt;

	public void Start () 
	{
		Event = (EventControl)GameObject.FindObjectOfType (typeof(EventControl));

		if (!isServer)
			return;
		
		TimeToBeDestroyed = 240F;
		Timer = TimeToBeDestroyed;
		WillBeDestroyedAt = (int)TimeToBeDestroyed;
		StartCoroutine(DestinoFinal (TimeToBeDestroyed));
	}

	public IEnumerator DestinoFinal(float Amount)
	{
		yield return new WaitForSeconds (Amount);
		Event.NumeroDeEventos--;
		NetworkServer.Destroy (this.gameObject);
	}

	public void Update()
	{
		if (!isServer)
			return;
		Timer -= Time.deltaTime;
		WillBeDestroyedAt = (int)Timer;
	}
}
