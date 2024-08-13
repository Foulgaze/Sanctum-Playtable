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

    public void InitPlaytable(Playtable table)
    {
        Player? clientPlayer = table.GetPlayer(GameOrchestrator.Instance.serverListener.uuid);
        if(clientPlayer is null)
        {
            Debug.LogError($"Could not find player of UUID - {GameOrchestrator.Instance.serverListener.uuid}");
            return;
        }
        this.UpdatePlayerDecklist();
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
