using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;
using Sanctum_Core;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ContainerViewer : MonoBehaviour, IDroppable
{
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private Button closeBtn;
    [SerializeField] private GridLayoutGroup gridLayout;
    [SerializeField] private Transform hiddenCardPrefab;
    public int cardsPerRow = 5;
    CardContainerCollection collection;
    private bool isOpponents;

    List<(int, Transform)> idToTransform = new();
    private Vector2 defaultSize;
    private int? revealCardCount;
    private HashSet<int> revealedCardIds = new();
    private bool firstTimeLoading = true;
    public void Setup(CardContainerCollection collection, string windowName, bool isOpponents, int? revealCardCount = null)
    {
        closeBtn.onClick.AddListener(() => Destroy(this.gameObject));
        defaultSize = GetComponent<RectTransform>().rect.size;
        name.text = windowName;
        this.collection = collection;
        collection.boardState.nonNetworkChange += UpdateCardContainer;
        this.isOpponents = isOpponents;
        this.revealCardCount = revealCardCount;
        SetupRevealedCards(collection.boardState);
        UpdateCardContainer(collection.boardState);
        if(isOpponents)
        {
            gridLayout.gameObject.layer = LayerMask.NameToLayer("UI");
        }
    }

    private bool RevealAllCards()
    {
        return revealCardCount == null;
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
            if(!RevealAllCards() && !revealedCardIds.Contains(kvp.Item1))
            {
                Destroy(kvp.Item2.gameObject);
            }
            else
            {
                CardFactory.Instance.DisposeOfCard(kvp.Item1, kvp.Item2, onField:false);
            }
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
        List<int> returnCards = new List<int>(cardIdsRaw[0]);
        returnCards.Reverse();
        return returnCards;
    }

    private void SetupRevealedCards(NetworkAttribute attribute)
    {
        if(RevealAllCards())
        {
            return;
        }
        List<int> cardIds = GetCardsToRender(attribute);
        int trueRevealCount = Math.Clamp((int)revealCardCount,0, cardIds.Count - 1);
        revealedCardIds = new HashSet<int>(cardIds.GetRange(0, trueRevealCount));
    }
    private void SetupGridLayout(int cardCount, float horizontalSpacingPercentage = 0.03f, float verticalSpacingPercentage = 0.1f)
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
    
    private Transform GetCardImage(int cardId)
    {
        if(RevealAllCards() || revealedCardIds.Contains(cardId))
        {
            return CardFactory.Instance.GetCardImage(cardId, isOpponents);
        }
        return Instantiate(hiddenCardPrefab);

    }
    private void SetupGridCard(int cardId)
    {
        Transform cardImage = GetCardImage(cardId);
        cardImage.transform.SetParent(gridLayout.transform);
		cardImage.GetComponent<RectTransform>().localScale = Vector3.one;
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

    private int GetClosestGridChildIndex(GridLayoutGroup gridLayoutGroup, Vector2 position, int totalColumns, int totalRows)
    {
        Vector2 cellSize = gridLayoutGroup.cellSize;
        Vector2 spacing = gridLayoutGroup.spacing;

        int column = Mathf.FloorToInt(position.x / (cellSize.x + spacing.x));
        int row = Mathf.FloorToInt(position.y / (cellSize.y + spacing.y)) * -1 - 1;
        int totalChildren = gridLayoutGroup.transform.childCount;

        column = Mathf.Clamp(column, 0, totalColumns - 1);
        row = Mathf.Clamp(row, 0, totalRows - 1);

        int childIndex = row * totalColumns + column;

        childIndex = Mathf.Clamp(childIndex, 0, totalChildren - 1);
        return childIndex;
    }

    private Vector2 GetMousePositionInGrid()
    {
        RectTransform rect = gridLayout.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect,
            Input.mousePosition,
            null,
            out Vector2 localPoint);
        return localPoint - new Vector2(rect.rect.size.x * -1,rect.rect.size.y)/2;
    }

    private int AlignInsertIndex(Vector2 mousePos, int insertPosition)
    {
        RectTransform rect = idToTransform[insertPosition].Item2.GetComponent<RectTransform>();
        if(mousePos.x < rect.anchoredPosition.x)
        {
            insertPosition += 1;
        }
        return insertPosition;   
    }

    private int? GetInsertPosition(int insertCardId)
    {
        int totalRows = Mathf.CeilToInt(idToTransform.Count / (float)cardsPerRow);
        Vector2 mousePos = GetMousePositionInGrid();
        int closestChild = GetClosestGridChildIndex(gridLayout, mousePos, cardsPerRow ,totalRows);
        int initialInsertId = idToTransform[closestChild].Item1;
        idToTransform.Reverse();
        int nonAdjustedInsertPosition = idToTransform.FindIndex(kvp => kvp.Item1 == initialInsertId);
        int adjustedInsertPosition = AlignInsertIndex(mousePos, nonAdjustedInsertPosition);
        for(int i = 0; i < adjustedInsertPosition; ++i)
        {
            if(idToTransform[i].Item1 == insertCardId)
            {
                adjustedInsertPosition -= 1;
                break;
            }
        }
        return adjustedInsertPosition;
    }

    private InsertCardData? InsertCardIntoContainer(int cardId)
    {
        if(idToTransform.Count == 0)
        {
            return new InsertCardData(null, cardId, null, false); 
        }   
        int? insertPosition = GetInsertPosition(cardId);
        if(!insertPosition.HasValue)
        {
            return null;
        }
        return new InsertCardData(null, cardId, insertPosition , false);
    }

    void Update()
    {
        if(Input.GetMouseButtonDown((int)MouseButton.Left))
        {
            
        }
    }

    public void DropCard(int cardId)
    {
        if(isOpponents)
        {
            return;
        }
        InsertCardData? insertData;
        if(RevealAllCards())
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
        if(insertData == null)
        {
            return;
        }
        GameOrchestrator.Instance.MoveCard(collection.Zone, insertData);
    }

    private void OnDestroy()
    {
        ClearExistingCards();
        collection.boardState.nonNetworkChange -= UpdateCardContainer;
    }
}
