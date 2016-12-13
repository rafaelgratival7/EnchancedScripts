using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NavLarvaManager : MonoBehaviour 
{
	#region Declaração;
	public GameObject Larva;

	private SoundManager LarvaSummon;

	private EffectManager Efeito;

	[HideInInspector]
	public GameObject[] Larvas;

	private GameObject[] LarvasSpawnPosition;

	private List<GameObject> PontosDeSpawn = new List<GameObject>();

	[HideInInspector]
	public List<int> Numerais = new List<int> ();

	private int Index;
	private int Contador = 0;

	private int MaximoDeMinions = 6;
	private int MinimoDeMinions = 0;


	private float Espera = 0.0F;
	[Range(0.01F,1F)]public float TempoDeEspera;

	public bool Comando = false; // Ta aqui só para acessar mais fácil só torna true 1 vez e pronto acessa o Comando;
	#endregion

	#region Parte-1 Iniciador
	void Awake()
	{
		LarvaSummon = GameObject.Find ("SoundSystem").GetComponent<SoundManager> ();
		Efeito = GameObject.Find ("EffectSystem").GetComponent<EffectManager> ();

		MaximoDeMinions = 6;
		MinimoDeMinions = 0;
	}

	void Start () 
	{
		Procurador ();
	}

	void Procurador()
	{
		foreach (Transform Child in this.transform)
		{
			PontosDeSpawn.Add (Child.gameObject);
		}

		LarvasSpawnPosition = new GameObject[PontosDeSpawn.Count];
		Larvas = new GameObject[PontosDeSpawn.Count];

		PontosDeSpawn.CopyTo (LarvasSpawnPosition);
	}

	void Update()
	{
		LarvaIniciadora ();

		if(Input.GetKey(KeyCode.A) && Time.time > Espera)
		{
			Espera = Time.time + TempoDeEspera;

			if(Contador < MaximoDeMinions)
			{
				Contador++;
			}
		}

		if(Input.GetKey(KeyCode.D) && Time.time > Espera)
		{
			Espera = Time.time + TempoDeEspera;

			if(Contador > MinimoDeMinions)
			{
				Contador--;
				Numerais.Remove (Contador+1);
				Destroy (Larvas [Contador]);
			}
		}
	}
	#endregion

	#region Parte-2 Matematica de Soma e Subtração
	void LarvaIniciadora()
	{
		switch(Contador)
		{
		case 0: 
			
			Numerais.Clear ();
			
			break;
		case 1:
			
			if(!Numerais.Contains(Contador))
			{
				Numerais.Add (Contador);

				Larvas [Contador - 1] = Instantiate (Larva, PontosDeSpawn [Contador - 1].transform.position, PontosDeSpawn [Contador - 1].transform.rotation) as GameObject;
				Larvas [Contador - 1].transform.parent = Utilities.GetChildren(transform,"P1");
				Larvas [Contador - 1].name = "Larva_" + (Contador - 1);

				Utilities.PlayEffectAt (Efeito.Efeitos [0],PontosDeSpawn [Contador - 1].transform.position, PontosDeSpawn [Contador - 1].transform.rotation, 5f);

				AudioSource.PlayClipAtPoint (LarvaSummon.NPC_Larva [0],PontosDeSpawn [Contador - 1].transform.position);
			}

			break;
		case 2:

			if(!Numerais.Contains(Contador))
			{
				Numerais.Add (Contador);

				Larvas [Contador - 1] = Instantiate (Larva, PontosDeSpawn [Contador - 1].transform.position, PontosDeSpawn [Contador - 1].transform.rotation) as GameObject;
				Larvas [Contador - 1].transform.parent = Utilities.GetChildren(transform,"P2");
				Larvas [Contador - 1].name = "Larva_" + (Contador - 1);

				Utilities.PlayEffectAt (Efeito.Efeitos [0],PontosDeSpawn [Contador - 1].transform.position, PontosDeSpawn [Contador - 1].transform.rotation, 5f);

				AudioSource.PlayClipAtPoint (LarvaSummon.NPC_Larva [0],PontosDeSpawn [Contador - 1].transform.position);
			}
	
			break;
		case 3:
			
			if(!Numerais.Contains(Contador))
			{
				Numerais.Add (Contador);

				Larvas [Contador - 1] = Instantiate (Larva, PontosDeSpawn [Contador - 1].transform.position, PontosDeSpawn [Contador - 1].transform.rotation) as GameObject;
				Larvas [Contador - 1].transform.parent = Utilities.GetChildren(transform,"P3");
				Larvas [Contador - 1].name = "Larva_" + (Contador - 1);

				Utilities.PlayEffectAt (Efeito.Efeitos [0],PontosDeSpawn [Contador - 1].transform.position, PontosDeSpawn [Contador - 1].transform.rotation, 5f);

				AudioSource.PlayClipAtPoint (LarvaSummon.NPC_Larva [0],PontosDeSpawn [Contador - 1].transform.position);
			}

			break;
		case 4:

			if(!Numerais.Contains(Contador))
			{
				Numerais.Add (Contador);

				Larvas [Contador - 1] = Instantiate (Larva, PontosDeSpawn [Contador - 1].transform.position, PontosDeSpawn [Contador - 1].transform.rotation) as GameObject;
				Larvas [Contador - 1].transform.parent = Utilities.GetChildren(transform,"P4");
				Larvas [Contador - 1].name = "Larva_" + (Contador - 1);

				Utilities.PlayEffectAt (Efeito.Efeitos [0],PontosDeSpawn [Contador - 1].transform.position, PontosDeSpawn [Contador - 1].transform.rotation, 5f);

				AudioSource.PlayClipAtPoint (LarvaSummon.NPC_Larva [0],PontosDeSpawn [Contador - 1].transform.position);
			}

			break;
		case 5:

			if(!Numerais.Contains(Contador))
			{
				Numerais.Add (Contador);

				Larvas [Contador - 1] = Instantiate (Larva, PontosDeSpawn [Contador - 1].transform.position, PontosDeSpawn [Contador - 1].transform.rotation) as GameObject;
				Larvas [Contador - 1].transform.parent = Utilities.GetChildren(transform,"P5");
				Larvas [Contador - 1].name = "Larva_" + (Contador - 1);

				Utilities.PlayEffectAt (Efeito.Efeitos [0],PontosDeSpawn [Contador - 1].transform.position, PontosDeSpawn [Contador - 1].transform.rotation, 5f);

				AudioSource.PlayClipAtPoint (LarvaSummon.NPC_Larva [0],PontosDeSpawn [Contador - 1].transform.position);
			}

			break;
		case 6:

			if(!Numerais.Contains(Contador))
			{
				Numerais.Add (Contador);

				Larvas [Contador - 1] = Instantiate (Larva, PontosDeSpawn [Contador - 1].transform.position, PontosDeSpawn [Contador - 1].transform.rotation) as GameObject;
				Larvas [Contador - 1].transform.parent = Utilities.GetChildren(transform,"P6");
				Larvas [Contador - 1].name = "Larva_" + (Contador - 1);

				Utilities.PlayEffectAt (Efeito.Efeitos [0],PontosDeSpawn [Contador - 1].transform.position, PontosDeSpawn [Contador - 1].transform.rotation, 5f);

				AudioSource.PlayClipAtPoint (LarvaSummon.NPC_Larva [0],PontosDeSpawn [Contador - 1].transform.position);
			}

			break;
		}
	}
	#endregion
}
