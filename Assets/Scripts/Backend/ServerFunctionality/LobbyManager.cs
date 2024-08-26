using System;
using System.Collections.Generic;
using Newtonsoft.Json;
public class LobbyManager
{
	public class LobbyInfo
	{
		public int size;
		public string code;
		public string clientUUID;
		public string clientUsername;
		public List<string> playerNames = new();

        public override string ToString()
        {
            return $"Size : {size} - Code : {code} - UUID : {clientUUID} - Username : {clientUsername}";
        }
    }

	public LobbyInfo lobbyInfo = new();
	public event Action<LobbyInfo,Dictionary<string,string>> onLobbyFilled;
	public event Action<LobbyInfo> onLobbyChanged = delegate{};
	public event Action<NetworkInstruction, string> sendNetworkCommand;


	/// <summary>
	/// Prompts the server to create a lobby
	/// </summary>
	/// <param name="username"> user username</param>
	/// <param name="lobbySize"> lobbysize</param>
	public void CreateLobby(string username, int lobbySize)
	{
		this.lobbyInfo.clientUsername = username;
		this.lobbyInfo.size = lobbySize;
		this.sendNetworkCommand(NetworkInstruction.CreateLobby, $"{lobbySize}|{username}");
	}

	/// <summary>
	/// Attempts to connect to preexisting lobby based on code
	/// </summary>
	/// <param name="username">name of user passed to lobby</param>
	/// <param name="lobbyCode">code of the lobby you are trying to connect to</param>
	public void JoinLobby(string username, string lobbyCode)
	{
		this.lobbyInfo.clientUsername = username;
		this.lobbyInfo.code = lobbyCode;
		this.sendNetworkCommand(NetworkInstruction.JoinLobby, $"{lobbyCode}|{username}");
	}
	
	/// <summary>
	/// Handles create lobby instruction from server
	/// </summary>
	/// <param name="instruction">Raw instruction containing uuid|lobbycode</param>
	public void HandleCreateLobby(string instruction)
	{
		string[] data = instruction.Split('|');
		if(data.Length != 2)
		{
			UnityLogger.LogError($"Could not parse create lobby data - {instruction}");
			return;
		}
		this.lobbyInfo.clientUUID = data[0];
		this.lobbyInfo.code = data[1];
		this.lobbyInfo.playerNames.Add(this.lobbyInfo.clientUsername);
		this.onLobbyChanged(this.lobbyInfo);
	}

	/// <summary>
	/// Handles the join lobby instruction from the server. 
	/// </summary>
	/// <param name="instruction">Raw instruction containing playerUUID|lobbySize</param>
	public void HandleJoinLobby(string instruction)
	{
		string[] data = instruction.Split('|');
		if(data.Length != 2)
		{
			UnityLogger.LogError($"Unable to parse Join Lobby instruction - {instruction}");
			return;
		}
		if(!int.TryParse(data[1], out int lobbySize))
		{
			UnityLogger.LogError($"Invalid lobby size - {instruction} - {data[1]}");
			return;
		}
		this.lobbyInfo.clientUUID = data[0];
		this.lobbyInfo.size = lobbySize;
		this.lobbyInfo.playerNames.Add(this.lobbyInfo.clientUsername);
		this.onLobbyChanged(this.lobbyInfo);
	}
	
	/// <summary>
	/// Updates the players present in the lobby
	/// </summary>
	/// <param name="rawList">A JsonSerialized list</param>
	public void HandePlayersInLobby(string rawList)
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
		this.lobbyInfo.playerNames = result;
		this.onLobbyChanged(lobbyInfo);
	}

	/// <summary>
	/// Parses the dictionary that represents {uuid : playername} for all players in lobby
	/// </summary>
	/// <param name="rawPlayerDict">A Json serialized dict</param>
	public void ParsePlayersInLobby(string rawPlayerDict)
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
		this.onLobbyFilled(lobbyInfo, playerDict);
	}
}