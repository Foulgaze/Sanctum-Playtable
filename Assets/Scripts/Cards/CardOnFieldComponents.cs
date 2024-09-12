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
        SetupAttributes(null);
        SetupListeners();
    }
    
    private void SetupListeners()
    {
        card.isTapped.nonNetworkChange += TapUntapCard;
        card.isFlipped.nonNetworkChange += SetupAttributes;
        card.isUsingBackSide.nonNetworkChange += SetupAttributes;
        card.power.nonNetworkChange += SetupPT;
        card.toughness.nonNetworkChange += SetupPT;
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

        if(card.isFlipped.Value)
        {
            potentialFileName = "default";
        }
        print($"Matching name {potentialFileName} - {card.CurrentInfo.manaCost}");

        Sprite backgroundColor = CardFactory.Instance.fileNameToSprite.ContainsKey(potentialFileName) ? CardFactory.Instance.fileNameToSprite[potentialFileName]  : CardFactory.Instance.fileNameToSprite["default"];
        backgroundImage.sprite = backgroundColor;
    }

    private void TapUntapCard(NetworkAttribute _)
    {
        tappedSymbol.gameObject.SetActive(card.isTapped.Value);
    }
    private void SetupPT(NetworkAttribute _)
    {
		powerToughess.text = $"{card.power.Value}/{card.toughness.Value}";
        powerToughess.transform.parent.gameObject.SetActive(true);
    }
    private bool EnablePowerToughess()
	{
        bool hasDefaultPT = card.CurrentInfo.power != string.Empty || card.CurrentInfo.toughness != string.Empty; 
		return hasDefaultPT && !card.isFlipped.Value;
	}
    

    private void SetupAttributes(NetworkAttribute _)
    {
        SetupPT(null);
        powerToughess.transform.parent.gameObject.SetActive(EnablePowerToughess());
        name.text = card.isFlipped.Value ? string.Empty : card.name.Value;
        RenderCardImage();
        SetupBackground();
        TapUntapCard(null);
    }

    public void RenderCardImage()
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
        card.isTapped.nonNetworkChange -= TapUntapCard;
        card.isFlipped.nonNetworkChange -= SetupAttributes;
        card.isUsingBackSide.nonNetworkChange -= SetupAttributes;
        card.power.nonNetworkChange -= SetupPT;
        card.toughness.nonNetworkChange -= SetupPT;
    }
}
