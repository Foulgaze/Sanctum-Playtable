using System.Collections.Generic;
using System.Linq;
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


	
	void Start()
	{
		GameOrchestrator.Instance.GameStarted += this.OnGameStarted;
		mainGameScreen.gameObject.SetActive(false);
	}
	
	private void OnGameStarted()
	{
		this.playtable = GameOrchestrator.Instance.playtable;
		mainGameScreen.gameObject.SetActive(true);
		opponentUUIDs = this.playtable.GetPlayers().Keys.ToList();
		opponentUUIDs.Remove(GameOrchestrator.Instance.serverListener.uuid);
		this.currentOpponentSelector = new(opponentUUIDs);
	}

	private void RenderOpponent()
	{
		
	}

}