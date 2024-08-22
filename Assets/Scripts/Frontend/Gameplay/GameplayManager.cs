using System.Collections.Generic;
using System.Linq;
using Sanctum_Core;
using TMPro;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
	private class OpponentRotator
	{
		private List<string> opponents;
		int currentIndex = 0;
		public OpponentRotator(List<string> opponents)
		{
			this.opponents = opponents;
			this.opponents.Sort();
		}

		public void Left()
		{
			this.currentIndex = this.currentIndex - 1 < 0 ? this.opponents.Count - 1 : this.currentIndex - 1;
		}

		public void Right()
		{
			this.currentIndex  += 1;
			this.currentIndex %= opponents.Count;
		}
	}

	[SerializeField]
	private Transform mainGameScreen;

	[SerializeField]
	private TextMeshProUGUI opponentName;
	[SerializeField]
	private TextMeshProUGUI opponentHealth;

	[SerializeField]
	private TextMeshProUGUI clientName;
	[SerializeField]
	private TextMeshProUGUI clientHealth;

	private OpponentRotator currentOpponentSelector;

	private Playtable playtable;
	private List<string> opponentUUIDs;

	private string pathToResources;
	
	void Start()
	{
		GameOrchestrator.Instance.serverListener.gameStarted += OnGameStarted;
		pathToResources = $"{Application.dataPath}/Resources";
		mainGameScreen.gameObject.SetActive(false);
	}
	
	private void OnGameStarted(string uuid, string name, Dictionary<string,string> players)
	{
		this.playtable = new Playtable(players.Count, $"{this.pathToResources}/cards.csv",$"{this.pathToResources}/tokens.csv", isSlavePlaytable: true );
		GameOrchestrator.Instance.serverListener.networkAttributeChanged += this.playtable.networkAttributeFactory.HandleNetworkedAttribute;
		mainGameScreen.gameObject.SetActive(true);
		this.opponentUUIDs = players.Keys.Where(key => key != uuid).ToList();
		this.currentOpponentSelector = new(this.opponentUUIDs);
	}

	private void RenderOpponent()
	{
		
	}

}