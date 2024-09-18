using System.Collections;
using System.Collections.Generic;
using Sanctum_Core;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class commandTaxController : MonoBehaviour
{
    [SerializeField] private commanderTaxBtn plusBtn;
    [SerializeField] private commanderTaxBtn minusBtn;
    [SerializeField] private TextMeshProUGUI opponentCommandTax;
    [SerializeField] private TextMeshProUGUI clientCommandTax;
    public Player clientPlayer;

    public void Setup(Playtable table, Player clientPlayer, OpponentRotator rotator)
    {
        plusBtn.onClick += ChangeCommanderTax;
        minusBtn.onClick += ChangeCommanderTax;
        this.clientPlayer = clientPlayer;
        clientPlayer.commandTax.nonNetworkChange += UpdateClient;
        if(rotator.opponentUUIDs.Count == 0)
        {
            opponentCommandTax.gameObject.SetActive(false);
        }
        foreach(string opponentUuid in rotator.opponentUUIDs)
        {
            Player p = table.GetPlayer(opponentUuid);
            p.commandTax.nonNetworkChange += UpdateOpponent;
        }
        rotator.onPlayerChanged += (player) => {opponentCommandTax.text = player.commandTax.Value.ToString();};
    }

    private void UpdateOpponent(NetworkAttribute attribute)
    {
        if(!GameOrchestrator.Instance.IsRenderedAttribute(attribute))
        {
            return;
        }
        opponentCommandTax.text = ((NetworkAttribute<int>)attribute).Value.ToString();
    }

    private void UpdateClient(NetworkAttribute attribute)
    {
        clientCommandTax.text = ((NetworkAttribute<int>)attribute).Value.ToString();
    }
    

    private void ChangeCommanderTax(bool increaseValue)
    {
        clientPlayer.isIncreasingCommandTax.SetValue(increaseValue);
    }
}
