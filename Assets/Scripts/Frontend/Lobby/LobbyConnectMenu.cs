using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyConnectMenu : MonoBehaviour
{
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
        createLobbyBtn.onClick.AddListener(() => this.CreateLobby(usernameField, lobbySizeField));
        joinLobbyBtn.onClick.AddListener(() => this.JoinLobby(usernameField, lobbyCodeField));

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
		GameOrchestrator.Instance.serverListener.CreateLobby(usernameField.text, lobbySize);
        triedToJoinOrCreateLobby.Invoke();
	}

	public void JoinLobby(TMP_InputField usernameField, TMP_InputField lobbyCodeField)
	{
        if(string.IsNullOrEmpty(usernameField.text) || string.IsNullOrEmpty(lobbyCodeField.text))
        {
            return;
        }
		GameOrchestrator.Instance.serverListener.JoinToLobby(usernameField.text, lobbyCodeField.text);
        triedToJoinOrCreateLobby.Invoke();
	}
}
