using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sanctum_Core;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static LobbyManager;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyCodeText;
    [SerializeField] private TMP_InputField deckListField;

    [SerializeField] private TextMeshProUGUI readyUpBtnText;
    [SerializeField] private Button readyUpBtn;

    [SerializeField] private Transform connectedPlayerHolder;
    [SerializeField] private Transform connectedPlayerPrefab;
    [SerializeField] private TextMeshProUGUI playerConnectedCount;

    private readonly Color notReady = new Color(0.92f, 0.63f, 0.63f, 1f);
    private readonly Color ready = new Color(0.63f, 0.92f, 0.63f, 1f);
    private string lobbyCode;
    private Dictionary<string, Transform> playerConnectionPrefabs = new();

    /// <summary>
    /// Updates the lobby menu screen with the current lobby status
    /// </summary>
    /// <param name="lobby">Contains all the values for the lobby</param>
    public void UpdateLobbyDisplay(LobbyInfo lobby)
    {
        this.playerConnectionPrefabs.Values.ToList().ForEach(transform => Destroy(transform.gameObject));
        this.playerConnectionPrefabs.Clear();
        Dictionary<string,string> fauxUUIDToName = new();
        for(int i = 0; i < lobby.playerNames.Count; ++i)
        {
            fauxUUIDToName[i.ToString()] = lobby.playerNames[i];
        }
        this.CreatePlayerReadyIcons(fauxUUIDToName);
        this.playerConnectedCount.text = $"Connected Players - ({lobby.playerNames.Count}/{lobby.size})";
        this.lobbyCode = lobby.code;
        this.lobbyCodeText.text = $"Lobby Code : {lobby.code}";
    }

    /// <summary>
    /// Sets up icons for player ready status and setup player decklist
    /// </summary>
    /// <param name="table"> Created playtable</param>
    /// <param name="clientPlayer">The client connected to server</param>
    /// <param name="uuidToName">Dictionary of all players</param>
    public void OnPlaytableCreated(Playtable table, Player clientPlayer, Dictionary<string,string> uuidToName)
    {
        this.playerConnectionPrefabs.Values.ToList().ForEach(transform => Destroy(transform.gameObject));
        this.playerConnectionPrefabs.Clear();
        this.playerConnectedCount.text = $"Connected Players - ({uuidToName.Count}/{uuidToName.Count})";
        clientPlayer.DeckListRaw.SetValue(deckListField.text);
        clientPlayer.ReadiedUp.SetValue(!deckListField.IsInteractable());
        uuidToName.Keys.ToList().ForEach(key => 
        {   
            Player player = table.GetPlayer(key);
            player.ReadiedUp.nonNetworkChange += OnPlayerInPlaytableReadyStatusChange;
        });
        this.readyUpBtn.onClick.AddListener(() => this.UpdatePlayerDecklist(clientPlayer.DeckListRaw, clientPlayer.ReadiedUp));
        this.CreatePlayerReadyIcons(uuidToName);
        this.playerConnectionPrefabs.Values.ToList().ForEach(transform => transform.GetComponent<UnityEngine.UI.Image>().color = notReady);
        OnPlayerInPlaytableReadyStatusChange(clientPlayer.ReadiedUp);
    }

    /// <summary>
    /// Inverts the decklist editablility. Changes button text as a result
    /// </summary>
    public void ChangeDecklistState()
    {
        deckListField.interactable = !deckListField.interactable;
        readyUpBtnText.text = deckListField.interactable ? "Ready Up" : "Unready";
    }

    // Copied from SO \_0.0_/
    public void CopyLobbyCodeToClipboard()
    {
        TextEditor te = new TextEditor(); 
        te.text = this.lobbyCode; 
        te.SelectAll(); 
        te.Copy();
    }

    private void OnPlayerInPlaytableReadyStatusChange(NetworkAttribute attribute)
    {
        string uuid = GameOrchestrator.Instance.GetUUIDFromAttributeID(attribute); // ignore 4 - from uuid. 
        this.playerConnectionPrefabs[uuid].GetComponent<UnityEngine.UI.Image>().color = ((NetworkAttribute<bool>)attribute).Value ? ready : notReady;
    }

    private void CreatePlayerReadyIcons(Dictionary<string,string> uuidToName)
	{
        foreach(var pair in uuidToName)
        {
            Transform playerConnected = Instantiate(this.connectedPlayerPrefab, this.connectedPlayerHolder);
            TextMeshProUGUI playerNameText = playerConnected.GetChild(0).GetComponent<TextMeshProUGUI>();
            playerNameText.text = pair.Value;
            playerConnectionPrefabs[pair.Key] = playerConnected;
        }
	}

    private void UpdatePlayerDecklist(NetworkAttribute deckListRaw, NetworkAttribute readiedUp)
    {
        deckListRaw.SetValue(deckListField.text);
        readiedUp.SetValue(!deckListField.interactable);
    }
}
