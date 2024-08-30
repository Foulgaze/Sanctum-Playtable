using System;
using System.Collections.Generic;
using System.Linq;
using Sanctum_Core;
using UnityEngine;
using UnityEngine.UI;

public class HandController : MonoBehaviour, IPhysicalCardContainer
{
    [SerializeField] private Transform leftHandLocation;
    [SerializeField] private Transform middleHandLocation;
    [SerializeField] private Transform rightHandLocation;
    public CardZone zone;
    public int defaultHandSize = 7;
    private float percentageOfScreenForCardWidth = 0.1f;
    private readonly List<Transform> cardTransforms = new();
    void Start()
    {
        
        
    }
    public CardZone GetZone()
    {
        return this.zone;
    }

    public void SetZone(CardZone zone)
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

        if (IsBoardEmpty(boardDescription))
        {
            return;
        }

        List<int> cardIds = boardDescription[0];
        int cardCount = cardIds.Count;
        int handSize = cardIds.Count % 2 == 0 && cardIds.Count < defaultHandSize ? defaultHandSize + 1 : defaultHandSize;
        int bezierPointCount = Math.Max(cardCount, handSize);

        Vector2 cardDimensions = CalculateCardDimensions();
        Vector3[] cardPositions = GenerateCardPositions(bezierPointCount);
        float[] cardRotations = GenerateCardRotations(bezierPointCount);

        int positionIndex = CalculateStartPosition(cardCount, bezierPointCount);

        for (int i = 0; i < cardCount; ++i, ++positionIndex)
        {
 
            CreateAndPositionCard(cardIds[i], cardPositions[positionIndex], cardRotations[positionIndex], cardDimensions);
        }
    }

    private void ClearExistingCards()
    {
        cardTransforms.ForEach(card => Destroy(card.gameObject));
        cardTransforms.Clear();
    }

    private bool IsBoardEmpty(List<List<int>> boardDescription)
    {
        return boardDescription.Count == 0 || boardDescription[0].Count == 0;
    }

    private Vector2 CalculateCardDimensions()
    {
        float cardWidth = Screen.width * percentageOfScreenForCardWidth;
        return new Vector2(cardWidth, 7f / 5f * cardWidth);
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

    private void CreateAndPositionCard(int cardId, Vector3 position, float rotation, Vector2 cardDimensions)
    {
        Transform card = CardFactory.Instance.GetCardImage(cardId);
        card.GetComponent<RectTransform>().sizeDelta = cardDimensions;
        card.SetParent(transform);
        card.position = position;
        card.GetChild(0).GetComponent<Image>().color = UnityEngine.Random.ColorHSV();
        card.rotation = Quaternion.Euler(0, 0, 90 - rotation);
        card.GetComponent<Image>().color = UnityEngine.Random.ColorHSV();
        cardTransforms.Add(card);
    }
}
