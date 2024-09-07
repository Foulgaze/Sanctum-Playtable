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
    public int cardsPerRow = 7; 
    CardContainerCollection collection;
    private bool isOpponents;
    void Start()
    {
        closeBtn.onClick.AddListener(() => Destroy(this.gameObject));
    }
    public void Setup(CardContainerCollection collection, string windowName, bool isOpponents)
    {
        name.text = windowName;
        this.collection = collection;
        SetupGridLayout();
        collection.boardState.nonNetworkChange += UpdateCardContainer;
        this.isOpponents = isOpponents;
    }

    public void UpdateCardContainer(NetworkAttribute attribute)
    {
        foreach(GameObject child in transform)
        {
            Destroy(child);
        }

        List<List<int>> cardIdsRaw = ((NetworkAttribute<List<List<int>>>)attribute).Value;
        if(cardIdsRaw.Count == 0 || cardIdsRaw[0].Count == 0)
        {
            return;
        }

        List<int> cardIds = cardIdsRaw[0];
        foreach(var cardId in cardIds)
        {
            Transform cardImage = CardFactory.Instance.GetCardImage(cardId, isOpponents);
            cardImage.transform.SetParent(gridLayout.transform);
        }
    }

    private void SetupGridLayout(float horizontalSpacingPercentage = .05f, float verticalSpacingPercentage = 0.3f)
    {
        RectTransform holderRect = gridLayout.GetComponent<RectTransform>();
        float availableWidth = holderRect.rect.width;
        float totalHorizontalPadding = gridLayout.padding.left + gridLayout.padding.right;

        float totalWidthForCellsAndSpacing = availableWidth - totalHorizontalPadding;
        
        float cellWidth = totalWidthForCellsAndSpacing / (cardsPerRow + (cardsPerRow - 1) * horizontalSpacingPercentage);
        float cellHeight = cellWidth * HandController.cardHeightToWidthRatio;
        
        gridLayout.cellSize = new Vector2(cellWidth, cellHeight);
        gridLayout.spacing = gridLayout.cellSize * new Vector2(horizontalSpacingPercentage, verticalSpacingPercentage);
    }


}
