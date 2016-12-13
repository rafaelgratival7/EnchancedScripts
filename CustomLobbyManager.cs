using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class CustomLobbyManager : NetworkLobbyManager
{
	//public int maxPlayers;
	public CustomLobbyManager lobbymanager;

	public List<MatchDesc> roomList = null;

	public bool LAN = false;

	public GameObject serverInfoPrefab;

	public GameObject internetLobbyRoom;

	public override void OnServerConnect(NetworkConnection conn)
	{
		//Setup Unet
		conn.SetChannelOption(Channels.DefaultReliable, ChannelOption.MaxPendingBuffers,500);
		conn.SetChannelOption (Channels.DefaultUnreliable,ChannelOption.MaxPendingBuffers,500);

	}

	public override void OnMatchCreate (CreateMatchResponse matchInfo)
	{
		if (matchInfo.success) {
			if (LAN) {
				StartHost (new MatchInfo (matchInfo));
			}
			//matchSize = 6;
		}
		base.OnMatchCreate (matchInfo);
	}

	//Controlar quando todos os jogadores estão conectados, e ativar seleção de personagem
	public override void OnLobbyServerPlayersReady () // All players ready
	{       
		StartCoroutine ("LoadGame");
	}

	public IEnumerator LoadGame ()
	{
		yield return new WaitForSeconds (3);  
		this.ServerChangeScene (playScene);
	}

	public override GameObject OnLobbyServerCreateLobbyPlayer (NetworkConnection conn, short playerControllerId)
	{
		GameObject obj = Instantiate (lobbyPlayerPrefab.gameObject) as GameObject;
		obj.transform.SetParent (GameObject.Find ("LobbyRoom").transform, false);
		obj.transform.localScale = new Vector3 (1, 1, 1);        
		return obj;        
	}

	public override bool OnLobbyServerSceneLoadedForPlayer (GameObject lobbyPlayer, GameObject gamePlayer) // Server carregou cena
	{		
		return base.OnLobbyServerSceneLoadedForPlayer (lobbyPlayer, gamePlayer);			
	}

	public override void OnLobbyClientSceneChanged (NetworkConnection conn) // Cliente carregou cena
	{		
		base.OnLobbyClientSceneChanged (conn);
	}

	// Script youtube
	public void StartupHost ()
	{
		LAN = true;
		SetPort ();
		NetworkLobbyManager.singleton.StartHost ();
	}

	public void JoinGame ()
	{
		SetIPAddress ();
		SetPort ();
		NetworkLobbyManager.singleton.StartClient ();
	}

	void SetIPAddress ()
	{
		string ipAddress = GameObject.Find ("InputFieldIPAddress").transform.FindChild ("Text").GetComponent<Text> ().text;
		NetworkLobbyManager.singleton.networkAddress = ipAddress;
	}

	void SetPort ()
	{
		NetworkLobbyManager.singleton.networkPort = 7777;        
	}

	// Script Projeto Nave para servidor dedicado

	public void CreateMatchMakingGame ()
	{
		LAN = false;
		CustomLobbyManager.singleton.StartMatchMaker ();
		CustomLobbyManager.singleton.matchMaker.CreateMatch (Config.PlayerData.PlayerName, (uint)matchSize, true, "", NetworkLobbyManager.singleton.OnMatchCreate);
		//Variaveis custom de identificacao

		//NetworkLobbyManager.singleton.backDelegate = NetworkLobbyManager.singleton.StopHost;
		//NetworkLobbyManager.singleton._isMatchmaking = true;
		//NetworkLobbyManager.singleton.DisplayIsConnecting();
		//NetworkLobbyManager.singleton.SetServerInfo("Matchmaker Host", NetworkLobbyManager.singleton.matchHost);
	}

	public void OpenServerList ()
	{
		CustomLobbyManager.singleton.StartMatchMaker ();
		ListMatchs ();             
		//NetworkLobbyManager.singleton.backDelegate = lobbyManager.SimpleBackClbk;
		//NetworkLobbyManager.singleton.ChangeTo(lobbyServerList);
	}


	public void OnJoinInternetMatch (JoinMatchResponse matchJoin)
	{
		if (matchJoin.success) {
			//Debug.Log("Able to join a match");
			MatchInfo hostInfo = new MatchInfo (matchJoin);
			NetworkManager.singleton.StartClient (hostInfo);
			internetLobbyRoom.SetActive (true);
			GameObject.Find ("ServerList").SetActive (false);
		} else {
			Debug.LogError ("Join match failed");
		}
	}

	public void OnConnected (NetworkMessage msg)
	{
		Debug.Log ("Connected!");
	}

	public void OnMatchList (ListMatchResponse matchListResponse)
	{
		if (matchListResponse.success && matchListResponse.matches != null) {
			if (matchListResponse.matches.Count == 0) {
				Debug.Log ("Nenhum servidor encontrado!");
			}

			for (int i = 0; i < matchListResponse.matches.Count; ++i) {
				GameObject obj = Instantiate (serverInfoPrefab) as GameObject;
				obj.transform.SetParent (GameObject.Find ("ServerList").transform, false);
				obj.GetComponent<ServerInfo> ().Populate (matchListResponse.matches [i], lobbymanager);
				//o.transform.SetParent(serverListRect, false);
				print ("Servidor encontrado!");
			}
			//ShowServers(matchListResponse);
		}
	}

	public void ShowServers (ListMatchResponse response)
	{
		if (response.matches.Count == 0) {
			Debug.Log ("Nenhum servidor encontrado!");
		}

		for (int i = 0; i < response.matches.Count; ++i) {
			//GameObject obj = Instantiate(serverInfoPrefab) as GameObject;
			//obj.transform.SetParent(GameObject.Find("ServerList").transform, false);
			//o.transform.SetParent(serverListRect, false);
			GameObject p = Instantiate (null) as GameObject;
			print ("Servidor encontrado!");
		}
	}
	// JoinMatch - Verificar LobbyServerEntry(estatisticas do server)
	// Estudar LobbyServerList para melhorar a lista de servers


	// Script Dedicated
	/*
    public void OnMatchList(ListMatchResponse matchList)
    {
        if (matchList == null)
        {
            Debug.Log("null Match List returned from server");
            return;
        }

        roomList = new List<MatchDesc>();
        roomList.Clear();
        foreach (MatchDesc match in matchList.matches)
        {
            roomList.Add(match);
        }
    }

    */

	public void StartMatchMaking ()
	{
		CustomLobbyManager.singleton.StartMatchMaker ();
	}

	public void CreateMatch ()
	{
		string roomname = "BOS" + Mathf.RoundToInt (Random.Range (0, 1000));
		NetworkManager.singleton.matchMaker.CreateMatch (roomname, 10, true, "", NetworkManager.singleton.OnMatchCreate);
		SetPort ();
		NetworkManager.singleton.StartHost ();
	}

	public void ListMatchs ()
	{
		CustomLobbyManager.singleton.matchMaker.ListMatches (0, 10, "", OnMatchList);
	}

	// Para fazer ainda(identificar o host selecionado e entrar)
	/*
    public void JoinMatch() // Essa parte vai apenas "Avisar" que você está ocupando um lugar no servidor de Matchmaking e NÃO NO HOST
    {
        // Atualizar informações de servidores disponíveis
        foreach(var info in roomList)
        {
            CustomLobbyManager.singleton.matchMaker.JoinMatch(info.networkId, "", CustomLobbyManager.singleton.OnMatchJoined); // No script isso é aplicado no GUI(???) Testar em UI para ver como fica
        }
    }*/

	public void EntrarSala ()
	{
		CustomLobbyManager.singleton.StartClient (); // Esse efetivamente entra no servidor/host
	}
}
