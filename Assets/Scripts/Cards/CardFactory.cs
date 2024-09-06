
using System;
using System.Collections.Generic;
using Sanctum_Core;
using UnityEngine;
using UnityEngine.UI;

public class CardFactory : MonoBehaviour
{
	public static CardFactory Instance { get; private set; }
	public Transform cardOnFieldPrefab;
	public Transform cardPilePrefab;
	public Transform cardImagePrefab;
	public Playtable playtable;
	public readonly Dictionary<string, Sprite> fileNameToSprite = new();
	
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

	void Start()
	{
		Sprite[] sprites = Resources.LoadAll<Sprite>("Colors");
		foreach(Sprite sprite in sprites)
		{
			fileNameToSprite[sprite.name] = sprite;
		}
	}
	public Transform GetCardOnField(int cardId, bool isOpponentCard)
	{
		Transform newCard = GameObject.Instantiate(cardOnFieldPrefab);
		Transform cardImage = newCard.GetChild(0).GetChild(0); // SIGH i hate doing getchild
		cardImage.GetComponent<CardDrag>().cardId = cardId;
		if(isOpponentCard)
		{
			cardImage.GetComponent<Image>().raycastTarget = false;
		}
		Card card = GetCard(cardId);
		CardOnFieldComponents components = newCard.GetComponent<CardOnFieldComponents>();
		components.Setup(card);
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

	public Transform GetCardImage(int cardId, bool isOpponentCard, bool renderCardBack = false)
	{
		Card card = GetCard(cardId);
		Transform newCardImage = Instantiate(cardImagePrefab);
		newCardImage.GetComponent<CardDrag>().cardId = cardId;
		if(isOpponentCard)
		{
			newCardImage.GetComponent<Image>().raycastTarget = false;
		}

		newCardImage.name = cardId.ToString();
		newCardImage.transform.position = new Vector3(1000,1000,1000);
		GenericCardComponents components = newCardImage.GetComponent<GenericCardComponents>();
		components.Setup(card, renderCardBack);
		return newCardImage;
	}
}