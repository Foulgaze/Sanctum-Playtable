using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

public class Lobby
{
	private string? lobbyCode = null;
	private List<string> playerNames = new();

	Action<List<string>> playersInLobbyAction = delegate{};
	public event PropertyChangedEventHandler lobbyCodeChanged = delegate{};

	public Lobby()
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
		this.playersInLobbyAction(this.playerNames);
	}



	
}