using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Linq;
using System;
using UnityEngine.Networking;


/// <summary>
/// Interface of Utilities.
/// </summary>
public class Utilities : NetworkBehaviour
{
	#region Functions
	/// <summary>
	/// Play the Clip, at Position, with Volume.
	/// Destroyed after clip lengh.
	/// </summary>
	/// <returns>The <see cref="UnityEngine.AudioSource"/>.</returns>
	/// <param name="Clip">Clip to be taken.</param>
	/// <param name="Position">Position to be instantiated.</param>
	/// <param name="Volume">Volume to be played.</param>
	public static AudioSource PlayClipAt(AudioClip Clip, Vector3 Position, float Volume)
	{
		GameObject Audio_Temporario = new GameObject("Audio_Temporario");

		Audio_Temporario.transform.position = Position;

		AudioSource aSource = Audio_Temporario.AddComponent<AudioSource>(); 

		aSource.clip = Clip;
		aSource.minDistance = 10;
		aSource.maxDistance = 500;
		aSource.volume = Volume;
		aSource.Play ();

		Destroy(Audio_Temporario, Clip.length);

		return aSource;
	}

	/// <summary>
	/// Play the Effect, at Position, at Rotation , and Destroy at Time.
	/// </summary>
	/// <returns>The <see cref="UnityEngine.GameObject"/>.</returns>
	/// <param name="Effect">Effect to be taken.</param>
	/// <param name="Position">Position to be instantiated.</param>
	/// <param name="Rotation">Rotation to be instantiated.</param>
	/// <param name="DestroyTime">Time to be destroyed after born.</param>
	public static GameObject PlayEffectAt(GameObject Effect, Vector3 Position, Quaternion Rotation ,float DestroyTime)
	{
		GameObject Efeito_Temporario = Instantiate (Effect, Position, Rotation) as GameObject;
		Efeito_Temporario.name = "Efeito_Temporario";

		Destroy (Efeito_Temporario, DestroyTime);

		return Efeito_Temporario;
	}

	/// <summary>
	/// Gets the children.
	/// </summary>
	/// <returns>The children.</returns>
	/// <param name="obj"> Object that contains children.</param>
	/// <param name="Name"> Name of the children to be found.</param>
	static public GameObject GetChildren(GameObject obj, string Name) 
	{
		Transform[] Childs = obj.transform.GetComponentsInChildren<Transform>(true);

		foreach (Transform Child in Childs)
		{
			if (Child.gameObject.name == Name) 
			{
				return Child.gameObject;
			}
		}
		return null;
	}

	/// <summary>
	/// Gets the children.
	/// </summary>
	/// <returns>The children.</returns>
	/// <param name="obj"> Transform that contains children</param>
	/// <param name="Name"> Name of the children to be found.</param>
	static public Transform GetChildren(Transform obj, string Name) 
	{
		Transform[] Childs = obj.transform.GetComponentsInChildren<Transform>(true);

		foreach (Transform Child in Childs)
		{
			if (Child.name == Name) 
			{
				return Child;
			}
		}
		return null;
	}

	/// <summary>
	/// Gets the father.
	/// </summary>
	/// <returns>The father.</returns>
	/// <param name="Obj"> Transform that contains father.</param>
	/// <param name="Name"> Name of the father to be found.</param>
	static public Transform GetFather(Transform Obj, string Name) 
	{
		Transform Father = Obj.transform.root;

		if(Father.name == Name)
		{
			return Father;
		}
		else
		{
			Transform[] Uncles = Father.GetComponentsInChildren<Transform> (true);

			foreach (Transform Child in Uncles)
			{
				if (Child.name == Name) 
				{
					return Child;
				}
			}
		}
		return null;
	}

	/// <summary>
	/// Gets the father.
	/// </summary>
	/// <returns>The father.</returns>
	/// <param name="Obj"> GameObject that contains father.</param>
	/// <param name="Name"> Name of the father to be found.</param>
	static public GameObject GetFather(GameObject Obj, string Name) 
	{
		Transform Father = Obj.transform.root;

		if(Father.name == Name)
		{
			return Father.gameObject;
		}
		else
		{
			Transform[] Uncles = Father.GetComponentsInChildren<Transform> (true);

			foreach (Transform Child in Uncles)
			{
				if (Child.name == Name) 
				{
					return Child.gameObject;
				}
			}
		}
		return null;
	}

	/// <summary>
	/// Gets the father.
	/// </summary>
	/// <returns>The father.</returns>
	/// <param name="Obj"> Get's the first Transform father of hierarchy.</param>
	static public Transform GetFather(Transform Obj) 
	{
		Transform Father = Obj.transform.root;

		return Father;
	}

	/// <summary>
	/// Gets the father.
	/// </summary>
	/// <returns>The father.</returns>
	/// <param name="Obj"> Get's the first GameObject father of hierarchy</param>
	static public GameObject GetFather(GameObject Obj) 
	{
		Transform Father = Obj.transform.root;

		return Father.gameObject;
	}

	/// <summary>
	/// Find object not attach to script in hierarchy.
	/// </summary>
	/// <returns>The in hierarchy.</returns>
	/// <param name="Name">Name of object to be found on hierarchy.</param>
	static public GameObject FindInHierarchy(string Name)
	{
		GameObject Finder = GameObject.Find (Name);

		return Finder;
	}

	/// <summary>
	/// Copy Component's
	/// </summary>
	/// <returns>Copy selected component's.</returns>
	/// <param name="Componente">Component.</param>
	/// <param name="Destiny">Game object to Paste.</param>
	/// <typeparam name="T">Object.GetComponent(Typeof).</typeparam>
	static public T CopyComponent<T>(T Componente, GameObject Destiny) where T : Component
	{
		System.Type type = Componente.GetType();

		Component Copy = Destiny.AddComponent(type);

		System.Reflection.FieldInfo[] fields = type.GetFields();

		foreach (System.Reflection.FieldInfo field in fields)
		{
			field.SetValue(Copy, field.GetValue(Componente));
		}

		return Copy as T;
	}

	/// <summary>
	/// Gets from net I.
	/// </summary>
	/// <returns>The from net I.</returns>
	/// <param name="netID">Net I.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static T GetFromNetID<T>(NetworkInstanceId netID) where T : Component
	{
		if (Network.isServer)
			return NetworkServer.FindLocalObject(netID).GetComponent<T>();
		else
			return ClientScene.FindLocalObject(netID).GetComponent<T>();
	}
	#endregion
}
