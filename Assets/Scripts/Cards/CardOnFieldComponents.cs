using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sanctum_Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardOnFieldComponents : MonoBehaviour, ITextureable
{
    public Card card;
    [SerializeField] private TextMeshProUGUI powerToughess;
    [SerializeField] private TextMeshProUGUI name;
    public Image cardImage;
    [SerializeField] private Transform tappedSymbol;
    public Image backgroundImage;

    private Vector3 tappedRotation = new Vector3(0,7f,0);

    [SerializeField] private TextMeshProUGUI redCounterText;
    [SerializeField] private TextMeshProUGUI greenCounterText;
    [SerializeField] private TextMeshProUGUI blueCounterText;

    public void Setup(Card card)
    {
        this.card = card;
        SetupAttributes(null);
        SetupListeners();
        SetupCounters(null);
    }
    public void Setup()
    {
        SetPowerToughnessStatus();
        
    }
    
    private void SetupListeners()
    {
        card.isTapped.nonNetworkChange += TapUntapCard;
        card.isFlipped.nonNetworkChange += SetupAttributes;
        card.isUsingBackSide.nonNetworkChange += SetupAttributes;
        card.power.nonNetworkChange += SetupPT;
        card.toughness.nonNetworkChange += SetupPT;
        card.redCounters.nonNetworkChange += SetupCounters;
        card.greenCounters.nonNetworkChange += SetupCounters;
        card.blueCounters.nonNetworkChange += SetupCounters;
    }

    private void SetupCounters(NetworkAttribute _)
    {
        UpdateCounter(redCounterText, card.redCounters.Value);
        UpdateCounter(blueCounterText, card.blueCounters.Value);
        UpdateCounter(greenCounterText, card.greenCounters.Value);
    }

    private void UpdateCounter(TextMeshProUGUI counterText, int counterValue)
    {
        counterText.text = counterValue.ToString();
        counterText.transform.parent.gameObject.SetActive(counterValue > 0);
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
        string potentialFileName = string.Join("", new HashSet<string>(matchingChars));

        if(card.isFlipped.Value)
        {
            potentialFileName = "default";
        }

        Sprite backgroundColor = CardFactory.Instance.fileNameToSprite.ContainsKey(potentialFileName) ? CardFactory.Instance.fileNameToSprite[potentialFileName]  : CardFactory.Instance.fileNameToSprite["default"];
        backgroundImage.sprite = backgroundColor;
    }

    private void TapUntapCard(NetworkAttribute _)
    {
        tappedSymbol.gameObject.SetActive(card.isTapped.Value);
        if(card.isTapped.Value)
        {
            transform.rotation = Quaternion.Euler(tappedRotation);
        }
        else
        {
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }
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
    
    public void SetPowerToughnessStatus()
    {
        powerToughess.transform.parent.gameObject.SetActive(EnablePowerToughess());
    }
    private void SetupAttributes(NetworkAttribute _)
    {
        SetupPT(null);
        SetPowerToughnessStatus();
        name.text = card.isFlipped.Value ? string.Empty : card.CurrentInfo.name;
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
        card.redCounters.nonNetworkChange -= SetupCounters;
        card.greenCounters.nonNetworkChange -= SetupCounters;
        card.blueCounters.nonNetworkChange -= SetupCounters;
    }
}
