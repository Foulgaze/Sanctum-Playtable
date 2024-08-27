using System;
using System.Collections;
using System.Collections.Generic;
using Sanctum_Core;
using UnityEngine;

public class FieldController : MonoBehaviour, IPhysicalCardContainer
{
    private CardZone zone;
    private Vector3 extents;
    private static float widthToHeightRatio = 4/3f;
    private static int maxColumnCount = 3;
    private float percentageOfCardAsSpacer = 0.25f;
    public int defaultCardCount = 0;
    private List<GameObject> cardsOnField = new();
    
    void Start()
    {
        this.extents = GetComponent<MeshRenderer>().bounds.extents;
        
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

    public void SetZone(CardZone zone)
    {
        this.zone = zone;
    }

    public void UpdateHolder(List<List<int>> boardDescription)
    {
        cardsOnField.ForEach(card => Destroy(card));
        int currentCardCount = Math.Max(boardDescription.Count, this.defaultCardCount);
        float cardWidth = transform.lossyScale.x / (currentCardCount + (currentCardCount - 1) * percentageOfCardAsSpacer);
        float totalWidth = (cardWidth * boardDescription.Count) + (cardWidth * percentageOfCardAsSpacer * (boardDescription.Count - 1));
        Vector3 iterPosition = transform.position + new Vector3(-totalWidth / 2 + cardWidth / 2, this.extents.y, 0);
        foreach(List<int> cardColumn in boardDescription)
        {
            this.RenderCardColumn(cardColumn, cardWidth, iterPosition);
            iterPosition += new Vector3(cardWidth + cardWidth * percentageOfCardAsSpacer, 0, 0);
        }
    }

    private void RenderCardColumn(List<int> cardColumn, float cardWidth, Vector3 centerPosition)
    {
        float offsetX = 0.1f * cardWidth;
        float offsetZ = 0.1f * (cardWidth * 1/widthToHeightRatio);
        foreach(int cardId in cardColumn)
        {
            Transform onFieldCard = CardFactory.Instance.GetCardOnField(cardId);
            cardsOnField.Add(onFieldCard.gameObject);
            onFieldCard.localScale = new Vector3(cardWidth, onFieldCard.localScale.y, cardWidth * 1/widthToHeightRatio);
            onFieldCard.transform.position = centerPosition;
            centerPosition += new Vector3(offsetX, onFieldCard.localScale.y,-offsetZ);
        }
    }

}
