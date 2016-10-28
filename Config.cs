using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class Config : MonoBehaviour
{
	public GameObject Logo;
	public GameObject PainelObjects;
	public GameObject NameTab;
	public GameObject GoldTab;
	public GameObject CashTab;
	public Text PlayerInputName;
	public Text PlayerToSeeName;

	
	[Serializable]
	public class PlayerData
	{
		public static string PlayerName;
		public static int PlayerMoney;
		public static float PlayerCash;
		public static int PlayerLevel;
		public static float PlayerExperience;
	}

	[Serializable]
	public class PlayerTeam
	{
		public static int Vermelho = 0;
		public static int Azul = 1;
	}

	[Serializable]
	public class CharacterSelected
	{
		public enum Persona
		{
			Coruja,
			Lobo,
			Iguana,
			Touro,
			Tigre,
			Macaco,
			Rinoceronte,
			Besouro
		};
	}

	[Serializable]
	public class SkinUnlocked
	{
		//Coruja
		public static int Coruja_1 = PlayerPrefs.GetInt ("Coruja_Skin_1");
		public static int Coruja_2 = PlayerPrefs.GetInt ("Coruja_Skin_2");
		//Lobo
		public static int Lobo_1 = PlayerPrefs.GetInt ("Lobo_Skin_1");
		public static int Lobo_2 = PlayerPrefs.GetInt ("Lobo_Skin_2");
		//Iguana
		public static int Iguana_1 = PlayerPrefs.GetInt ("Iguana_Skin_1");
		public static int Iguana_2 = PlayerPrefs.GetInt ("Iguana_Skin_2");
		//Touro
		public static int Touro_1 = PlayerPrefs.GetInt ("Touro_Skin_1");
		public static int Touro_2 = PlayerPrefs.GetInt ("Touro_Skin_2");
		//Tigre
		public static int Tigre_1 = PlayerPrefs.GetInt ("Tigre_Skin_1");
		public static int Tigre_2 = PlayerPrefs.GetInt ("Tigre_Skin_2");
		//Macaco
		public static int Macaco_1 = PlayerPrefs.GetInt ("Macaco_Skin_1");
		public static int Macaco_2 = PlayerPrefs.GetInt ("Macaco_Skin_2");
		//Rinoceronte
		public static int Rinoceronte_1 = PlayerPrefs.GetInt ("Rinoceronte_Skin_1");
		public static int Rinoceronte_2 = PlayerPrefs.GetInt ("Rinoceronte_Skin_2");
		//Besouro
		public static int Besouro_1 = PlayerPrefs.GetInt ("Besouro_Skin_1");
		public static int Besouro_2 = PlayerPrefs.GetInt ("Besouro_Skin_2");
	}

	[Serializable]
	public class SerialData
	{
		public static string[] Serials = PlayerPrefsX.GetStringArray ("SerialArray");
	}

	[Serializable]
	public class ListData
	{
		public static string[] ListOfNames;
	}

	public void SetupPlayerName ()
	{
		PlayerData.PlayerName = PlayerInputName.text;
		PlayerToSeeName.text = PlayerData.PlayerName;
		SetupPlayerIdentify ();
	}

	public static void SetupSerialIdentify()
	{
		PlayerPrefsX.SetStringArray ("SerialArray",SerialData.Serials);
	}

	public static void SetupPlayerIdentify ()
	{
		PlayerPrefs.SetString ("NameIdentify", PlayerData.PlayerName);
		PlayerPrefs.SetInt  ("MoneyIdentify",PlayerData.PlayerMoney);
		PlayerPrefs.SetInt ("LevelIdentify",PlayerData.PlayerLevel);
		PlayerPrefs.SetFloat ("CashIdentify",PlayerData.PlayerCash);
		PlayerPrefs.SetFloat ("ExperienceIdentify",PlayerData.PlayerExperience);
		PlayerPrefs.Save ();
	}

	public static void LoadPlayerIdentify ()
	{
		PlayerData.PlayerName = PlayerPrefs.GetString ("NameIdentify");
		PlayerData.PlayerMoney = PlayerPrefs.GetInt ("MoneyIdentify");
		PlayerData.PlayerCash = PlayerPrefs.GetFloat ("CashIdentify");
		PlayerData.PlayerLevel = PlayerPrefs.GetInt ("LevelIdentify");
		PlayerData.PlayerExperience = PlayerPrefs.GetFloat ("ExperienceIdentify");
		SerialData.Serials = PlayerPrefsX.GetStringArray ("SerialArray");
	}

	public void LogoutPlayerIdenitfy()
	{
		SetupPlayerIdentify ();
		NameTab.GetComponent <Text>().text = "See you soon";
	}

	public void Start()
	{
		LoadPlayerIdentify ();

		if(PlayerData.PlayerName.Length > 2)
		{
			Logo.GetComponent <FancyNext>().Target = PainelObjects;
			NameTab.SetActive (true);
			CashTab.SetActive (true);
			GoldTab.SetActive (true);
			NameTab.GetComponent <Text>().text = PlayerData.PlayerName;
			CashTab.GetComponent <Text> ().text = PlayerData.PlayerCash.ToString ();
			GoldTab.GetComponent <Text>().text = PlayerData.PlayerMoney.ToString ();
		}
	}

	public void Update()
	{
		//Coruja
		SkinUnlocked.Coruja_1 = PlayerPrefs.GetInt ("Coruja_Skin_1");
		SkinUnlocked.Coruja_2 = PlayerPrefs.GetInt ("Coruja_Skin_2");
		//Lobo
		SkinUnlocked.Lobo_1 = PlayerPrefs.GetInt ("Lobo_Skin_1");
		SkinUnlocked.Lobo_2 = PlayerPrefs.GetInt ("Lobo_Skin_2");
		//Iguana
		SkinUnlocked.Iguana_1 = PlayerPrefs.GetInt ("Iguana_Skin_1");
		SkinUnlocked.Iguana_2 = PlayerPrefs.GetInt ("Iguana_Skin_2");
		//Touro
		SkinUnlocked.Touro_1 = PlayerPrefs.GetInt ("Touro_Skin_1");
		SkinUnlocked.Touro_2 = PlayerPrefs.GetInt ("Touro_Skin_2");
		//Tigre
		SkinUnlocked.Tigre_1 = PlayerPrefs.GetInt ("Tigre_Skin_1");
		SkinUnlocked.Tigre_2 = PlayerPrefs.GetInt ("Tigre_Skin_2");
		//Macaco
		SkinUnlocked.Macaco_1 = PlayerPrefs.GetInt ("Macaco_Skin_1");
		SkinUnlocked.Macaco_2 = PlayerPrefs.GetInt ("Macaco_Skin_2");
		//Rinoceronte
		SkinUnlocked.Rinoceronte_1 = PlayerPrefs.GetInt ("Rinoceronte_Skin_1");
		SkinUnlocked.Rinoceronte_2 = PlayerPrefs.GetInt ("Rinoceronte_Skin_2");
		//Besouro
		SkinUnlocked.Besouro_1 = PlayerPrefs.GetInt ("Besouro_Skin_1");
		SkinUnlocked.Besouro_2 = PlayerPrefs.GetInt ("Besouro_Skin_2");

		if(PlayerData.PlayerExperience >= 300)
		{
			PlayerData.PlayerLevel += 1;
			PlayerData.PlayerExperience = 0;
			SetupPlayerIdentify ();
		}
	}
}
