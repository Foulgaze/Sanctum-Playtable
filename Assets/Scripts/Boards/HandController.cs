using System;
using System.Collections.Generic;
using System.Linq;
using Sanctum_Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HandController : MonoBehaviour, IPhysicalCardContainer, IDroppable
{
    [SerializeField] private Transform leftHandLocation;
    [SerializeField] private Transform middleHandLocation;
    [SerializeField] private Transform rightHandLocation;
    private RectTransform handBox;
    public readonly static float cardHeightToWidthRatio = 7f / 5f;
    public int? currentHeldCardId {get; set;}= null;
    private CardZone zone;
    public int defaultHandSize = 7;
    private static float percentageOfScreenForCardWidth = 0.1f;
    private readonly Dictionary<int,RectTransform> idToCardTransform = new();
    private List<int> currentlyHeldCards = new();
    void Start()
    {
        handBox = GetComponent<RectTransform>();
        
    }
    public CardZone GetZone()
    {
        return this.zone;
    }

    public void Setup(CardZone zone, bool _)
    {
        this.zone = zone;
    }

    public void OnCardAdded(NetworkAttribute attribute)
    {
        if(!GameOrchestrator.Instance.IsRenderedAttribute(attribute))
        {
            return;
        }
        UpdateHolder(((NetworkAttribute<List<List<int>>>)attribute).Value);
    }
    

    public void UpdateHolder(List<List<int>> boardDescription)
    {
        ClearExistingCards();

        if (boardDescription.Count == 0 || boardDescription[0].Count == 0)
        {
            return;
        }

        List<int> cardIds = boardDescription[0];
        this.currentlyHeldCards = cardIds;
        int cardCount = cardIds.Count;
        int handSize = cardIds.Count % 2 == 0 && cardIds.Count < defaultHandSize ? defaultHandSize + 1 : defaultHandSize;
        int bezierPointCount = Math.Max(cardCount, handSize);

        Vector2 cardDimensions = CalculateCardDimensions();
        Vector3[] cardPositions = GenerateCardPositions(bezierPointCount);
        float[] cardRotations = GenerateCardRotations(bezierPointCount);

        int positionIndex = CalculateStartPosition(cardCount, bezierPointCount);

        for (int i = 0; i < cardCount; ++i, ++positionIndex)
        {
 
            Transform newCard = CreateAndPositionCard(cardIds[i], cardPositions[positionIndex], cardRotations[positionIndex], cardDimensions);
            
            idToCardTransform[cardIds[i]] = newCard.GetComponent<RectTransform>();
        }
    }

    public bool CardInHand(int cardId)
    {
        return this.currentlyHeldCards.Contains(cardId);
    }

    private void ClearExistingCards()
    {
        foreach(var kvp in idToCardTransform)
        {
            kvp.Value.rotation = Quaternion.Euler(0,0,0);
            CardFactory.Instance.DisposeOfCard(kvp.Key, kvp.Value.transform, onField: false);
        }
        idToCardTransform.Clear();
    }

    public static Vector2 CalculateCardDimensions()
    {
        float cardWidth = Screen.width * percentageOfScreenForCardWidth;
        return new Vector2(cardWidth, cardHeightToWidthRatio * cardWidth);
    }

    private Vector3[] GenerateCardPositions(int bezierPointCount)
    {
        return BezierEquations.GenerateApproximatelyEquidistantPoints(
            leftHandLocation.position, middleHandLocation.position, rightHandLocation.position, bezierPointCount);
    }

    private float[] GenerateCardRotations(int bezierPointCount)
    {
        return BezierEquations.GenerateCardRotations(
            leftHandLocation.position, middleHandLocation.position, rightHandLocation.position, bezierPointCount);
    }

    private int CalculateStartPosition(int cardCount, int bezierPointCount)
    {
        int middlePosition = bezierPointCount / 2;
        return middlePosition - cardCount / 2;
    }

    private Transform CreateAndPositionCard(int cardId, Vector3 position, float rotation, Vector2 cardDimensions)
    {
        Transform card = CardFactory.Instance.GetCardImage(cardId,false);
        CardFactory.Instance.SetCardZone(cardId, this.zone);

        RectTransform rect = card.GetComponent<RectTransform>();
        rect.sizeDelta = cardDimensions;
        rect.localScale = Vector3.one;
        card.SetParent(transform);
        card.position = position;
        card.rotation = Quaternion.Euler(0, 0, 90 - rotation);
        return card;
    }

    

    public void AddCard(int cardId)
    {

        GameOrchestrator.Instance.MoveCard(this.zone, new InsertCardData(null, cardId, null, false));
    }

    public void RerenderContainer()
    {
        UpdateHolder(new List<List<int>>(){currentlyHeldCards});
    }

    public bool MouseInHand()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(handBox, Input.mousePosition);
    }

    public void FlipTopCard(NetworkAttribute value)
    {
        UnityLogger.LogError("Trying to flip top card of hand");
        return;
    }
    public bool RevealTopCard()
    {
        return true;
    }
    public void DropCard(int cardId, CardDrag droppedCardOn)
    {
        if(!idToCardTransform.ContainsKey(droppedCardOn.cardId))
        {
            UnityLogger.LogError($"Unable to find card {droppedCardOn.cardId} in hand box");
        }
        var transforms = idToCardTransform.Values.OrderBy(transform => transform.anchoredPosition.x).ToList();
        float mousePositionX = MouseUtility.Instance.GetMousePositionOnCanvas().x;
        int insertPosition = transforms.IndexOf(idToCardTransform[droppedCardOn.cardId]);
        if(mousePositionX >= transforms[insertPosition].anchoredPosition.x)
        {
            insertPosition += 1;
        }
        if(idToCardTransform.ContainsKey(cardId))
        {
            if(transforms.IndexOf(idToCardTransform[cardId]) < insertPosition)
            {
                insertPosition -= 1;
            }
        }
        GameOrchestrator.Instance.MoveCard(this.zone, new InsertCardData(null, cardId, insertPosition, false));

    }

    public void DropCard(int cardId)
    {
        float mousePositionX = MouseUtility.Instance.GetMousePositionOnCanvas().x;
        int? insertPosition = 0;
        var transforms = idToCardTransform.Values.OrderBy(transform => transform.anchoredPosition.x).ToList();
        for(; insertPosition < transforms.Count; ++insertPosition )
        {
            if(transforms[(int)insertPosition].anchoredPosition.x > mousePositionX)
            {
                break;
            }
        }
        if(idToCardTransform.ContainsKey(cardId))
        {
            if(transforms.IndexOf(idToCardTransform[cardId]) < insertPosition)
            {
                insertPosition -= 1;
            }
        }
        if(transforms.Count != 0 && transforms.Last().anchoredPosition.x < mousePositionX )
        {
            insertPosition = null;
        }
        
        GameOrchestrator.Instance.MoveCard(this.zone, new InsertCardData(null, cardId, insertPosition, false));
    }
    public bool IsOpponent()
    {
        return false;
    }
}
