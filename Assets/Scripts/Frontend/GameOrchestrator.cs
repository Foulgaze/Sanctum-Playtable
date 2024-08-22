
using System;
using System.Collections.Generic;
using System.Net;
using Sanctum_Core;
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
        serverListener = new(IPAddress.Loopback.ToString(),serverPort);
    }

    void Update()
    {
        serverListener.ReadServerData();
    }	
}