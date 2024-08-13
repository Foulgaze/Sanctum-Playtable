using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class Lobby
{
	private string? lobbyCode = null;
	private List<string> playerNames = new();
	public event Action<List<string>> playersInLobbyChanged = delegate{};
	public event Action<Playtable> playtableCreated = delegate{};
	private readonly string pathToAssets;
	public event PropertyChangedEventHandler lobbyCodeChanged = delegate{};

	public Lobby(string pathToAssets)
	{
		this.pathToAssets = pathToAssets;
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
			this.lobbyCodeChanged(this.lobbyCode, new PropertyChangedEventArgs("lobbyCode"));
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
			Logger.LogError($"Could not convert {rawList} to list - {ex.Message}");
			return;
		}
		this.playerNames = result;
		this.playersInLobbyChanged.Invoke(this.playerNames);
	}

	public void CreatePlaytable(string rawPlayerDict)
	{
		Dictionary<string,string>? playerDict;
		try
		{
			playerDict = JsonConvert.DeserializeObject<Dictionary<string,string>>(rawPlayerDict);
		}
		catch (Exception ex)
		{
			Logger.LogError($"Unable to parse - {rawPlayerDict}");
			return;
		}
		Playtable newTable = new Playtable(playerDict.Keys.Count,$"{pathToAssets}/CSVs/cards.csv", $"{pathToAssets}/CSVs/tokens.csv" );
		playerDict.Keys.ToList().ForEach(uuid => newTable.AddPlayer(uuid, playerDict[uuid]));
		playtableCreated.Invoke(newTable);
	}



	
}