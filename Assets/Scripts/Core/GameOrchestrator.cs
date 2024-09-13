
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Sanctum_Core;
using UnityEngine;
using static LobbyManager;
public class GameOrchestrator : MonoBehaviour
{
    [SerializeField] private PlayerDescriptionController playerDescriptionController;
    [SerializeField] private LobbyMenu lobbyMenu;
    [SerializeField] private ScreenChanger lobbyScreenChanger;
    [SerializeField] private ConnectToLobbyMenu connectToLobbyMenu;
    [SerializeField] private BoardController boardController;
    [SerializeField] private RightClickMenuController rightClickMenuController; 

    public HandController handController;
	public static GameOrchestrator Instance { get; private set; }
    private LobbyManager lobbyManager = new();
    private ServerConnectionManager serverListener;
    private string pathToCSVs;
    private const int ServerPort = 51522;
    private Playtable playtable;
    public OpponentRotator opponentRotator;
    private int insertCardID = 0;

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
        this.serverListener = new(IPAddress.Loopback.ToString(),ServerPort);
        this.pathToCSVs = $"{Application.streamingAssetsPath}/CSVs/";
        this.InitListeners();
    }

    private void InitListeners()
    {
        this.lobbyManager.sendNetworkCommand += this.serverListener.SendMessage;
        this.serverListener.onNetworkCommandReceived[NetworkInstruction.CreateLobby] += this.lobbyManager.HandleCreateLobby;
        this.serverListener.onNetworkCommandReceived[NetworkInstruction.JoinLobby] += this.lobbyManager.HandleJoinLobby;
        this.serverListener.onNetworkCommandReceived[NetworkInstruction.PlayersInLobby] += this.lobbyManager.HandePlayersInLobby;
        this.serverListener.onNetworkCommandReceived[NetworkInstruction.StartGame] += this.lobbyManager.ParsePlayersInLobby;
        this.lobbyManager.onLobbyChanged += this.lobbyMenu.UpdateLobbyDisplay;
        this.lobbyManager.onLobbyFilled += this.OnLobbyFilled;
        this.connectToLobbyMenu.joinLobby += (_, _) => this.lobbyScreenChanger.OnChangeToLoadingScreen();
        this.connectToLobbyMenu.createLobby += (_, _) => this.lobbyScreenChanger.OnChangeToLoadingScreen();
        this.connectToLobbyMenu.joinLobby += this.lobbyManager.JoinLobby;
        this.connectToLobbyMenu.createLobby += this.lobbyManager.CreateLobby;
        this.serverListener.onNetworkCommandReceived[NetworkInstruction.JoinLobby] += (_) => this.lobbyScreenChanger.OnChangeToLobbyMenu();
        this.serverListener.onNetworkCommandReceived[NetworkInstruction.CreateLobby] += (_) => this.lobbyScreenChanger.OnChangeToLobbyMenu();
        this.rightClickMenuController.networkCommand += (instruction, payload) => this.serverListener.SendMessage(instruction,payload);
    }

    public void OnLobbyFilled(LobbyInfo info,  Dictionary<string, string> players)
	{
		this.playtable = new Playtable(players.Count, $"{this.pathToCSVs}/cards.csv", $"{this.pathToCSVs}/tokens.csv", isSlave: true);
		players.Keys.ToList().ForEach(key => this.playtable.AddPlayer(key, players[key]));
        Player clientPlayer = this.playtable.GetPlayer(this.lobbyManager.lobbyInfo.clientUUID);
        rightClickMenuController.clientPlayer = clientPlayer;

		this.playtable.networkAttributeFactory.attributeValueChanged += (attribute) => this.serverListener.SendMessage(NetworkInstruction.NetworkAttribute, $"{attribute.Id}|{attribute.SerializedValue}");
		this.serverListener.onNetworkCommandReceived[NetworkInstruction.NetworkAttribute] += this.playtable.networkAttributeFactory.HandleNetworkedAttribute;
        List<string> opponentUUIDs = players.Keys.Where(uuid => this.lobbyManager.lobbyInfo.clientUUID != uuid).ToList();
        List<Player> opponents = opponentUUIDs.Select(uuid => playtable.GetPlayer(uuid)).ToList();
        clientPlayer.RevealCardZone.nonNetworkChange += (attribute) => rightClickMenuController.RevealOpponentZone(attribute, playtable);
        this.lobbyScreenChanger.OnPlaytableCreated(this.playtable);
        this.opponentRotator = new(opponentUUIDs, this.playtable);
        this.playerDescriptionController.InitializeDescriptions(this.playtable,clientPlayer, opponentRotator);
        this.lobbyMenu.OnPlaytableCreated(this.playtable, this.playtable.GetPlayer(lobbyManager.lobbyInfo.clientUUID), players);
        this.playtable.GameStarted.nonNetworkChange += (_) => this.lobbyScreenChanger.OnChangeToGameStart();
        boardController.SetupListeners(this.playtable, clientPlayer, players);
        CardFactory.Instance.playtable = this.playtable;
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
        // serverListener.SendMessage(NetworkInstruction.SpecialAction, $"{(int)action}|{payload}");
    }



    public string GetPlayerName(string uuid)
    {
        return playtable.GetPlayer(uuid).Name;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKey(KeyCode.G))
        {
            this.SendSpecialAction(SpecialAction.Mill, "10");
        }
        if(Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKey(KeyCode.L))
        {
            serverListener.SendMessage(NetworkInstruction.NetworkAttribute, $"{this.lobbyManager.lobbyInfo.clientUUID}-{(int)CardZone.MainField}-insert|{JsonConvert.SerializeObject(new InsertCardData(0,insertCardID++,null, false))}");
        }
        if(Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKey(KeyCode.D))
        {
            serverListener.SendMessage(NetworkInstruction.NetworkAttribute, $"{this.lobbyManager.lobbyInfo.clientUUID}-{(int)CardZone.LeftField}-insert|{JsonConvert.SerializeObject(new InsertCardData(0,insertCardID++,null, false))}");
        }
        if(Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKey(KeyCode.N))
        {
            this.SendSpecialAction(SpecialAction.Draw, "1");
        }
        serverListener.ReadServerData();
    }	
}