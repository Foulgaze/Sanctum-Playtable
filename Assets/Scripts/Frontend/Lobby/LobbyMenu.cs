using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI playersInLobbyText;
    [SerializeField]
    private TextMeshProUGUI lobbyCodeText;

    void Start()
    {
        Lobby lobby = GameOrchestrator.Instance.serverListener.lobby;
        lobby.lobbyCodeChanged += this.OnLobbyCodeChanged;
        lobby.playersInLobbyChanged += this.OnPlayerLobbyChange;
    }

    private void OnPlayerLobbyChange(List<string> players)
	{
		string currentPlayers = string.Join('\n',players);
		playersInLobbyText.text = currentPlayers;
	}

	private void OnLobbyCodeChanged(object obj, EventArgs e)
	{
		this.lobbyCodeText.text = (string) obj;
	}

}
