
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using Newtonsoft.Json;
using UnityEngine;
public class GameOrchestrator : MonoBehaviour
{
	public static GameOrchestrator Instance { get; private set; }
    public ServerListener serverListener;
    private const int serverPort = 51522;
    private Dictionary<string,string> uuidToName = new();

    public event Action<Playtable> playtableCreated = delegate { };
    public event Action GameStarted = delegate { };
    
    public Playtable? playtable = null;
    public Player clientPlayer;

    public bool hasGameStarted = false;
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
        serverListener = new(IPAddress.Loopback.ToString(),serverPort, $"{Application.dataPath}/Resources");
        serverListener.lobby.playtableCreated += InitPlaytable;
    }

    private void InitPlaytable(Playtable table)
    {
        Player clientPlayer = table.GetPlayer(this.serverListener.uuid);
        if(this.serverListener.uuid == null || clientPlayer == null)
        {
            Debug.LogError("Null UUID at playtable init");
        }
        this.playtable = table;
        this.clientPlayer = clientPlayer;
        this.uuidToName = table.GetPlayers();
        this.serverListener.PlaytableInit(this.playtable);
        playtableCreated.Invoke(this.playtable);
        this.playtable.GameStarted.valueChange += StartGame;
    }

    private void StartGame(object startGame, PropertyChangedEventArgs args)
    {

        if(!JsonConvert.DeserializeObject<bool>(args.PropertyName)  || this.hasGameStarted)
        {
            return;
        }
        this.hasGameStarted = true;
        GameStarted.Invoke();
    }



    void Update()
    {
        serverListener.ReadServerData();
    }	
}