using System.Collections;
using System.Collections.Generic;
using Sanctum_Core;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    [SerializeField] private Transform clientLibrary;
    [SerializeField] private Transform clientGraveyard;
    [SerializeField] private Transform clientExile;
    [SerializeField] private Transform opponentLibrary;
    [SerializeField] private Transform opponentGraveyard;
    [SerializeField] private Transform opponentExile;

    Dictionary<CardZone, IPhysicalCardContainer> clientZoneToCardContainer;
    Dictionary<CardZone, IPhysicalCardContainer> opponentZoneToCardContainer;
    void Start()
    {
        clientZoneToCardContainer = new()
        {
            {CardZone.Library , clientLibrary.GetComponent<IPhysicalCardContainer>()},
            {CardZone.Graveyard , clientGraveyard.GetComponent<IPhysicalCardContainer>()},
            {CardZone.Exile , clientExile.GetComponent<IPhysicalCardContainer>()},
        };
        opponentZoneToCardContainer = new()
        {
            {CardZone.Library , opponentLibrary.GetComponent<IPhysicalCardContainer>()},
            {CardZone.Graveyard , opponentGraveyard.GetComponent<IPhysicalCardContainer>()},
            {CardZone.Exile , opponentExile.GetComponent<IPhysicalCardContainer>()},
        };
        SetupZones(clientZoneToCardContainer);
        SetupZones(opponentZoneToCardContainer);
    }

    private void SetupZones(  Dictionary<CardZone, IPhysicalCardContainer> zoneToContainer)
    {
        foreach (var (zone, container) in zoneToContainer)
        {
            container.SetZone(zone);
        }
    }

    public void SetupListeners(Playtable table, Player clientPlayer, Dictionary<string,string> uuidToName)
    {
        UnityLogger.Log("Setting up listeners");
        foreach (string uuid in uuidToName.Keys)
        {
            Player player = table.GetPlayer(uuid);

            if (clientPlayer == player)
            {
                this.RegisterCardZoneListeners(clientZoneToCardContainer, player);
            }
            else
            {
                this.RegisterCardZoneListeners(opponentZoneToCardContainer, player);
            }
            foreach (CardZone zone in CardZone.GetValues(typeof(CardZone)))
            {
                CardContainerCollection collection = player.GetCardContainer(zone);
                collection.removeCardIds.nonNetworkChange += (attribute) => OnCardRemoved(attribute, collection);
            }
        }
    }
    private void RegisterCardZoneListeners(Dictionary<CardZone,IPhysicalCardContainer> zoneToContainer, Player player)
    {
        foreach(var kvp in zoneToContainer)
        {
            var cardContainer = player.GetCardContainer(kvp.Key);
            cardContainer.boardState.nonNetworkChange += kvp.Value.OnCardAdded;
        }
    }

    public void OnCardRemoved(NetworkAttribute attribute, CardContainerCollection collection)
    {
        List<int> removedCardIds = ((NetworkAttribute<List<int>>)attribute).Value;
        foreach(int removedId in removedCardIds)
        {
            collection.RemoveCardFromContainer(removedId, networkChange: false);
        }
        if(!GameOrchestrator.Instance.IsRenderedAttribute(attribute))
        {
            return;
        }
        Player currentOpponent = GameOrchestrator.Instance.opponentRotator.GetCurrentOpponent();
        if(currentOpponent.GetCardContainer(collection.Zone) == collection)
        {
            clientZoneToCardContainer[collection.Zone].UpdateHolder(collection.ToList());
        }
        else
        {
            clientZoneToCardContainer[collection.Zone].UpdateHolder(collection.ToList());
        }
    }
}
