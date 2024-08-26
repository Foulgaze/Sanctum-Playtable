using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sanctum_Core;
using UnityEngine;

public interface IPhysicalCardContainer
{
    public CardZone GetZone();
    public void SetZone(CardZone zone);
    public void OnCardAdded(NetworkAttribute attribute);
    public void UpdateHolder(List<List<int>> boardDescription);
}
public class PileController : MonoBehaviour, IPhysicalCardContainer
{
    private CardZone zone;
    private Vector3 extents;
    private Vector3 cardTopperExtents;
    private Vector3 nextCardPosition;
    private List<int> removedCardIds = new();
    List<Transform> cardTransforms = new();
    void Start()
    {
        extents = this.transform.GetComponent<MeshRenderer>().bounds.extents;
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
        UnityLogger.Log("Updating Holder");

        nextCardPosition = transform.position + new Vector3(0,extents.y,0);
        cardTransforms.ForEach(prefab => Destroy(prefab));
        cardTransforms.Clear();
        if(boardDescription.Count == 0 || boardDescription[0].Count == 0)
        {
            return;
        }
        List<int> cardIds = boardDescription[0];
        UnityLogger.Log($"Rendering {cardIds.Count} cards"); 
        for(int i = 0; i < cardIds.Count; ++i)
        {
            int cardId = cardIds[i];
            Transform cardTopper = Instantiate(CardFactory.Instance.cardPilePrefab);
            cardTopper.localScale = new Vector3(extents.x*2,cardTopperExtents.y,extents.z*2 );
            cardTopper.position = GetNextCardPosition();
            cardTransforms.Add(cardTopper);
            if(i == cardIds.Count - 1)
            {
                PrepareTopCard(cardTopper, cardId);
                break;
            }
        }
    }

    private Vector3 GetNextCardPosition()
    {
        nextCardPosition += new Vector3(0,cardTopperExtents.y, 0);
        return nextCardPosition;
    }
    private void PrepareTopCard(Transform createdCard, int cardId)
    {
        Transform cardImage = CardFactory.Instance.GetCardImage(cardId);
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
    }

    public CardZone GetZone()
    {
        return this.zone;
    }

    public void SetZone(CardZone zone)
    {
        this.zone = zone;
    }

}
