using System;
using System.Collections;
using System.Collections.Generic;
using Sanctum_Core;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ContainerViewer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI name;
    [SerializeField] Button closeBtn;
    [SerializeField] GridLayoutGroup gridLayout;
    public int cardsPerRow = 5;
    CardContainerCollection collection;
    private bool isOpponents;

    Dictionary<int, Transform> idToTransform = new();
    private Vector2 defaultSize;
    public void Setup(CardContainerCollection collection, string windowName, bool isOpponents)
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

        foreach(var kvp in idToTransform)
        {
            CardFactory.Instance.DisposeOfCard(kvp.Key, kvp.Value, onField:false);
        }

        idToTransform.Clear();

        List<List<int>> cardIdsRaw = ((NetworkAttribute<List<List<int>>>)attribute).Value;

        if(cardIdsRaw.Count == 0 || cardIdsRaw[0].Count == 0)
        {
            return;
        }

        List<int> cardIds = cardIdsRaw[0];
        SetupGridLayout(cardIds.Count);

        foreach(var cardId in cardIds)
        {
            Transform cardImage = CardFactory.Instance.GetCardImage(cardId, isOpponents);
            cardImage.transform.SetParent(gridLayout.transform);
            idToTransform[cardId] = cardImage;
        }
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
}
