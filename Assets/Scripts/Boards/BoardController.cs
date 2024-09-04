using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sanctum_Core;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    [SerializeField] private Transform clientLibrary;
    [SerializeField] private Transform clientGraveyard;
    [SerializeField] private Transform clientExile;
    [SerializeField] private Transform clientMainField;
    [SerializeField] private Transform clientLeftField;
    [SerializeField] private Transform clientRightField;
    [SerializeField] private Transform clientHand;
    [SerializeField] private Transform opponentLibrary;
    [SerializeField] private Transform opponentGraveyard;
    [SerializeField] private Transform opponentExile;
    [SerializeField] private Transform opponentMainField;
    [SerializeField] private Transform opponentLeftField;
    [SerializeField] private Transform opponentRightField;

    Dictionary<CardZone, IPhysicalCardContainer> clientZoneToCardContainer;
    Dictionary<CardZone, IPhysicalCardContainer> opponentZoneToCardContainer;

	public static int cardContainerLayermask;

    void Start()
    {
		cardContainerLayermask = 1 << LayerMask.NameToLayer("CardContainer");

        clientZoneToCardContainer = new()
        {
            {CardZone.Library , clientLibrary.GetComponent<IPhysicalCardContainer>()},
            {CardZone.Graveyard , clientGraveyard.GetComponent<IPhysicalCardContainer>()},
            {CardZone.Exile , clientExile.GetComponent<IPhysicalCardContainer>()},
            {CardZone.MainField , clientMainField.GetComponent<IPhysicalCardContainer>()},
            {CardZone.LeftField , clientLeftField.GetComponent<IPhysicalCardContainer>()},
            {CardZone.RightField , clientRightField.GetComponent<IPhysicalCardContainer>()},
            {CardZone.Hand, clientHand.GetComponent<IPhysicalCardContainer>()},
        };
        opponentZoneToCardContainer = new()
        {
            {CardZone.Library , opponentLibrary.GetComponent<IPhysicalCardContainer>()},
            {CardZone.Graveyard , opponentGraveyard.GetComponent<IPhysicalCardContainer>()},
            {CardZone.Exile , opponentExile.GetComponent<IPhysicalCardContainer>()},
            {CardZone.MainField , opponentMainField.GetComponent<IPhysicalCardContainer>()},
            {CardZone.LeftField , opponentLeftField.GetComponent<IPhysicalCardContainer>()},
            {CardZone.RightField , opponentRightField.GetComponent<IPhysicalCardContainer>()},
        };
        SetupZones(clientZoneToCardContainer, isOpponent : false);
        SetupZones(opponentZoneToCardContainer, isOpponent : true);
    }

    private void SetupZones(  Dictionary<CardZone, IPhysicalCardContainer> zoneToContainer, bool isOpponent)
    {
        foreach (var (zone, container) in zoneToContainer)
        {
            container.Setup(zone,isOpponent);
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
        List<List<int>> cardsInCollection = collection.boardState.Value;
        List<int> removedCardIds = ((NetworkAttribute<List<int>>)attribute).Value;
        RemoveNumbers(cardsInCollection, removedCardIds);
        collection.boardState.NonNetworkedSet(cardsInCollection);
    }

    private void RemoveNumbers(List<List<int>> listOfLists, List<int> numbersToRemove)
    {
        HashSet<int> numbers = new HashSet<int>(numbersToRemove);
        // Iterate through each sublist
        foreach (var sublist in listOfLists)
        {
            // Remove each number in numbersToRemove from the current sublist
            sublist.RemoveAll(num => numbers.Contains(num));
        }
    }
}
