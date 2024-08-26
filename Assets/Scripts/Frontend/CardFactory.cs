
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
		Card card = GetCard(cardId);
		SetupCardOnField(card, newCard);
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

		Transform newCardImage = Instantiate(cardImagePrefab);
		newCardImage.transform.position = new Vector3(1000,1000,1000);
		SetupCardForHandOrPile(card, newCardImage);
		// Pull new texture at some point
		return newCardImage;
	}

	// To Do
	// Make this work with power toughness
	private void SetupCardForHandOrPile(Card card, Transform newCard)
	{
		GenericCardComponents components = newCard.GetComponent<GenericCardComponents>();
		components.name.text = card.name.Value;
		components.manaCost.text = card.CurrentInfo.manaCost;
		components.type.text = card.CurrentInfo.type;
		components.description.text = card.CurrentInfo.text;
	}

	private bool EnablePowerToughess(Card card)
	{
		return card.CurrentInfo.power != string.Empty || card.CurrentInfo.toughness != string.Empty;
	}

	private void SetupCardOnField(Card card, Transform newCard)
	{
		CardOnFieldComponents components = newCard.GetComponent<CardOnFieldComponents>();
		components.name.text = card.name.Value;
		if(EnablePowerToughess(card))
		{
			components.powerToughess.text = $"{card.power.Value}/{card.toughness.Value}";
			components.powerToughess.transform.parent.gameObject.SetActive(true);
		}
		else
		{
			components.powerToughess.transform.parent.gameObject.SetActive(false);
		}
		// components.cardImage
		components.tappedSymbol.gameObject.SetActive(card.isTapped.Value);
	}

}