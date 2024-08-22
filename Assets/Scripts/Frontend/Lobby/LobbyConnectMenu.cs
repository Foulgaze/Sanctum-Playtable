using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyConnectMenu : MonoBehaviour
{
    [SerializeField]
    private Button submitUsernameBtn;
    [SerializeField]
    private Transform usernameBackground;
    [SerializeField]
    private Button createLobbyBtn;
    [SerializeField]
    private Button joinLobbyBtn;
    [SerializeField]
    private TMP_InputField usernameField;
    [SerializeField]
    private TMP_InputField lobbySizeField;
    [SerializeField]
    private TMP_InputField lobbyCodeField;

    public event Action triedToJoinOrCreateLobby = delegate{};

    void Start()
    {
        this.usernameBackground.gameObject.SetActive(false);
        this.createLobbyBtn.onClick.AddListener(() => 
        {
            this.SetupUsernameButton(() =>  this.CreateLobby(usernameField, lobbySizeField));

        });
        this.joinLobbyBtn.onClick.AddListener(() => 
        {
            this.SetupUsernameButton(() => this.JoinLobby(usernameField, lobbyCodeField));
        });
    }

    public void SetupUsernameButton(Action action)
    {
        this.usernameBackground.gameObject.SetActive(true);
        this.submitUsernameBtn.onClick.RemoveAllListeners();
        this.submitUsernameBtn.onClick.AddListener( () => action());
        this.submitUsernameBtn.onClick.AddListener(() => GameOrchestrator.Instance.uiHelper.DisableTransform(this.usernameBackground));
    }

    public void CreateLobby(TMP_InputField usernameField, TMP_InputField lobbySizeField)
	{
        if(string.IsNullOrEmpty(usernameField.text) || string.IsNullOrEmpty(lobbySizeField.text))
        {
            return;
        }
		if(!int.TryParse(lobbySizeField.text, out int lobbySize))
		{
			Debug.LogError($"Invalid value for lobby size - {lobbySizeField.text}");
			return;
		}
        bool connectedToServer = GameOrchestrator.Instance.serverListener.ConnectToServer();
        if(!connectedToServer)
        {
            Debug.LogError($"Unable to connect to server");
            return;
        }
		GameOrchestrator.Instance.serverListener.CreateLobby(usernameField.text, lobbySize);
        triedToJoinOrCreateLobby.Invoke();
	}

	public void JoinLobby(TMP_InputField usernameField, TMP_InputField lobbyCodeField)
	{
        if(string.IsNullOrEmpty(usernameField.text) || string.IsNullOrEmpty(lobbyCodeField.text))
        {
            Debug.LogError("Either username or lobbycode is empty");
            return;
        }
        bool connectedToServer = GameOrchestrator.Instance.serverListener.ConnectToServer();
        if(!connectedToServer)
        {
            Debug.LogError($"Unable to connect to server");
            return;
        }
		GameOrchestrator.Instance.serverListener.JoinToLobby(usernameField.text, lobbyCodeField.text);
        triedToJoinOrCreateLobby.Invoke();
	}
}
