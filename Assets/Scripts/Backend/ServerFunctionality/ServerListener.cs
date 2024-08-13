

using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.ComponentModel;

public enum NetworkInstruction
{
	CreateLobby, JoinLobby, PlayersInLobby, InvalidCommand, LobbyDescription, StartGame, NetworkAttribute, BoardUpdate, CardCreation, SpecialAction, Disconnect
}
public class ServerListener
{
	private readonly TcpClient client = new();
	private readonly StringBuilder buffer = new();

	public Playtable? playtable;

	public readonly Lobby lobby;
	public const int bufferSize = 4096;

	public string? uuid = null;
	private string? username = null;
	
	public event Action lobbyCreatedOrJoined = delegate{};

	private readonly string hostname;
	private readonly int portNumber;

	public ServerListener(string hostname,int portNumber, string pathToAssets)
	{
		this.hostname = hostname;
		this.portNumber = portNumber;
		this.lobby = new(pathToAssets);
	}

	public void PlaytableInit(Playtable table)
	{
		table.networkAttributeFactory.attributeValueChanged += (sender, args) => 
		{this.SendMessage(NetworkInstruction.NetworkAttribute, $"{sender}|{args.PropertyName}");};
		this.playtable = table;
	}

	private void CheckForTCPConnect()
	{
		if(this.client.Connected)
		{
			return;
		}
		client.Connect(hostname,portNumber);
		Logger.Log("Connecting");
	}

	/// <summary>
	/// Prompts the server to create a lobby
	/// </summary>
	/// <param name="username"> user username</param>
	/// <param name="lobbySize"> lobbysize</param>
	public void CreateLobby(string username, int lobbySize)
	{
		this.CheckForTCPConnect();
		this.username = username;
		this.SendMessage(NetworkInstruction.CreateLobby, $"{lobbySize}|{username}");
	}

	/// <summary>
	/// Attempts to connect to preexisting lobby based on code
	/// </summary>
	/// <param name="username">name of user passed to lobby</param>
	/// <param name="lobbyCode">code of the lobby you are trying to connect to</param>
	public void JoinToLobby(string username, string lobbyCode)
	{
		this.CheckForTCPConnect();
		this.username = username;
		this.lobby.LobbyCode = lobbyCode;
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
		Logger.Log($"Received:[{command}]");
		switch(command.opCode)
		{
			case (int)NetworkInstruction.CreateLobby:
				this.HandleCreateLobby(command.instruction);
				break;
			case (int)NetworkInstruction.JoinLobby:
				this.HandleJoinLobby(command.instruction);
				this.lobby.UpdatePlayersInLobby(JsonConvert.SerializeObject(new List<string>(){this.username}));
				break;
			case (int)NetworkInstruction.PlayersInLobby:
				this.lobby.UpdatePlayersInLobby(command.instruction);
				break;
			case (int)NetworkInstruction.StartGame:
				this.lobby.CreatePlaytable(command.instruction);
				break;
			case (int) NetworkInstruction.NetworkAttribute:
				if(playtable == null)
				{
					Logger.LogError($"Null playtable for instruction - {command.instruction}");
					break;
				}
				this.playtable.networkAttributeFactory.HandleNetworkedAttribute(command.instruction, new PropertyChangedEventArgs("instruction"));
				break;
			case (int) NetworkInstruction.BoardUpdate:
				this.HandleBoardUpdate(command.instruction);
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
			Logger.LogError($"Could not parse create lobby data - {instruction}");
			return;
		}
		this.uuid = data[0];
		string lobbyCode = data[1];
		this.lobby.LobbyCode = lobbyCode;
		this.lobby.UpdatePlayersInLobby(JsonConvert.SerializeObject(new List<string>(){this.username}));
		this.lobbyCreatedOrJoined();
	}

	private void HandleBoardUpdate(string rawBoardUpdate)
	{
		if (!TryParseBoardUpdate(rawBoardUpdate, out string uuid, out CardZone boardZone, out List<List<int>> boardUpdate))
		{
			Logger.LogError($"Invalid board update - [{rawBoardUpdate}]");
			return;
		}

		var player = playtable.GetPlayer(uuid);
		if (player == null)
		{
			Logger.LogError($"Unable to find player with UUID - [{uuid}]");
			return;
		}

		var collection = player.GetCardContainer(boardZone);
		collection.SetCollectionValue(boardUpdate);
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
		this.uuid = instruction;
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
		Logger.Log($"Sending:[{new NetworkCommand((int)instruction, payload)}]");
		string message = JsonConvert.SerializeObject(new NetworkCommand((int)instruction, payload));
		byte[] data = Encoding.UTF8.GetBytes(AddMessageSize(message));
		this.client.GetStream().Write(data, 0, data.Length);
	}
}