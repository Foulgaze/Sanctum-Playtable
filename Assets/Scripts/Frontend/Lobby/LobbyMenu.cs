using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI playersInLobbyText;
    [SerializeField]
    private TextMeshProUGUI lobbyCodeText;
    [SerializeField]
    private TMP_InputField deckListField;

    [SerializeField]
    private TextMeshProUGUI readyUpBtnText;

    void Start()
    {
         GameOrchestrator.Instance.serverListener.lobby.lobbyChanged += UpdateLobbyDisplay;

    }

    public void UpdateLobbyDisplay(LobbyConnection lobby)
    {
        this.OnPlayerLobbyChange(lobby.playerNames);
        this.lobbyCodeText.text = lobby.code;
    }

    private void OnPlayerLobbyChange(List<string> players)
	{
		string currentPlayers = string.Join('\n',players);
		playersInLobbyText.text = currentPlayers;
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
        te.text = lobbyCodeText.text; 
        te.SelectAll(); 
        te.Copy();
    }

}
