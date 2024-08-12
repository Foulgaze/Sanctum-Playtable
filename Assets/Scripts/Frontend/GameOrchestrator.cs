
using System.Net;
using UnityEngine;
public class GameOrchestrator : MonoBehaviour
{
	public static GameOrchestrator Instance { get; private set; }
    public ServerListener serverListener;
    private const int serverPort = 51522;
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
        serverListener = new(IPAddress.Loopback.ToString(),serverPort, Application.dataPath );
        
    }
	
}