using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;
using Sanctum_Core;
public class LobbyConnection
{
	public int? size = null;
	public string? code = null;
	public List<string> playerNames = new();
	public event Action<string,string,Dictionary<string,string>> gameStarted;
	public event Action<LobbyConnection> lobbyChanged = delegate{};
	public string? uuid = null;
	public string? username = null;

	public void UpdatePlayersInLobby(string rawList)
	{
		List<string> result;
		try
		{
			result = JsonConvert.DeserializeObject<List<string>>(rawList);
		}
		catch (Exception ex)
		{
			UnityLogger.LogError($"Could not convert {rawList} to list - {ex.Message}");
			return;
		}
		this.playerNames = result;
		this.lobbyChanged.Invoke(this);
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
			UnityLogger.LogError($"Unable to parse - {rawPlayerDict}");
			return;
		}
		this.gameStarted.Invoke(this.uuid, this.username, playerDict);
	}
}