using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Sanctum_Core;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ContainerViewer : MonoBehaviour, IDroppable
{
    [SerializeField] TextMeshProUGUI name;
    [SerializeField] Button closeBtn;
    [SerializeField] GridLayoutGroup gridLayout;
    public int cardsPerRow = 5;
    CardContainerCollection collection;
    private bool isOpponents;

    List<(int, Transform)> idToTransform = new();
    private Vector2 defaultSize;
    private int? revealCardCount;
    public void Setup(CardContainerCollection collection, string windowName, bool isOpponents, int? revealCardCount = null)
    {
        closeBtn.onClick.AddListener(() => Destroy(this.gameObject));
        defaultSize = GetComponent<RectTransform>().rect.size;
        name.text = windowName;
        this.collection = collection;
        collection.boardState.nonNetworkChange += UpdateCardContainer;
        this.isOpponents = isOpponents;
        UpdateCardContainer(collection.boardState);
    }

    public void UpdateCardContainer(NetworkAttribute attribute)
    {

        ClearExistingCards();
        List<int> cardIds = GetCardsToRender(attribute);
        SetupGridLayout(cardIds.Count);
        cardIds.ForEach(cardId => SetupGridCard(cardId));
    }

    private void ClearExistingCards()
    {
        foreach(var kvp in idToTransform)
        {
            CardFactory.Instance.DisposeOfCard(kvp.Item1, kvp.Item2, onField:false);
        }
        idToTransform.Clear();
    }
    private List<int> GetCardsToRender(NetworkAttribute attribute)
    {
        List<List<int>> cardIdsRaw = ((NetworkAttribute<List<List<int>>>)attribute).Value;

        if(cardIdsRaw.Count == 0 || cardIdsRaw[0].Count == 0)
        {
            return new List<int>();
        }
        return cardIdsRaw[0];
    }
    private void SetupGridLayout(int cardCount, float horizontalSpacingPercentage = 0.05f, float verticalSpacingPercentage = 0.1f)
    {
        RectTransform holderRect = gridLayout.GetComponent<RectTransform>();
        float availableWidth = holderRect.rect.width;
        Vector2 totalPadding = new Vector2(gridLayout.padding.left + gridLayout.padding.right, 
                                        gridLayout.padding.top + gridLayout.padding.bottom);
        
        float widthForCellsAndSpacing = availableWidth - totalPadding.x;
        
        float cellWidth = CalculateCellWidth(widthForCellsAndSpacing, horizontalSpacingPercentage);
        float cellHeight = cellWidth * HandController.cardHeightToWidthRatio;
        
        int totalVerticalCards = Mathf.CeilToInt(cardCount / (float)cardsPerRow);
        float windowHeight = CalculateWindowHeight(cellHeight, totalVerticalCards, verticalSpacingPercentage, totalPadding.y);
        
        SetGridLayoutProperties(cellWidth, cellHeight, horizontalSpacingPercentage, verticalSpacingPercentage);
        AdjustRectTransform(windowHeight);
    }
    private void SetupGridCard(int cardId)
    {
        Transform cardImage = CardFactory.Instance.GetCardImage(cardId, isOpponents);
        cardImage.transform.SetParent(gridLayout.transform);
        idToTransform.Add((cardId,cardImage));  
    }

    private float CalculateCellWidth(float availableWidth, float horizontalSpacingPercentage)
    {
        return availableWidth / (cardsPerRow + (cardsPerRow - 1) * horizontalSpacingPercentage);
    }

    private float CalculateWindowHeight(float cellHeight, int totalVerticalCards, float verticalSpacingPercentage, float verticalPadding)
    {
        float calculatedHeight = cellHeight * (totalVerticalCards + (totalVerticalCards - 1) * verticalSpacingPercentage) + verticalPadding;
        return Mathf.Max(calculatedHeight, defaultSize.y);
    }

    private void SetGridLayoutProperties(float cellWidth, float cellHeight, float horizontalSpacingPercentage, float verticalSpacingPercentage)
    {
        gridLayout.cellSize = new Vector2(cellWidth, cellHeight);
        gridLayout.spacing = gridLayout.cellSize * new Vector2(horizontalSpacingPercentage, verticalSpacingPercentage);
    }

    private void AdjustRectTransform(float windowHeight)
    {
        RectTransform rect = gridLayout.transform.GetComponent<RectTransform>();
        float verticalOffset = windowHeight - rect.rect.size.y;
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, windowHeight);
        rect.anchoredPosition -= new Vector2(0, verticalOffset / 2);
    }

    private int FindClosestCardIndex(List<(int, RectTransform)> rects, Vector2 mousePos)
    {
        int minIndex = 0;
        float minDistance = float.MaxValue;
        for(int i = 0; i < rects.Count; ++i)
        {
            RectTransform rect = rects[i].Item2;
            float distance = Vector2.Distance(rect.anchoredPosition, mousePos);
            if(distance < minDistance)
            {
                minIndex = i;
                minDistance = distance;
            }
        }
        if(mousePos.x > rects[minIndex].Item2.anchoredPosition.x)
        {
            ++minIndex;
        }
        return minIndex;

    }

    private InsertCardData InsertCardIntoContainer(int cardId)
    {
        if(idToTransform.Count == 0)
        {
            return new InsertCardData(null, cardId, null, false); 
        }
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            gridLayout.GetComponent<RectTransform>(),
            Input.mousePosition,
            null,
            out Vector2 localPoint);
        List<(int, RectTransform)> rects = idToTransform.Select(pair => (pair.Item1, pair.Item2.GetComponent<RectTransform>())).ToList();
        int closestCardIndex = FindClosestCardIndex(rects, localPoint);
        return new InsertCardData(null, cardId, closestCardIndex, false);
    }

    public void DropCard(int cardId)
    {
        if(isOpponents)
        {
            return;
        }
        InsertCardData insertData;
        if(revealCardCount == null)
        {

            bool idAlreadyPresent = idToTransform.Any(pair => pair.Item1 == cardId);
            if(idAlreadyPresent)
            {
                return;
            }
            insertData = new InsertCardData(null, cardId, null, false);
        }
        else
        {
            insertData = InsertCardIntoContainer(cardId);
        }
        
        GameOrchestrator.Instance.MoveCard(collection.Zone, insertData);

    }
}
