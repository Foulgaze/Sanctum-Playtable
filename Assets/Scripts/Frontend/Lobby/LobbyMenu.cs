using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI lobbyCodeText;
    [SerializeField]
    private TMP_InputField deckListField;

    [SerializeField]
    private TextMeshProUGUI readyUpBtnText;

    [SerializeField]
    private Transform connectedPlayerHolder;
    [SerializeField]
    private Transform connectedPlayerPrefab;
    [SerializeField]
    private TextMeshProUGUI playerConnectedCount;

    private string lobbyCode;
    void Start()
    {
         GameOrchestrator.Instance.serverListener.lobby.lobbyChanged += UpdateLobbyDisplay;

    }

    public void UpdateLobbyDisplay(LobbyConnection lobby)
    {
        this.OnPlayerLobbyChange(lobby.playerNames);
        this.playerConnectedCount.text = $"Connected Players - ({lobby.playerNames}/{lobby.size})";
        this.lobbyCode = lobby.code;
        this.lobbyCodeText.text = $"Lobby Code : {lobby.code}";
    }

    private void OnPlayerLobbyChange(List<string> players)
	{
		foreach(Transform child in this.connectedPlayerHolder)
        {
            Destroy(child.gameObject);
        }
        foreach(string playerName in players)
        {
            Transform playerConnected = Instantiate(this.connectedPlayerPrefab, this.connectedPlayerHolder);
            TextMeshProUGUI playerNameText = playerConnected.GetChild(0).GetComponent<TextMeshProUGUI>();
            playerNameText.text = playerName;
        }
	}

    public void ChangeDecklistState()
    {
        deckListField.interactable = !deckListField.interactable;
        readyUpBtnText.text = deckListField.interactable ? "Ready Up" : "Unready";
        this.UpdatePlayerDecklist();
    }

    private void UpdatePlayerDecklist()
    {
        if(GameOrchestrator.Instance.playtable == null)
        {
            return;
        }
        GameOrchestrator.Instance.clientPlayer.DeckListRaw.SetValue(deckListField.text);
        GameOrchestrator.Instance.clientPlayer.ReadiedUp.SetValue(!deckListField.interactable);
    }

    // Copied from SO \_0.0_/
    public void CopyLobbyCodeToClipboard()
    {
        TextEditor te = new TextEditor(); 
        te.text = this.lobbyCode; 
        te.SelectAll(); 
        te.Copy();
    }

}
