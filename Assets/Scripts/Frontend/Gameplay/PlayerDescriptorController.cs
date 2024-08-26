
using System.Collections.Generic;
using Sanctum_Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDescriptionController : MonoBehaviour
{
	[SerializeField] private Transform opponentDescriptor;
	[SerializeField] private TextMeshProUGUI opponentName;
	[SerializeField] private TextMeshProUGUI opponentHealth;

	[SerializeField] private TextMeshProUGUI clientName;
	[SerializeField] private TMP_InputField clientHealth;
	[SerializeField] private Button healthIncreaseBtn;
	[SerializeField] private Button healthDecreaseBtn;

	public OpponentRotator rotator;

	void Start()
	{
	}
	public void InitializeDescriptions(Playtable table, Player clientPlayer, OpponentRotator rotator)
	{
		this.rotator = rotator;
		this.InitialzieClientListeners(clientPlayer);
		this.SetupOpponentListeners(table, clientPlayer, rotator);
	}

	public void InitialzieClientListeners(Player clientPlayer)
	{
		this.clientHealth.text = clientPlayer.Health.Value.ToString(); 
		this.clientName.text = clientPlayer.Name;
		clientPlayer.Health.nonNetworkChange += (attribute) => this.clientHealth.text = ((NetworkAttribute<int>)attribute).Value.ToString();
		this.clientHealth.onEndEdit.AddListener((rawHealth) => clientPlayer.Health.SetValue(int.Parse(rawHealth)));

		this.healthIncreaseBtn.onClick.AddListener(() => clientPlayer.isIncreasingHealth.SetValue(true));
		this.healthDecreaseBtn.onClick.AddListener(() => clientPlayer.isIncreasingHealth.SetValue(false));
	}

	private void SetupOpponentListeners(Playtable table, Player clientPlayer, OpponentRotator rotator)
	{
		if(!rotator.HasOpponents())
		{
			UnityLogger.Log("Disabled Descriptor");
			this.opponentDescriptor.gameObject.SetActive(false);
			return;
		}
		this.RenderOpponent();
		rotator.opponentUUIDs.ForEach(uuid => table.GetPlayer(uuid).Health.nonNetworkChange += OnOpponentHealtChange);
	}

	private void OnOpponentHealtChange(NetworkAttribute attribute)
	{
		if(GameOrchestrator.Instance.IsRenderedAttribute(attribute))
		{
			this.opponentHealth.text = ((NetworkAttribute<int>)attribute).Value.ToString();
		}
	}

	private void RenderOpponent()
	{
		this.opponentName.text = this.rotator.GetCurrentOpponent().Name;
		this.opponentHealth.text = this.rotator.GetCurrentOpponent().Health.Value.ToString();
	}

}