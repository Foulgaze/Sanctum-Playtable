
using System;
using System.Collections.Generic;
using System.Net;
using Sanctum_Core;
using UnityEngine;
public class GameOrchestrator : MonoBehaviour
{
	public static GameOrchestrator Instance { get; private set; }

    public UIHelper uiHelper;
    public ServerListener serverListener;
    [SerializeField]
    public GameplayManager manager;
    private const int serverPort = 51522;
    public Dictionary<string,string> uuidToName = new();

    public event Action<Playtable> playtableCreated = delegate { };
    public event Action playtableGameStarted = delegate { };    
    public Playtable? playtable = null;
    public Player clientPlayer;
    [HideInInspector]
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
        serverListener = new(IPAddress.Loopback.ToString(),serverPort);
        manager.playtableCreated += (playtable) => playtableCreated?.Invoke(playtable);
    }

    void Update()
    {
        serverListener.ReadServerData();
    }	
}