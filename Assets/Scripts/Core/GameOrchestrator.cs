
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Sanctum_Core;
using UnityEngine;
using UnityEngine.UI;
using static LobbyManager;
public class GameOrchestrator : MonoBehaviour
{
    [SerializeField] private PlayerDescriptionController playerDescriptionController;
    [SerializeField] private LobbyMenu lobbyMenu;
    [SerializeField] private ScreenChanger lobbyScreenChanger;
    [SerializeField] private ConnectToLobbyMenu connectToLobbyMenu;
    [SerializeField] private BoardController boardController;
    [SerializeField] private RightClickMenuController rightClickMenuController; 
    [SerializeField] private CardIdentifier cardIdentifier; 
    public HandController handController;
	public static GameOrchestrator Instance { get; private set; }
    private LobbyManager lobbyManager = new();
    private ServerConnectionManager serverListener;
    private string pathToCSVs;
    private const int ServerPort = 51522;
    private Playtable playtable;
    public OpponentRotator opponentRotator;

	private void Awake() 
    {         
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
        this.Init();
    }

    private void Init()
    {
        Debug.developerConsoleVisible = true;
        serverListener = new(IPAddress.Loopback.ToString(),ServerPort);
        pathToCSVs = $"{Application.streamingAssetsPath}/CSVs/";
        InitListeners();
    }

    public void InitListeners()
    {
        InitLobbyManagerListeners();
        InitServerListenerListeners();
        InitConnectToLobbyMenuListeners();
        InitRightClickMenuControllerListeners();
    }

    private void InitLobbyManagerListeners()
    {
        lobbyManager.sendNetworkCommand += serverListener.SendMessage;
        lobbyManager.onLobbyChanged += lobbyMenu.UpdateLobbyDisplay;
        lobbyManager.onLobbyFilled += OnLobbyFilled;
    }

    private void InitServerListenerListeners()
    {
        serverListener.onNetworkCommandReceived[NetworkInstruction.CreateLobby] += lobbyManager.HandleCreateLobby;
        serverListener.onNetworkCommandReceived[NetworkInstruction.JoinLobby] += lobbyManager.HandleJoinLobby;
        serverListener.onNetworkCommandReceived[NetworkInstruction.PlayersInLobby] += lobbyManager.HandePlayersInLobby;
        serverListener.onNetworkCommandReceived[NetworkInstruction.StartGame] += lobbyManager.ParsePlayersInLobby;
        
        serverListener.onNetworkCommandReceived[NetworkInstruction.JoinLobby] += (_) => lobbyScreenChanger.OnChangeToLobbyMenu();
        serverListener.onNetworkCommandReceived[NetworkInstruction.CreateLobby] += (_) => lobbyScreenChanger.OnChangeToLobbyMenu();
    }

    private void InitConnectToLobbyMenuListeners()
    {
        connectToLobbyMenu.joinLobby += (_, _) => lobbyScreenChanger.OnChangeToLoadingScreen();
        connectToLobbyMenu.createLobby += (_, _) => lobbyScreenChanger.OnChangeToLoadingScreen();
        connectToLobbyMenu.joinLobby += lobbyManager.JoinLobby;
        connectToLobbyMenu.createLobby += lobbyManager.CreateLobby;
    }

    private void InitRightClickMenuControllerListeners()
    {
        rightClickMenuController.networkCommand += (instruction, payload) => serverListener.SendMessage(instruction, payload);
    }

    public void OnLobbyFilled(LobbyInfo info, Dictionary<string, string> players)
    {
        InitializePlaytable(players);
        SetupUIControllers(players);
        SetupNetworkListeners();
        SetupOpponents(players);
        SetupPlayerDescriptions(players);
        SetupLobbyMenu(players);
        SetupGameStartListener();
        SetupBoardController(players);
        CardFactory.Instance.playtable = playtable;
    }

    private void InitializePlaytable(Dictionary<string, string> players)
    {
        playtable = new Playtable(players.Count, $"{pathToCSVs}/cards.csv", $"{pathToCSVs}/tokens.csv", isSlave: true);
        players.Keys.ToList().ForEach(key => playtable.AddPlayer(key, players[key]));
        lobbyScreenChanger.OnPlaytableCreated(playtable);
    }

    private void SetupUIControllers(Dictionary<string, string> players)
    {
        Player clientPlayer = playtable.GetPlayer(lobbyManager.lobbyInfo.clientUUID);
        rightClickMenuController.clientPlayer = clientPlayer;
        rightClickMenuController.tokenSelectMenu.Setup(CardData.GetTokenUUINamePairs());
        clientPlayer.RevealCardZone.nonNetworkChange += (attribute) => rightClickMenuController.RevealOpponentZone(attribute, playtable);
        cardIdentifier.clientPlayer = clientPlayer;
    }

    private void SetupNetworkListeners()
    {
        playtable.networkAttributeFactory.attributeValueChanged += (attribute) => 
            serverListener.SendMessage(NetworkInstruction.NetworkAttribute, $"{attribute.Id}|{attribute.SerializedValue}");
        serverListener.onNetworkCommandReceived[NetworkInstruction.NetworkAttribute] += 
            playtable.networkAttributeFactory.HandleNetworkedAttribute;
    }

    private void SetupOpponents(Dictionary<string, string> players)
    {
        List<string> opponentUUIDs = players.Keys.Where(uuid => lobbyManager.lobbyInfo.clientUUID != uuid).ToList();
        opponentRotator = new(opponentUUIDs, playtable);
    }

    private void SetupPlayerDescriptions(Dictionary<string, string> players)
    {
        Player clientPlayer = playtable.GetPlayer(lobbyManager.lobbyInfo.clientUUID);
        playerDescriptionController.InitializeDescriptions(playtable, clientPlayer, opponentRotator);
    }

    private void SetupLobbyMenu(Dictionary<string, string> players)
    {
        Player clientPlayer = playtable.GetPlayer(lobbyManager.lobbyInfo.clientUUID);
        lobbyMenu.OnPlaytableCreated(playtable, clientPlayer, players);
    }

    private void SetupGameStartListener()
    {
        playtable.GameStarted.nonNetworkChange += (_) => lobbyScreenChanger.OnChangeToGameStart();
    }

    private void SetupBoardController(Dictionary<string, string> players)
    {
        Player clientPlayer = playtable.GetPlayer(lobbyManager.lobbyInfo.clientUUID);
        boardController.SetupListeners(playtable, clientPlayer, players, opponentRotator);
    }

    public bool isClientAttribute(NetworkAttribute attribute)
    {
        string attributeUUID = this.GetUUIDFromAttributeID(attribute);
        return attributeUUID == lobbyManager.lobbyInfo.clientUUID;
    }

    public bool isOpponentAttribute(NetworkAttribute attribute)
    {
        string attributeUUID = this.GetUUIDFromAttributeID(attribute);
        return !this.opponentRotator.HasOpponents() ? false : opponentRotator.GetCurrentOpponent().Uuid == attributeUUID;
    }
    public bool IsRenderedAttribute(NetworkAttribute attribute)
    {
        return isOpponentAttribute(attribute) || isClientAttribute(attribute);
    }

    public string GetUUIDFromAttributeID(NetworkAttribute attribute)
    {
        return string.Join('-', attribute.Id.Split('-'), 0, 5);
    }

    public void MoveCard(CardZone zone, InsertCardData data)
    {
        string uuid = lobbyManager.lobbyInfo.clientUUID;
        CardContainerCollection collection = playtable.GetPlayer(uuid).GetCardContainer(zone);
        collection.insertCardData.SetValue(data);
    }

    public void DisableRightClickMenu()
    {
        rightClickMenuController.CleanupMenu();
    }

    public void FlipLibraryTop()
    {
        CardContainerCollection collection = playtable.GetPlayer(lobbyManager.lobbyInfo.clientUUID).GetCardContainer(CardZone.Library);
        collection.revealTopCard.SetValue(!collection.revealTopCard.Value);
    }

    public void RevealZoneToOpponents(CardZone zone, List<string> uuids, int? revealCardCount)
    {
        uuids.ForEach(uuid => playtable.GetPlayer(uuid).RevealCardZone.SetValue((zone,lobbyManager.lobbyInfo.clientUUID, revealCardCount)));
    }

    public void SendSpecialAction(SpecialAction action, string payload)
    {
        playtable.specialAction.SetValue((payload,lobbyManager.lobbyInfo.clientUUID,action));
    }

    public string GetPlayerName(string uuid)
    {
        return playtable.GetPlayer(uuid).Name;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            opponentRotator.Right();
        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            opponentRotator.Left();
        }
        serverListener.ReadServerData();
    }	
}