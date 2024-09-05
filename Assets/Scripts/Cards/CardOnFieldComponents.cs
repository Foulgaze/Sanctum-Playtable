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
        SetupAttributes(null);
        SetupBackground();
    }

    private void SetupBackground()
    {
        string pattern = @"\{([GRBWU])\}";

        // Find all matches in the input string
        MatchCollection matches = Regex.Matches(card.CurrentInfo.manaCost, pattern);

        List<string> matchingChars = new();

        // Iterate over each match and extract the value
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
		TextureController.Instance.TextureImage(this);
		tappedSymbol.gameObject.SetActive(card.isTapped.Value);
        // ROtate some amount
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
}
