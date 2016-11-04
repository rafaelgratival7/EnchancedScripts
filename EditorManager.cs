using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class EditorManager : EditorWindow
{
	Component Componente;
	Component[] Componentes;
	GameObject ObjetoFrom;
	GameObject[] ObjetoTo;
	bool isPressed = false;
	bool isComponents = false;

	[MenuItem("GameObject/Utility/Add Multiples Components %&c")]
	public static void InitializerMultiply()
	{
		EditorManager ManagerWindow = (EditorManager)EditorWindow.GetWindow (typeof(EditorManager),false,"Tool Box");
		ManagerWindow.Show ();
		ManagerWindow.ObjetoFrom = null;
		ManagerWindow.ObjetoTo = null;
	}

	void OnGUI () 
	{
		
		if(GUI.Button (new Rect (20, 20, 200, 30), "Copy Component's",EditorStyles.toolbarButton))
		{
			ObjetoFrom = Selection.activeObject as GameObject;

			Componentes = ObjetoFrom.GetComponents (typeof(Component));

			isComponents = true;
		}

		if(GUI.Button(new Rect(240,20,200,30),"Paste Component's",EditorStyles.toolbarButton))
		{
			ObjetoTo = Selection.gameObjects.ToArray ();

			for(int i = 0; i < ObjetoTo.Length;i++)
			{
				for(int a = 0; a < Componentes.Length; a++)
				{
					Componente = Componentes [a];
					Utilities.CopyComponent (ObjetoFrom.GetComponent (Componente.GetType ()),ObjetoTo[i]);
				}
			}

			isPressed = true;
		}

		if(isPressed)
		{
			for(int i = 0; i < ObjetoTo.Length; i++)
			{
				ObjetoTo[i] = (GameObject) EditorGUI.ObjectField(new Rect(270,50 + (20 * i),160,16), ObjetoTo[i], typeof(GameObject),true);
			}
		}

		if(isComponents)
		{
			ObjetoFrom = (GameObject) EditorGUI.ObjectField(new Rect(50,50,160,16), ObjetoFrom, typeof(GameObject),true);

			for (int a = 0; a < Componentes.Length; a++)
			{
				Componentes[a] = (Component) EditorGUI.ObjectField(new Rect(50 , 120 + (20 * a),160,16), Componentes[a], typeof(Component),true);
			}
		}
	}
}