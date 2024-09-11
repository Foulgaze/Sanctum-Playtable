
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
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
	public readonly Dictionary<int, Queue<Transform>> uuidToCardOnField = new();
	public readonly Dictionary<int, Queue<Transform>> uuidToCardImage = new();
	public readonly Dictionary<int, CardZone> cardIdToContainer = new();
	[SerializeField] Transform disposedCardParent;

	
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

	public void DisposeOfCard(int cardId, Transform cardTransform, bool onField)
	{
		var disposeDict = onField ? uuidToCardOnField : uuidToCardImage;
		cardTransform.SetParent(disposedCardParent);
		if(!disposeDict.ContainsKey(cardId))
		{
			disposeDict[cardId] = new Queue<Transform>();
		}
		disposeDict[cardId].Enqueue(cardTransform);
	}

	public Transform GetCardOnField(int cardId, bool isOpponentCard)
	{

		bool queueHasCard = uuidToCardOnField.ContainsKey(cardId) && uuidToCardOnField[cardId].Count != 0;
		Transform cardOnField = queueHasCard ? uuidToCardOnField[cardId].Dequeue() : Instantiate(cardOnFieldPrefab);
		
		CardOnFieldComponents components = cardOnField.GetComponent<CardOnFieldComponents>();
		components.backgroundImage.raycastTarget = !isOpponentCard;
		
		if(!queueHasCard)
		{
			
			components.backgroundImage.GetComponent<CardDrag>().cardId = cardId;
			Card card = GetCard(cardId);
			components.Setup(card);
		}

		return cardOnField;
	}

	private Card GetCard(int id)
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
		
		bool queueHasCard = uuidToCardImage.ContainsKey(cardId) && uuidToCardImage[cardId].Count != 0;

		Transform cardImage = queueHasCard ? uuidToCardImage[cardId].Dequeue() : Instantiate(cardImagePrefab);
		GenericCardComponents components = cardImage.GetComponent<GenericCardComponents>();
		if(queueHasCard)
		{
			components.RenderCardImage(renderCardBack);
		}
		else
		{
			cardImage.GetComponent<CardDrag>().cardId = cardId;
			cardImage.name = cardId.ToString();
			components.Setup(card, renderCardBack);
		}
		
		cardImage.GetComponent<Image>().raycastTarget = !isOpponentCard;
		return cardImage;
	}

	public CardZone GetCardZone(int cardId)
	{
		Card? card = playtable.cardFactory.GetCard(cardId);
		if(card == null)
		{
			throw new Exception($"Could not find card of id - {cardId}");
		}
		return cardIdToContainer[cardId];
	}

	public void SetCardZone(int cardId, CardZone zone)
	{
		cardIdToContainer[cardId] = zone;
	}

	public void SetCardFlip(int cardId, bool isUpsideDown)
	{
		Card card = GetCard(cardId);
		card.isFlipped.SetValue(isUpsideDown);
	}
}