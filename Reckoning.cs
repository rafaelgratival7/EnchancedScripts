using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Reckoning : NetworkBehaviour {

	[SyncVar] private Vector3 syncPos;
	[SyncVar] private float syncYRot;
	[SyncVar] private float syncXRot;
	[SyncVar] private float syncZRot;

	private Vector3 lastPos;
	private Quaternion lastRot;
	private Transform myTransform;
	private float lerpRate = 10;
	private float posThreshold = 0.01F;
	private float rotThreshold = 0.01F;

	public void Start()
	{
		myTransform = transform;
	}

	public void Update()
	{
		if (!isServer)
			return;

		TransmiteMotion ();
		LerpMotion ();
	}


	public void TransmiteMotion()
	{
		if(Vector3.Distance (myTransform.position,lastPos)> posThreshold || Quaternion.Angle (myTransform.rotation,lastRot) > rotThreshold)
		{
			lastPos = myTransform.position;
			lastRot = myTransform.rotation;

			syncPos = myTransform.position;
			syncYRot = myTransform.localEulerAngles.y;
			syncXRot = myTransform.localEulerAngles.x;
			syncZRot = myTransform.localEulerAngles.z;
		}
	}
	public void LerpMotion()
	{
		myTransform.position = Vector3.Lerp (myTransform.position, syncPos, Time.deltaTime * lerpRate);
		Vector3 newRot = new Vector3 (syncXRot, syncYRot, syncZRot);
		myTransform.rotation = Quaternion.Lerp (myTransform.rotation, Quaternion.Euler (newRot), Time.deltaTime * lerpRate);
	}
}
