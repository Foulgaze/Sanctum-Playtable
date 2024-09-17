using System.Collections;
using System.Collections.Generic;
using Sanctum_Core;
using TMPro;
using UnityEngine;

public class OpponentHandController : MonoBehaviour, IPhysicalCardContainer
{
    [SerializeField] private TextMeshProUGUI handCountText;
    public void AddCard(int cardId)
    {
    }

    public void FlipTopCard(NetworkAttribute value)
    {
    }

    public CardZone GetZone()
    {
        return CardZone.Hand;
    } 

    public bool IsOpponent()
    {
        return true;
    }

    public void OnCardAdded(NetworkAttribute attribute)
    {
        if(!GameOrchestrator.Instance.IsRenderedAttribute(attribute))
        {
            return;
        }
        var cardsInHand = ((NetworkAttribute<List<List<int>>>)attribute).Value;
        if(cardsInHand.Count == 0)
        {
            handCountText.text = 0.ToString();
            return;
        }
        handCountText.text = cardsInHand[0].Count.ToString();
    }

    public void RerenderContainer()
    {
    }

    public bool RevealTopCard()
    {
        return false;
    }

    public void Setup(CardZone zone, bool isOpponent)
    {
    }

    public void UpdateHolder(List<List<int>> boardDescription)
    {
    }
}
