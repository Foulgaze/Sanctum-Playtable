

using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

public enum NetworkInstruction
{
	CreateLobby, JoinLobby, PlayersInLobby, InvalidCommand, LobbyDescription, StartGame, NetworkAttribute, BoardUpdate, CardCreation, SpecialAction, Disconnect
}
public class ServerListener
{
	private readonly TcpClient client;
	private readonly StringBuilder buffer = new();

	public readonly Lobby lobby =  new();
	public const int bufferSize = 4096;

	private string? uuid = null;
	private string? username = null;
	public ServerListener(string hostname,int portNumber )
	{
		client = new(hostname, portNumber);
	}

	/// <summary>
	/// Prompts the server to create a lobby
	/// </summary>
	/// <param name="username"> user username</param>
	/// <param name="lobbySize"> lobbysize</param>
	public void CreateLobby(string username, int lobbySize)
	{
		this.username = username;
		SendMessage(client.GetStream(), NetworkInstruction.CreateLobby, $"{lobbySize}|{username}");
	}

	/// <summary>
	/// Attempts to connect to preexisting lobby based on code
	/// </summary>
	/// <param name="username">name of user passed to lobby</param>
	/// <param name="lobbyCode">code of the lobby you are trying to connect to</param>
	public void ConnectToLobby(string username, string lobbyCode)
	{
		this.username = username;
		SendMessage(client.GetStream(), NetworkInstruction.JoinLobby, $"{lobbyCode}|{username}");
	}

	public void ReadServerData()
	{
		NetworkCommand? instruction;
		do
		{
			instruction = NetworkCommandManager.GetNextNetworkCommand(client.GetStream(),buffer, bufferSize);

		}while(instruction != null);
	}

	private void HandleServerCommand(NetworkCommand command)
	{
		switch(command.opCode)
		{
			case (int)NetworkInstruction.CreateLobby:
				this.HandleCreateLobby(command.instruction);
				break;
			case (int)NetworkInstruction.JoinLobby:
				this.lobby.UpdatePlayersInLobby(command.instruction);
				break;
			case (int)NetworkInstruction.PlayersInLobby:
				break;
			case (int) NetworkInstruction.NetworkAttribute:
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
			Logger.Log($"Could not parse create lobby data - {instruction}");
			return;
		}
		this.uuid = data[0];
		string lobbyCode = data[1];
		this.lobby.LobbyCode = lobbyCode;
		this.lobby.UpdatePlayersInLobby(username);
	}

	private void HandleJoinLobby(string instruction)
	{
		this.uuid = instruction;
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
	public static void SendMessage(NetworkStream stream, NetworkInstruction instruction, string payload)
	{
		string message = JsonConvert.SerializeObject(new NetworkCommand((int)instruction, payload));
		byte[] data = Encoding.UTF8.GetBytes(AddMessageSize(message));
		stream.Write(data, 0, data.Length);
	}
}