using System;
using System.Collections.Generic;
using System.Linq;
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
		ChangeScreens(this.lobbyConnectMenuScreen);
		Lobby lobby = GameOrchestrator.Instance.serverListener.lobby;
		this.lobbyConnectMenu.triedToJoinOrCreateLobby += this.OnChangeToLoadingScreen;
		lobby.lobbyCreatedOrJoined += this.OnChangeToLobbyMenu;
	}

	private void ChangeScreens(Transform? screenToChangeTo = null)
	{
		foreach(Transform screen in new List<Transform>{lobbyConnectMenuScreen, lobbyLoadingScreen, lobbyMenuScreen})
		{
			if(screen == screenToChangeTo)
			{
				screen.gameObject.SetActive(true);
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


	
	
}