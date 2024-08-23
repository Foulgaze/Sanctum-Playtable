using System;
using System.Collections.Generic;
using System.Linq;
using Sanctum_Core;
using TMPro;
using UnityEngine;

public class LobbyFrontend : MonoBehaviour 
{
	
	[SerializeField]
	private Transform lobbyConnectMenuScreen;
	[SerializeField]
	private Transform lobbyLoadingScreen;
	[SerializeField]
	private Transform lobbyMenuScreen;
	[SerializeField]
	private LobbyConnectMenu lobbyConnectMenu;

	void Start()
	{
		this.ChangeScreens(this.lobbyConnectMenuScreen);
		LobbyConnection lobby = GameOrchestrator.Instance.serverListener.lobby;
		this.lobbyConnectMenu.triedToJoinOrCreateLobby += this.OnChangeToLoadingScreen;
		GameOrchestrator.Instance.serverListener.lobbyCreatedOrJoined += this.OnChangeToLobbyMenu;
		GameOrchestrator.Instance.playtableGameStarted += this.OnChangeToGameStart;
		GameOrchestrator.Instance.serverListener.problemConnectingToServer += (message) => this.ChangeScreens(this.lobbyConnectMenuScreen);
		GameOrchestrator.Instance.playtableCreated += OnPlaytableCreated;
	}

	private void OnPlaytableCreated(Playtable table)
	{
		table.GameStarted.nonNetworkChange += (_) => ChangeScreens(null);
	}


	private void ChangeScreens(Transform? screenToChangeTo = null)
	{
		foreach(Transform screen in new List<Transform>{lobbyConnectMenuScreen, lobbyLoadingScreen, lobbyMenuScreen})
		{
			if(screen == screenToChangeTo)
			{
				screen.gameObject.SetActive(true);
				continue;
			}
			screen.gameObject.SetActive(false);
		}
	}
	private void OnChangeToLoadingScreen()
	{
		this.ChangeScreens(this.lobbyLoadingScreen);
	}

	private void OnChangeToLobbyMenu()
	{
		this.ChangeScreens(this.lobbyMenuScreen);
	}

	private void OnChangeToGameStart()
	{
		this.ChangeScreens();
	}


	
	
}