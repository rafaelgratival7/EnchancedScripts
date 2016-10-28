using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;

public class FaceScript : MonoBehaviour {

    #region Declaração

    #endregion

	void Update()
	{
		transform.LookAt (transform.position + Camera.main.transform.rotation * Vector3.back, Camera.main.transform.rotation * Vector3.down);
	}
}
