using System.Collections;
using System.Collections.Generic;
using Sanctum_Core;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GenericCardComponents : MonoBehaviour, ITextureable
{
    public Card card;
    public TextMeshProUGUI name;
    public TextMeshProUGUI manaCost;
    public TextMeshProUGUI type;
    public TextMeshProUGUI description;
    public Image cardImage;
    public bool renderCardBack = false;

    void Start()
    {
        // UnityLogger.Log($"Card Name : [{card.Id}]");
        // card.isUsingBackSide.nonNetworkChange += Setup;
    }

    public void Setup(Card card, bool renderCardBack)
    {
        this.card = card;
        this.renderCardBack = renderCardBack;
        card.isUsingBackSide.nonNetworkChange += SetAttributes;
        SetAttributes(null);
    }

    private void SetAttributes(NetworkAttribute? _)
    {
		name.text = card.name.Value;
        manaCost.text = card.CurrentInfo.manaCost;
        type.text = card.CurrentInfo.type;
        description.text = card.CurrentInfo.text;
        // cardImage.enabled = false;
        if(renderCardBack)
        {
            TextureController.Instance.TextureBackOfCard(this);
        }
        else
        {
		    TextureController.Instance.TextureImage(this);
        }
    }

    public void TextureSelf(CardInfo info, Sprite sprite)
    {
        if(info.name == card.CurrentInfo.name)
        {

            cardImage.sprite = sprite;
            cardImage.enabled = true;
        }
    }

    public Card GetCard()
    {
        return card;
    }
}
