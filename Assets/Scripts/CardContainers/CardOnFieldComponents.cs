using System.Collections;
using System.Collections.Generic;
using Sanctum_Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardOnFieldComponents : MonoBehaviour, ITextureable
{
    public Card card;
    public bool enablePT;
    public TextMeshProUGUI powerToughess;
    public TextMeshProUGUI name;
    public Image cardImage;
    public Transform tappedSymbol;

    void Start()
    {
        card.isTapped.nonNetworkChange += Setup;
        card.isFlipped.nonNetworkChange += Setup;
        card.power.nonNetworkChange += Setup;
        card.toughness.nonNetworkChange += Setup;
        card.isUsingBackSide.nonNetworkChange += Setup;
        enablePT = EnablePowerToughess();

    }

    private bool EnablePowerToughess()
	{
		return card.CurrentInfo.power != string.Empty || card.CurrentInfo.toughness != string.Empty;
	}

    public void Setup(NetworkAttribute _)
    {
		name.text = card.name.Value;
		if(enablePT)
		{
			powerToughess.text = $"{card.power.Value}/{card.toughness.Value}";
			powerToughess.transform.parent.gameObject.SetActive(true);
		}
		else
		{
			powerToughess.transform.parent.gameObject.SetActive(false);
		}
		TextureController.Instance.TextureImage(this);
		tappedSymbol.gameObject.SetActive(card.isTapped.Value);
        // ROtate some amount
    }

    public void TextureSelf(CardInfo info, Sprite sprite)
    {
        if(info == card.CurrentInfo)
        {
            cardImage.sprite = sprite;
        }
    }

    public Card GetCard()
    {
        return card;
    }
}
