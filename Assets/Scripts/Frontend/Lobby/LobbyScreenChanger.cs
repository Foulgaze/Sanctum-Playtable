using System;
using System.Collections.Generic;
using System.Linq;
using Sanctum_Core;
using TMPro;
using UnityEngine;

public class ScreenChanger : MonoBehaviour 
{
	
	[SerializeField] private Transform lobbyConnectMenuScreen;
	[SerializeField] private Transform lobbyLoadingScreen;
	[SerializeField] private Transform lobbyMenuScreen;
	[SerializeField] private Transform gameplayScreen;
	[SerializeField] private ConnectToLobbyMenu lobbyConnectMenu;

	void Start()
	{
		this.ChangeScreens(this.lobbyConnectMenuScreen);
	}

	public void OnPlaytableCreated(Playtable table)
	{
		table.GameStarted.nonNetworkChange += (_) => ChangeScreens(null);
	}


	private void ChangeScreens(Transform? screenToChangeTo = null)
	{
		foreach(Transform screen in new List<Transform>{lobbyConnectMenuScreen, lobbyLoadingScreen, lobbyMenuScreen, gameplayScreen})
		{
			if(screen == screenToChangeTo)
			{
				screen.gameObject.SetActive(true);
				continue;
			}
			screen.gameObject.SetActive(false);
		}
	}
	public void OnChangeToLoadingScreen()
	{
		this.ChangeScreens(this.lobbyLoadingScreen);
	}

	public void OnChangeToLobbyMenu()
	{
		this.ChangeScreens(this.lobbyMenuScreen);
	}

	public void OnChangeToGameStart()
	{
		this.ChangeScreens(gameplayScreen);
	}


	
	
}