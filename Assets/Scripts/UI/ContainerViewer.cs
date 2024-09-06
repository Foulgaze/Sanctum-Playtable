using System.Collections;
using System.Collections.Generic;
using Sanctum_Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContainerViewer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI name;
    [SerializeField] Button closeBtn;
    [SerializeField] GridLayoutGroup gridLayout;
    public int cardsPerRow = 7; 
    CardContainerCollection collection;
    private List<Transform> cardTransforms = new();

    void Start()
    {
        closeBtn.onClick.AddListener(() => Destroy(this.gameObject));
    }
    void Setup(CardContainerCollection collection, string windowName)
    {
        name.text = windowName;
        this.collection = collection;
    }

    public void UpdateCardContainer(NetworkAttribute attribute)
    {
        cardTransforms.ForEach(transform => Destroy(transform.gameObject));
        cardTransforms.Clear();
        List<List<int>> cardIdsRaw = ((NetworkAttribute<List<List<int>>>)attribute).Value;
        if(cardIdsRaw.Count == 0 || cardIdsRaw[0].Count == 0)
        {
            return;
        }
        List<int> cardIds = cardIdsRaw[0];

    }

    public void SetupGridLayout(float horizontalSpacingPercentage = .05f, float verticalSpacingPercentage = 0.3f)
{
    RectTransform holderRect = gridLayout.GetComponent<RectTransform>();
    float availableWidth = holderRect.rect.width;
    float totalHorizontalPadding = gridLayout.padding.left + gridLayout.padding.right;

    float totalWidthForCellsAndSpacing = availableWidth - totalHorizontalPadding;
    
    float cellWidth = totalWidthForCellsAndSpacing / (cardsPerRow + (cardsPerRow - 1) * horizontalSpacingPercentage);
    float cellHeight = cellWidth * 7/5;
    float spacing = cellWidth * horizontalSpacingPercentage;

    // Set the calculated spacing and cell size
    gridLayout.spacing = new Vector2(spacing, cellHeight * verticalSpacingPercentage);
    gridLayout.cellSize = new Vector2(cellWidth, cellHeight);
}


}
