using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

 public class CameraLmanager : MonoBehaviour {

    #region Declaração
	public Camera[] ListaDeCamaras;
	private int Contador;
    #endregion

	void Start()
	{
		for(int i = 0; i < ListaDeCamaras.Length;i++)
		{
			if(i == 0)
			{
				ListaDeCamaras [i].enabled = true;
			}
			else
			{
				ListaDeCamaras [i].enabled = false;
			}
		}
	}

	void Update()
    {
		if(Input.GetKeyDown(KeyCode.Q))
		{

			if (Contador > 0) 
			{
				Contador--;
				ListaDeCamaras [Contador].enabled = true;
				ListaDeCamaras [Contador + 1].enabled = false;
			} 
		}

		if(Input.GetKeyDown(KeyCode.E))
		{

			if (Contador < ListaDeCamaras.Length - 1) 
			{
				Contador++;
				ListaDeCamaras [Contador].enabled = true;
			} 
		}
    }
}

[CustomEditor(typeof(CameraLmanager))]
public class CameraEditor : Editor{

	public override void OnInspectorGUI()
	{
		EditorGUILayout.HelpBox ("Apertar Q para ultima Camera , Apertar E para proxima Camera, Lembrar de ativar todas as Cameras", MessageType.Warning);
	}
}
