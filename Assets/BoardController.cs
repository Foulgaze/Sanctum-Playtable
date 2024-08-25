using System.Collections;
using System.Collections.Generic;
using Sanctum_Core;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    [SerializeField] Transform clientLibrary;
    [SerializeField] Transform clientGraveyard;
    [SerializeField] Transform clientExile;
    [SerializeField] Transform opponentLibrary;
    [SerializeField] Transform opponentGraveyard;
    [SerializeField] Transform opponentExile;

    void Start()
    {
        GameOrchestrator.Instance.playtableCreated += SetupListeners;
        SetZone(clientLibrary, CardZone.Library);
        SetZone(clientGraveyard, CardZone.Graveyard);
        SetZone(clientExile, CardZone.Exile);
        SetZone(opponentLibrary, CardZone.Library);
        SetZone(opponentGraveyard, CardZone.Graveyard);
        SetZone(opponentExile, CardZone.Exile);
    }

    private void SetZone( Transform cardHolder,CardZone zone)
    {
        cardHolder.GetComponent<CardHolder>().SetZone(zone);
    }

    private void SetupListeners(Playtable table)
    {
        UnityLogger.Log("Setting up listeners");
        Player clientPlayer = GameOrchestrator.Instance.clientPlayer;

        foreach (string uuid in GameOrchestrator.Instance.uuidToName.Keys)
        {
            Player player = table.GetPlayer(uuid);

            if (player == null)
            {
                UnityLogger.LogError($"Player with UUID {uuid} not found in the table.");
                continue;
            }

            if (clientPlayer == player)
            {
                RegisterListeners(player, clientLibrary, clientGraveyard, clientExile);
            }
            else
            {
                RegisterListeners(player, opponentLibrary, opponentGraveyard, opponentExile);
            }
        }
    }

    private void RegisterListeners(Player player, Transform library, Transform graveyard, Transform exile)
    {
        RegisterCardZoneListeners(player, CardZone.Library, library);
        RegisterCardZoneListeners(player, CardZone.Graveyard, graveyard);
        RegisterCardZoneListeners(player, CardZone.Exile, exile);
    }

    private void RegisterCardZoneListeners(Player player, CardZone zone, Transform container)
    {
        var cardHolder = container.GetComponent<CardHolder>();
        var cardContainer = player.GetCardContainer(zone);

        if (cardHolder == null)
        {
            UnityLogger.LogError($"CardHolder component missing on {container.name}");
            return;
        }

        cardContainer.boardState.nonNetworkChange += cardHolder.OnCardAdded;
        cardContainer.removeCardIds.nonNetworkChange += cardHolder.OnCardRemoved;
    }
}
