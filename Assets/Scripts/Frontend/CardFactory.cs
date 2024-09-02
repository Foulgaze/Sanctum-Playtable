
using System;
using System.Collections.Generic;
using Sanctum_Core;
using UnityEngine;

public class CardFactory : MonoBehaviour
{
	public static CardFactory Instance { get; private set; }
	private Dictionary<int, Transform> idToCardTransform = new();
	public Transform cardOnFieldPrefab;
	public Transform cardPilePrefab;
	public Transform cardImagePrefab;
	public Playtable playtable;
	
	private void Awake() 
    {         
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }
	public Transform GetCardOnField(int cardId)
	{
		if(idToCardTransform.ContainsKey(cardId))
		{
			return idToCardTransform[cardId];
		}
		Transform newCard = GameObject.Instantiate(cardOnFieldPrefab);
		newCard.GetChild(0).GetChild(0).GetComponent<CardDrag>().cardId = cardId; // SIGH i hate doing getchild
		Card card = GetCard(cardId);
		CardOnFieldComponents components = newCard.GetComponent<CardOnFieldComponents>();
		components.card = card;
		components.Setup(null);
		return newCard;
	}

	public Card GetCard(int id)
	{
		Card? card = playtable.cardFactory.GetCard(id);

		if(card == null)
		{
			UnityLogger.LogError($"Unable to find card of Id - {id}");
			throw new Exception($"Unable to find card of Id - {id}");
		}
		return card;
	}

	public Transform GetCardImage(int cardId)
	{
		if(idToCardTransform.ContainsKey(cardId))
		{
			return Instantiate(idToCardTransform[cardId]);
		}
		Card card = GetCard(cardId);
		Debug.Log($"CARD - {card.name.Value}");
		Transform newCardImage = Instantiate(cardImagePrefab);
		newCardImage.GetComponent<CardDrag>().cardId = cardId;

		newCardImage.name = cardId.ToString();
		newCardImage.transform.position = new Vector3(1000,1000,1000);
		GenericCardComponents components = newCardImage.GetComponent<GenericCardComponents>();
		components.Setup(card);
		Debug.Log($"CARD - {components.card.name.Value}");
		// Pull new texture at some point
		return Instantiate(newCardImage);
	}
}