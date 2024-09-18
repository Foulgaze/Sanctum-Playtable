
using System;
using System.Collections.Generic;
using System.IO;
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
	[SerializeField] private Transform disposedCardParent;
	private Dictionary<string, List<string>> nameToRelatedCards;
	private Dictionary<string, string> uuidToName;

	
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
		string relatedTokensPath = $"{Application.streamingAssetsPath}/relatedTokens.txt";
		string uuidToNamePath = $"{Application.streamingAssetsPath}/uuidToName.txt";
		nameToRelatedCards = JsonConvert.DeserializeObject<Dictionary<string,List<string>>>(File.ReadAllText(relatedTokensPath));
		uuidToName = JsonConvert.DeserializeObject<Dictionary<string,string>>(File.ReadAllText(uuidToNamePath));
		Sprite[] sprites = Resources.LoadAll<Sprite>("Colors");
		foreach(Sprite sprite in sprites)
		{
			fileNameToSprite[sprite.name] = sprite;
		}
	}

	private void LoadRelatedTokens(string path)
	{
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
		cardOnField.GetChild(0).GetChild(0).GetComponent<CardDrag>().isOpponent = isOpponentCard;
		
		if(!queueHasCard)
		{
			components.backgroundImage.GetComponent<CardDrag>().cardId = cardId;
			Card card = GetCard(cardId);
			components.Setup(card);
		}
		else
		{
			components.Setup();
		}

		return cardOnField;
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
		
		bool queueHasCard = uuidToCardImage.ContainsKey(cardId) && uuidToCardImage[cardId].Count != 0;

		Transform cardImage = queueHasCard ? uuidToCardImage[cardId].Dequeue() : Instantiate(cardImagePrefab);
		GenericCardComponents components = cardImage.GetComponent<GenericCardComponents>();
		if(queueHasCard)
		{
			components.renderCardBack = renderCardBack;
			components.RenderCardImage(renderCardBack);
		}
		else
		{
			cardImage.GetComponent<CardDrag>().cardId = cardId;
			cardImage.name = cardId.ToString();
			components.Setup(card, renderCardBack);
		}
		
		cardImage.GetComponent<CardDrag>().isOpponent = isOpponentCard;
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

	public List<(string,string)>? GetRelatedCardsUUIDNamePair(int cardId)
	{
		Card card = GetCard(cardId);
		if(!nameToRelatedCards.ContainsKey(card.CurrentInfo.name))
		{
			return null;
		}
		List<(string,string)> uuidNamePairs = new();
		foreach(string uuid in nameToRelatedCards[card.CurrentInfo.name])
		{
			uuidNamePairs.Add((uuid, uuidToName[uuid]));
		}
		return uuidNamePairs;
	}

	

}