using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using UnityEngine;

public class Lobby
{
	private string? lobbyCode = null;
	private List<string> playerNames = new();
	public Action<List<string>> playersInLobbyChanged = delegate{};
	public Action<Playtable> startGame = delegate{};
	public Action lobbyCreatedOrJoined = delegate{};

	private readonly string pathToAssets;
	public event PropertyChangedEventHandler lobbyCodeChanged = delegate{};

	public Lobby(string pathToAssets)
	{

	}

	public String LobbyCode
	{
		get
		{
			return this.lobbyCode;
		}
		set
		{
			if(value == this.lobbyCode)
			{
				return;
			}
			this.lobbyCode = value;
			lobbyCodeChanged(lobbyCodeChanged, new PropertyChangedEventArgs("lobbyCode"));
		}
	}
	public void UpdatePlayersInLobby(string rawList)
	{
		List<string> result;
		try
		{
			result = JsonConvert.DeserializeObject<List<string>>(rawList);
		}
		catch (Exception ex)
		{
			Logger.Log($"Could not convert {rawList} to list - {ex.Message}");
			return;
		}
		this.playerNames = result;
		this.playersInLobbyChanged(this.playerNames);
	}

	public void StartGame(string rawPlayerDict)
	{
		Dictionary<string,string>? playerDict;
		try
		{
			playerDict = JsonConvert.DeserializeObject<Dictionary<string,string>>(rawPlayerDict);
		}
		catch (Exception ex)
		{
			Logger.Log($"Unable to parse - {rawPlayerDict}");
			return;
		}
		Playtable newTable = new Playtable(playerDict.Keys.Count,$"{pathToAssets}/CSVs/cards.csv", $"{pathToAssets}/CSVs/tokens.csv" );
		// playerDict.foreach (uuid =>  newTable.)
		
	}



	
}