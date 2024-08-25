using System;
using System.Collections.Generic;
using System.Linq;
using Sanctum_Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{

	[SerializeField] private Transform mainGameScreen;

	[SerializeField] private Transform opponentDescriptor;
	[SerializeField] private TextMeshProUGUI opponentName;
	[SerializeField] private TextMeshProUGUI opponentHealth;

	[SerializeField] private TextMeshProUGUI clientName;
	[SerializeField] private TMP_InputField clientHealth;
	[SerializeField] private Button healthIncreaseBtn;
	[SerializeField] private Button healthDecreaseBtn;

	public OpponentRotator? currentOpponentSelector = null;
	private Playtable playtable;
	private List<string> opponentUUIDs;
	private string pathToResources;
	public event Action<Playtable> playtableCreated = delegate{};
	
	void Start()
	{
		GameOrchestrator.Instance.serverListener.onLobbyFilled += OnLobbyFilled;
		pathToResources = $"{Application.streamingAssetsPath}/CSVs/";
		mainGameScreen.gameObject.SetActive(false);
	}
	
	private void OnLobbyFilled(string uuid, string name, Dictionary<string, string> players)
	{
		this.InitializePlaytable(players);
		this.AssignPlayerRoles(uuid, players);
		this.RegisterListeners();
		this.SetupOpponentSelector(uuid, players);
	}

	private void InitializePlaytable(Dictionary<string, string> players)
	{
		this.playtable = new Playtable(players.Count, $"{this.pathToResources}/cards.csv", $"{this.pathToResources}/tokens.csv", isSlave: true);
		players.Keys.ToList().ForEach(key => this.playtable.AddPlayer(key, players[key]));
		GameOrchestrator.Instance.playtable = this.playtable;
	}

	private void AssignPlayerRoles(string uuid, Dictionary<string, string> players)
	{
		GameOrchestrator.Instance.uuidToName = players;
		GameOrchestrator.Instance.clientPlayer = this.playtable.GetPlayer(uuid);
		this.playtableCreated(this.playtable);
	}

	private void RegisterListeners()
	{
		this.playtable.networkAttributeFactory.attributeValueChanged += GameOrchestrator.Instance.serverListener.NetworkAttributeChanged;
		GameOrchestrator.Instance.serverListener.networkAttributeChanged += this.playtable.networkAttributeFactory.HandleNetworkedAttribute;
		this.playtable.GameStarted.nonNetworkChange += OnGameStarted;
		this.playtable.GameStarted.nonNetworkChange += (attribute) => mainGameScreen.gameObject.SetActive(true);
	}

	private void SetupOpponentSelector(string uuid, Dictionary<string, string> players)
	{
		this.opponentUUIDs = players.Keys.Where(key => key != uuid).ToList();
		this.currentOpponentSelector = this.opponentUUIDs.Count == 0 ? null : new(this.opponentUUIDs, this.playtable);
	}

	private void OnGameStarted(NetworkAttribute gameStarted)
	{
		if(GameOrchestrator.Instance.hasGameStarted)
		{
			return;
		}
		GameOrchestrator.Instance.hasGameStarted = true;
		this.mainGameScreen.gameObject.SetActive(true);
		SetupClientListeners();
		SetupOpponentListeners();

	}

	private void SetupClientListeners()
	{
		clientHealth.text = GameOrchestrator.Instance.clientPlayer.Health.Value.ToString(); 
		clientName.text = GameOrchestrator.Instance.clientPlayer.Name;
		GameOrchestrator.Instance.clientPlayer.Health.nonNetworkChange += (attribute) => clientHealth.text = ((NetworkAttribute<int>)attribute).Value.ToString();
		this.clientHealth.onEndEdit.AddListener((rawHealth) => GameOrchestrator.Instance.clientPlayer.Health.SetValue(int.Parse(rawHealth)));

		healthIncreaseBtn.onClick.AddListener(() => GameOrchestrator.Instance.clientPlayer.isIncreasingHealth.SetValue(true));
		healthDecreaseBtn.onClick.AddListener(() => GameOrchestrator.Instance.clientPlayer.isIncreasingHealth.SetValue(false));
	}

	private void SetupOpponentListeners()
	{
		RenderOpponent();
		if(this.opponentUUIDs.Count == 0)
		{
			UnityLogger.Log("Disabled Descriptor");
			opponentDescriptor.gameObject.SetActive(false);
			return;
		}
		this.opponentUUIDs.ForEach(uuid => this.playtable.GetPlayer(uuid).Health.nonNetworkChange += OnOpponentHealtChange);
		// Do deck setup here
	}

	private void OnOpponentHealtChange(NetworkAttribute attribute)
	{
		Player opponent = this.currentOpponentSelector.currentOpponent();
		if(opponent.Health == attribute)
		{
			opponentHealth.text = ((NetworkAttribute<int>)attribute).Value.ToString();
		}
	}

	private void RenderOpponent()
	{
		if(currentOpponentSelector == null)
		{
			return;
		}
		Player opponent = this.currentOpponentSelector.currentOpponent();
		this.opponentName.text = opponent.Name;
		this.opponentHealth.text = opponent.Health.Value.ToString();
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.RightArrow))
		{
			this.currentOpponentSelector.Right();
			RenderOpponent();
		}
	}

}