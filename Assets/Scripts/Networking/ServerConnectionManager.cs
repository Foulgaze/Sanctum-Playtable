

using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using Sanctum_Core;
public enum NetworkInstruction
{
    CreateLobby, JoinLobby, PlayersInLobby, InvalidCommand, LobbyDescription, StartGame, NetworkAttribute, Disconnect
}
public class ServerConnectionManager
{
	private TcpClient client = new();
	private readonly StringBuilder buffer = new();
	private const int bufferSize = 4096;
	private readonly string hostname;
	private readonly int portNumber;
	public Dictionary<NetworkInstruction, Action<string>> onNetworkCommandReceived = new();


	public ServerConnectionManager(string hostname,int portNumber)
	{
		this.hostname = hostname;
		this.portNumber = portNumber;
		foreach(NetworkInstruction instruction in Enum.GetValues(typeof(NetworkInstruction)))
		{
			onNetworkCommandReceived[instruction] = delegate { };
		}
	}

	/// <summary>
	/// Reads data from the TCPConnection. Handles any commands received
	/// </summary>
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
		if(!this.onNetworkCommandReceived.ContainsKey((NetworkInstruction)command.opCode))
		{
			UnityLogger.LogError($"Could not parse [{command}] because of opcode - {command.opCode}");
			return;
		}
		this.onNetworkCommandReceived[(NetworkInstruction)command.opCode](command.instruction);
	}

	/// <summary>
	/// Adds the preceding 4 characters to the message, representing the size
	/// </summary>
	/// <param name="message">The message that will have size appended </param>
	/// <returns></returns>
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
		if(!client.Connected)
		{
			client.Connect(this.hostname, this.portNumber);
		}
		UnityLogger.Log($"Sending:[{new NetworkCommand((int)instruction, payload)}]");
		string message = JsonConvert.SerializeObject(new NetworkCommand((int)instruction, payload));
		byte[] data = Encoding.UTF8.GetBytes(AddMessageSize(message));
		this.client.GetStream().Write(data, 0, data.Length);
	}
}