using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sanctum_Core;
using Unity.VisualScripting;
using UnityEngine;


public class PileController : MonoBehaviour, IPhysicalCardContainer
{
    private CardZone zone;
    public bool isOpponent;
    private Vector3 extents;
    private Vector3 cardTopperExtents;
    private Vector3 nextCardPosition;
    private List<int> removedCardIds = new();
    List<Transform> cardTransforms = new();
    private List<int> currentlyHeadCards = new();

    public bool revealTopCard = true;
    void Start()
    {
        extents = this.transform.GetComponent<BoxCollider>().bounds.extents;
        cardTopperExtents = CardFactory.Instance.cardPilePrefab.GetComponent<MeshRenderer>().bounds.extents;
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
        ResetCardTransforms();

        if (boardDescription.Count == 0 || boardDescription[0].Count == 0)
            return;
        currentlyHeadCards = boardDescription[0];
        boardDescription[0].ForEach(InstantiateCardTopper);
        if (cardTransforms.Count != 0)
            PrepareTopCard(cardTransforms.Last(), boardDescription[0].Last());
    }

    private void ResetCardTransforms()
    {
        nextCardPosition = transform.position + new Vector3(0, extents.y, 0);
        cardTransforms.ForEach(prefab => Destroy(prefab.gameObject));
        cardTransforms.Clear();
    }

    private void InstantiateCardTopper(int cardId)
    {
        Transform cardTopper = Instantiate(CardFactory.Instance.cardPilePrefab);
        CardFactory.Instance.SetCardZone(cardId, this.zone);
        cardTopper.localScale = new Vector3(extents.x * 2, cardTopperExtents.y, extents.z * 2);
        cardTopper.position = GetNextCardPosition();
        cardTopper.SetParent(transform);
        cardTransforms.Add(cardTopper);
    }

    private Vector3 GetNextCardPosition()
    {
        nextCardPosition += new Vector3(0,cardTopperExtents.y, 0);
        return nextCardPosition;
    }
    private void PrepareTopCard(Transform createdCard, int cardId)
    {
        Transform cardImage = CardFactory.Instance.GetCardImage(cardId, isOpponent, renderCardBack : !revealTopCard);
        Transform canvas = createdCard.GetChild(0);

        canvas.gameObject.SetActive(true); // Enable canvas
        cardImage.SetParent(canvas);
        cardImage.position = Vector3.zero;
        PrepareTopCardImage(cardImage);
    }

    private void PrepareTopCardImage(Transform cardImage)
    {
        RectTransform imageRect = cardImage.GetComponent<RectTransform>();
        imageRect.localScale = Vector3.one;
        imageRect.anchorMin = Vector2.zero;
        imageRect.anchorMax = Vector2.one;
        imageRect.offsetMin = Vector2.zero;
        imageRect.offsetMax = Vector2.zero;
        imageRect.localEulerAngles = Vector3.zero;
        imageRect.anchoredPosition3D = Vector3.zero;

        cardImage.GetComponent<CardDrag>().renderCardBack = !revealTopCard;
    }

    public CardZone GetZone()
    {
        return this.zone;
    }

    public void Setup(CardZone zone, bool isOpponent)
    {
        this.zone = zone;
        this.isOpponent = isOpponent;
    }

    public void AddCard(int cardId)
    {
        GameOrchestrator.Instance.MoveCard(this.zone, new InsertCardData(null, cardId, null, false));
    }

    public void RerenderContainer()
    {
        UpdateHolder(new List<List<int>>{currentlyHeadCards});
    }

    public void FlipTopCard(NetworkAttribute attribute)
    {
        revealTopCard = ((NetworkAttribute<bool>)attribute).Value;
        RerenderContainer();
    }

    public bool RevealTopCard()
    {
        return revealTopCard;
    }

    public bool IsOpponent()
    {
        return this.isOpponent;
    }
}
