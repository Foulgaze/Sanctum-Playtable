using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sanctum_Core;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyCodeText;
    [SerializeField] private TMP_InputField deckListField;

    [SerializeField] private TextMeshProUGUI readyUpBtnText;

    [SerializeField] private Transform connectedPlayerHolder;
    [SerializeField] private Transform connectedPlayerPrefab;
    [SerializeField] private TextMeshProUGUI playerConnectedCount;
    private string lobbyCode;
    private List<Transform> playerConnectionPrefabs = new();
    void Start()
    {
         GameOrchestrator.Instance.serverListener.lobby.lobbyChanged += UpdateLobbyDisplay;
         GameOrchestrator.Instance.playtableCreated += OnPlaytableCreated;
    }

    public void UpdateLobbyDisplay(LobbyConnection lobby)
    {
        this.OnPlayerLobbyChange(lobby.playerNames);
        this.playerConnectedCount.text = $"Connected Players - ({lobby.playerNames.Count}/{lobby.size})";
        this.lobbyCode = lobby.code;
        this.lobbyCodeText.text = $"Lobby Code : {lobby.code}";
    }

    public void OnPlaytableCreated(Playtable table)
    {
        this.UpdatePlayerReadyStatus();
        var uuidToName = GameOrchestrator.Instance.uuidToName;
        uuidToName.Keys.ToList().ForEach(key => 
        {
            Player player = table.GetPlayer(key);
            player.ReadiedUp.nonNetworkChange += OnPlayerChange;
        });
        List<string> uuids = uuidToName.Keys.ToList();
        OnPlayerInPlaytableReadyStatusChange();
    }

    private void OnPlayerChange(NetworkAttribute _)
    {
        OnPlayerInPlaytableReadyStatusChange();
    }

    private void UpdatePlayerReadyStatus()
    {
        GameOrchestrator.Instance.clientPlayer.DeckListRaw.SetValue(deckListField.text);
        GameOrchestrator.Instance.clientPlayer.ReadiedUp.SetValue(!deckListField.IsInteractable());
    }

    private void OnPlayerInPlaytableReadyStatusChange()
    {
        var uuidToName = GameOrchestrator.Instance.uuidToName;
        Playtable table = GameOrchestrator.Instance.playtable;
        List<string> uuids = uuidToName.Keys.ToList();
        this.OnPlayerLobbyChange(uuids.Select(uuid => uuidToName[uuid]).ToList());
        for(int i = 0; i < uuids.Count; ++i)
        {
            string uuid = uuids[i];
            Player player = (Player)table.GetPlayer(uuid);
            this.playerConnectionPrefabs[i].GetComponent<UnityEngine.UI.Image>().color = player.ReadiedUp.Value ? new Color(0.63f, 0.92f, 0.63f, 1f) : new Color(0.92f, 0.63f, 0.63f, 1f);
        }
    }

    private void OnPlayerLobbyChange(List<string> players)
	{
		foreach(Transform prefab in this.playerConnectionPrefabs)
        {
            Destroy(prefab.gameObject);
        }
        this.playerConnectionPrefabs.Clear();
        foreach(string playerName in players)
        {
            Transform playerConnected = Instantiate(this.connectedPlayerPrefab, this.connectedPlayerHolder);
            TextMeshProUGUI playerNameText = playerConnected.GetChild(0).GetComponent<TextMeshProUGUI>();
            playerNameText.text = playerName;
            playerConnectionPrefabs.Add(playerConnected);
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
        UnityLogger.Log("Updating Decklist status");
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
