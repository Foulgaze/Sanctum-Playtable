using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConnectToLobbyMenu : MonoBehaviour
{
    [SerializeField] private Button submitUsernameBtn;
    [SerializeField] private Transform usernameBackground;
    [SerializeField] private Button createLobbyBtn;
    [SerializeField] private Button joinLobbyBtn;
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField lobbySizeField;
    [SerializeField] private TMP_InputField lobbyCodeField;

    public event Action<string,int> createLobby = delegate{};
    public event Action<string,string> joinLobby = delegate{};

    void Start()
    {
        this.usernameBackground.gameObject.SetActive(false);
        this.createLobbyBtn.onClick.AddListener(() => 
        {
            this.SetupUsernameBtn(() =>  this.CreateLobby(usernameField, lobbySizeField));

        });
        this.joinLobbyBtn.onClick.AddListener(() => 
        {
            this.SetupUsernameBtn(() => this.JoinLobby(usernameField, lobbyCodeField));
        });
    }

    /// <summary>
    /// Adds the setup username button to the username btn whenever called
    /// </summary>
    /// <param name="submitAction"> The action that should be taken when the username is submitted </param>
    public void SetupUsernameBtn(Action submitAction)
    {
        this.usernameBackground.gameObject.SetActive(true);
        this.submitUsernameBtn.onClick.RemoveAllListeners();
        this.submitUsernameBtn.onClick.AddListener( () => submitAction());
        this.submitUsernameBtn.onClick.AddListener(() => this.usernameBackground.gameObject.SetActive(false));
    }

    /// <summary>
    /// Calls action that a lobby should be created
    /// </summary>
    /// <param name="usernameField">Field of username</param>
    /// <param name="lobbySizeField">Field of lobbysize</param>
    public void CreateLobby(TMP_InputField usernameField, TMP_InputField lobbySizeField)
	{
        if(string.IsNullOrEmpty(usernameField.text) || string.IsNullOrEmpty(lobbySizeField.text))
        {
            return;
        }
		if(!int.TryParse(lobbySizeField.text, out int lobbySize) || lobbySize < 1)
		{
			Debug.LogError($"Invalid value for lobby size - {lobbySizeField.text}");
			return;
		}
		this.createLobby(usernameField.text, lobbySize);
	}

    /// <summary>
    /// Calls action that a lobby should be joined
    /// </summary>
    /// <param name="usernameField">The username</param>
    /// <param name="lobbyCodeField">The lbbbycode</param>
	public void JoinLobby(TMP_InputField usernameField, TMP_InputField lobbyCodeField)
	{
        if(string.IsNullOrEmpty(usernameField.text) || string.IsNullOrEmpty(lobbyCodeField.text))
        {
            Debug.LogError("Either username or lobbycode is empty");
            return;
        }
		this.joinLobby(usernameField.text, lobbyCodeField.text);
	}
}
