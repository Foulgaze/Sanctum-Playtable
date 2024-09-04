using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sanctum_Core;
using UnityEngine;
using UnityEngine.UI;

public class FieldController : MonoBehaviour, IPhysicalCardContainer
{
    private CardZone zone;
    private bool isOpponent;
    private Vector3 extents;
    private static float widthToHeightRatio = 4/3f;
    private float percentageOfCardAsSpacer = 0.25f;
    public int defaultCardCount = 0;
    private Dictionary<int, GameObject> idToCardOnField = new();
    private List<List<int>> currentlyHeldCardContainers = new();
    private int cardLayermask;
    
    void Start()
    {
        this.extents = GetComponent<MeshRenderer>().bounds.extents;
        cardLayermask = 1 << LayerMask.NameToLayer("cardSection");
        
    }
    public CardZone GetZone()
    {
        return this.zone;
    }

    public void OnCardAdded(NetworkAttribute attribute)
    {
        if(!GameOrchestrator.Instance.IsRenderedAttribute(attribute))
        {
            return;
        }
        UpdateHolder(((NetworkAttribute<List<List<int>>>)attribute).Value);
    }

    public void Setup(CardZone zone, bool isOpponent)
    {
        this.zone = zone;
        this.isOpponent = isOpponent;
    }

    public void UpdateHolder(List<List<int>> boardDescription)
    {
        currentlyHeldCardContainers = boardDescription;
        idToCardOnField.Values.ToList().ForEach(card => Destroy(card));
        idToCardOnField.Clear();

        int currentCardCount = Math.Max(boardDescription.Count, this.defaultCardCount);
        float cardWidth = transform.localScale.x / (currentCardCount + (currentCardCount + 1) * percentageOfCardAsSpacer);
        float spacerWidth = cardWidth * percentageOfCardAsSpacer;
        float totalWidth = (cardWidth * boardDescription.Count) + (spacerWidth * (boardDescription.Count + 1));

        Vector3 iterPosition = transform.position + new Vector3(-totalWidth / 2 + spacerWidth + cardWidth / 2, this.extents.y, 0);
        UnityLogger.Log($"Card Width - {cardWidth}");

        foreach(List<int> cardColumn in boardDescription)
        {
            this.RenderCardColumn(cardColumn, cardWidth, iterPosition);
            iterPosition += new Vector3(cardWidth + spacerWidth, 0, 0);
        }
    }

    private void RenderCardColumn(List<int> cardColumn, float cardWidth, Vector3 centerPosition)
    {
        float offsetX = 0.1f * cardWidth;
        float offsetZ = 0.1f * (cardWidth * 1/widthToHeightRatio);
        foreach(int cardId in cardColumn)
        {
            Transform onFieldCard = CardFactory.Instance.GetCardOnField(cardId, isOpponent);
            idToCardOnField[cardId] = onFieldCard.gameObject;
            onFieldCard.localScale = new Vector3(cardWidth, onFieldCard.localScale.y, cardWidth * 1/widthToHeightRatio);
            onFieldCard.position = centerPosition;
            onFieldCard.SetParent(transform);
            centerPosition += new Vector3(offsetX, onFieldCard.localScale.y,-offsetZ);
        }
    }

    public void AddCard(int cardId)
    {
        RaycastHit? hit = MouseUtility.Instance.RaycastFromMouse(cardLayermask);
        InsertCardData? insertCardData;
        if(hit != null)
        {
            insertCardData = AddCardToExistingContainer(((RaycastHit)hit).transform, cardId);
        }
        else
        {
            insertCardData = AddCardToNewColumn(cardId);
        }
        if(insertCardData == null)
        {
            return;
        }
        GameOrchestrator.Instance.MoveCard(this.zone, insertCardData);
    }

    private InsertCardData? AddCardToExistingContainer(Transform hitCard, int cardId)
    {
        int hitCardId = hitCard.parent.GetComponent<CardOnFieldComponents>().card.Id;
        if(hitCardId == cardId)
        {
            return null;
        }
        for(int currentContainerIndex = 0; currentContainerIndex < currentlyHeldCardContainers.Count; ++currentContainerIndex)
        {
            List<int> currentCardColumn = currentlyHeldCardContainers[currentContainerIndex];
            int cardIndex = currentCardColumn.IndexOf(hitCardId);
            if(cardIndex != -1)
            {
                return new InsertCardData(insertPosition : currentContainerIndex, cardID: cardId, containerInsertPosition: null, createNewContainer: false);
            }
        }
        UnityLogger.LogError($"Could not find card {hitCardId} in held cards");
        return new InsertCardData(null, hitCardId, null, false);
    }

    private InsertCardData? AddCardToNewColumn(int cardId)
    {
        RaycastHit? hit = MouseUtility.Instance.RaycastFromMouse(BoardController.cardContainerLayermask);
		if(!hit.HasValue)
		{
            UnityLogger.LogError($"Can't find card container for card Id {cardId} ");
			return new InsertCardData(null, cardId, null, false);
		}
        float hitXPosition = hit.Value.point.x;
        int insertPosition = 0;
        currentlyHeldCardContainers = currentlyHeldCardContainers.Where(container => container.Any()).ToList(); // Remove empty containers
		for(; insertPosition < currentlyHeldCardContainers.Count; ++insertPosition)
        {
            float xPositionOfContainer = idToCardOnField[currentlyHeldCardContainers[insertPosition][0]].transform.position.x; 
            if(xPositionOfContainer > hitXPosition)
            {
                break;
            }
        }
        return new InsertCardData(insertPosition, cardId, null, true);

    }

    public void RerenderContainer()
    {
        UpdateHolder(this.currentlyHeldCardContainers);
    }

    public void RemoveCard(int cardId)
    {
        for (int i = 0; i < currentlyHeldCardContainers.Count; i++)
        {
            var innerList = currentlyHeldCardContainers[i];
            int index = innerList.IndexOf(cardId);
            if (index != -1)
            {
                innerList.RemoveAt(index);
                RerenderContainer();
                return;
            }
        }
    }
}
