using System.Collections.Generic;
using System.Text.RegularExpressions;
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
    public Image backgroundImage;

    public void Setup(Card card)
    {
        this.card = card;
        card.isTapped.nonNetworkChange += SetupAttributes;
        card.isFlipped.nonNetworkChange += SetupAttributes;
        card.power.nonNetworkChange += SetupAttributes;
        card.toughness.nonNetworkChange += SetupAttributes;
        card.isUsingBackSide.nonNetworkChange += SetupAttributes;
        enablePT = EnablePowerToughess();
        if(!card.isFlipped.Value)
        {
		    SetupAttributes(null);
        }
        else
        {
            RenderCard();
        }
    }

    private void SetupBackground()
    {
        string pattern = @"\{([GRBWU])\}";

        MatchCollection matches = Regex.Matches(card.CurrentInfo.manaCost, pattern);

        List<string> matchingChars = new();

        foreach (Match match in matches)
        {
            matchingChars.Add(match.Groups[1].Value);
        }

        matchingChars.Sort();

        string potentialFileName = string.Join("", matchingChars);
        Sprite backgroundColor = CardFactory.Instance.fileNameToSprite.ContainsKey(potentialFileName) ? CardFactory.Instance.fileNameToSprite[potentialFileName]  : CardFactory.Instance.fileNameToSprite["default"];
        backgroundImage.sprite = backgroundColor;
    }

    private bool EnablePowerToughess()
	{
		return card.CurrentInfo.power != string.Empty || card.CurrentInfo.toughness != string.Empty;
	}
    

    private void SetupAttributes(NetworkAttribute _)
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
        RenderCard();
        SetupBackground();

		tappedSymbol.gameObject.SetActive(card.isTapped.Value);
        // ROtate some amount
    }

    public void RenderCard()
    {
        if(!card.isFlipped.Value)
        {
		    TextureController.Instance.TextureImage(this);
        }
        else
        {
            TextureController.Instance.TextureBackOfCard(this);
        }
    }

    public void TextureSelf(CardInfo info, Sprite sprite)
    {
        if(info.name == card.CurrentInfo.name)
        {
            cardImage.sprite = sprite;

        }
    }

    public Card GetCard()
    {
        return card;
    }

    private void OnDestroy()
    {
        card.isTapped.nonNetworkChange -= SetupAttributes;
        card.isFlipped.nonNetworkChange -= SetupAttributes;
        card.power.nonNetworkChange -= SetupAttributes;
        card.toughness.nonNetworkChange -= SetupAttributes;
        card.isUsingBackSide.nonNetworkChange -= SetupAttributes;
    }
}
