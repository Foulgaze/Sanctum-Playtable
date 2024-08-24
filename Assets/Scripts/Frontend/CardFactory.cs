
using System.Collections.Generic;
using UnityEngine;

public class CardFactory : MonoBehaviour
{
	public static CardFactory Instance { get; private set; }
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

	private Dictionary<int, Transform> idToCardTransform = new();
	[SerializeField] Transform cardPrefab;
	public static Transform GetCardTransform(int cardId)
	{
		if(idToCardTransform.ContainsKey(cardId))
		{
			return idToCardTransform[cardId];
		}
		Transform newCard = GameObject.Instantiate(cardPrefab);
		return null;

	}

	public static Transform GetCardImage()
	{
		return null;
	}

}