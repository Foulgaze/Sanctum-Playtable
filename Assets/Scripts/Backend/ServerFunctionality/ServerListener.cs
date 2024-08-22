

using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.ComponentModel;
using Sanctum_Core;
    public enum NetworkInstruction
    {
        CreateLobby, JoinLobby, PlayersInLobby, InvalidCommand, LobbyDescription, StartGame, NetworkAttribute, CardCreation, SpecialAction, Disconnect
    }
public class ServerListener
{
	private readonly TcpClient client = new();
	private readonly StringBuilder buffer = new();
	public readonly LobbyConnection lobby = new();
	public const int bufferSize = 4096;

	public event Action lobbyCreatedOrJoined = delegate{};
	public event Action<string,string,Dictionary<string,string>> gameStarted = delegate{};
	public event Action<string> networkAttributeChanged = delegate{};
	public event Action<string?> problemConnectingToServer = delegate{};
	private readonly string hostname;
	private readonly int portNumber;

	public ServerListener(string hostname,int portNumber)
	{
		this.hostname = hostname;
		this.portNumber = portNumber;
		this.lobby.gameStarted += this.gameStarted;
	}

	// public void PlaytableInit(Playtable table)
	// {
	// 	table.networkAttributeFactory.attributeValueChanged += (attribute) => {this.SendMessage(NetworkInstruction.NetworkAttribute, $"{attribute.Id}|{attribute.SerializedValue}");};
	// 	this.playtable = table;
	// }

	public bool ConnectToServer()
	{
		if(this.client.Connected)
		{
			return true;
		}
		try
		{
			this.client.Connect(hostname,portNumber);
		}
		catch
		{
			return false;
		}
		return true;
	}

	/// <summary>
	/// Prompts the server to create a lobby
	/// </summary>
	/// <param name="username"> user username</param>
	/// <param name="lobbySize"> lobbysize</param>
	public void CreateLobby(string username, int lobbySize)
	{
		this.lobby.username = username;
		this.lobby.size = lobbySize;
		this.SendMessage(NetworkInstruction.CreateLobby, $"{lobbySize}|{username}");
	}

	/// <summary>
	/// Attempts to connect to preexisting lobby based on code
	/// </summary>
	/// <param name="username">name of user passed to lobby</param>
	/// <param name="lobbyCode">code of the lobby you are trying to connect to</param>
	public void JoinToLobby(string username, string lobbyCode)
	{
		this.lobby.username = username;
		this.lobby.code = lobbyCode;
		this.SendMessage(NetworkInstruction.JoinLobby, $"{lobbyCode}|{username}");
	}

	public void ReadServerData()
	{
		if(!client.Connected)
		{
			return;
		}
		NetworkCommand? command;
		do
		{
			command = NetworkCommandManager.GetNextNetworkCommand(client.GetStream(),buffer, bufferSize);
			if(command == null)
			{
				return;
			}
			this.HandleServerCommand(command);
		}while(true);
	}

	private void HandleServerCommand(NetworkCommand command)
	{
		UnityLogger.Log($"Received:[{command}]");
		switch(command.opCode)
		{
			case (int)NetworkInstruction.CreateLobby:
				this.HandleCreateLobby(command.instruction);
				break;
			case (int)NetworkInstruction.JoinLobby:
				this.HandleJoinLobby(command.instruction);
				this.lobby.UpdatePlayersInLobby(JsonConvert.SerializeObject(new List<string>(){this.lobby.username}));
				break;
			case (int)NetworkInstruction.PlayersInLobby:
				this.lobby.UpdatePlayersInLobby(command.instruction);
				break;
			case (int)NetworkInstruction.StartGame:
				this.lobby.CreatePlaytable(command.instruction);
				break;
			case (int) NetworkInstruction.NetworkAttribute:
				this.networkAttributeChanged(command.instruction);
				break;
			case (int) NetworkInstruction.InvalidCommand:
				this.problemConnectingToServer(command.instruction);
				break;
			default:
				break;
		}
	}

	private void HandleCreateLobby(string instruction)
	{
		string[] data = instruction.Split('|');
		if(data.Length != 2)
		{
			UnityLogger.LogError($"Could not parse create lobby data - {instruction}");
			return;
		}
		this.lobby.uuid = data[0];
		this.lobby.code = data[1];
		this.lobby.UpdatePlayersInLobby(JsonConvert.SerializeObject(new List<string>(){lobby.username}));
		this.lobbyCreatedOrJoined();
	}


	private bool TryParseBoardUpdate(string rawBoardUpdate, out string uuid, out CardZone boardZone, out List<List<int>> boardUpdate)
	{
		uuid = string.Empty;
		boardZone = default;
		boardUpdate = null;

		var data = rawBoardUpdate.Split('|');
		if (data.Length != 2)
		{
			return false;
		}

		if (!TryParseUUIDAndZone(data[0], out uuid, out boardZone))
		{
			return false;
		}

		if (!TryParseBoardData(data[1], out boardUpdate))
		{
			return false;
		}

		return true;
	}

	private bool TryParseUUIDAndZone(string rawUUIDAndZone, out string uuid, out CardZone boardZone)
	{
		uuid = string.Empty;
		boardZone = default;

		var uuidBoardData = rawUUIDAndZone.Split('-');
		if (uuidBoardData.Length != 2 || !int.TryParse(uuidBoardData[1], out int boardId) || !EnumExtensions.IsDefined<CardZone>((CardZone)boardId))
		{
			return false;
		}

		uuid = uuidBoardData[0];
		boardZone = (CardZone)boardId;
		return true;
	}

	private bool TryParseBoardData(string rawBoardData, out List<List<int>> boardUpdate)
	{
		boardUpdate = null;
		try
		{
			boardUpdate = JsonConvert.DeserializeObject<List<List<int>>>(rawBoardData);
			return boardUpdate != null;
		}
		catch (JsonException)
		{
			return false;
		}
	}

	private void HandleJoinLobby(string instruction)
	{
		this.lobby.uuid = instruction;
		this.lobbyCreatedOrJoined();
	}


	private static string AddMessageSize(string message)
	{
		string msgByteSize = message.Length.ToString().PadLeft(4, '0');
		return msgByteSize + message;
	}

	/// <summary>
	/// Sends a serialized network command over the specified <see cref="NetworkStream"/>.
	/// </summary>
	/// <param name="stream">The <see cref="NetworkStream"/> to send the message through.</param>
	/// <param name="instruction">The <see cref="NetworkInstruction"/> that indicates the type of command being sent.</param>
	/// <param name="payload">The string payload containing additional data for the command.</param>
	public void SendMessage( NetworkInstruction instruction, string payload)
	{
		UnityLogger.Log($"Sending:[{new NetworkCommand((int)instruction, payload)}]");
		string message = JsonConvert.SerializeObject(new NetworkCommand((int)instruction, payload));
		byte[] data = Encoding.UTF8.GetBytes(AddMessageSize(message));
		this.client.GetStream().Write(data, 0, data.Length);
	}
}