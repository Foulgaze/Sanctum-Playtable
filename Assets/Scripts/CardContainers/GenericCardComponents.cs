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

    void Start()
    {
        // UnityLogger.Log($"Card Name : [{card.Id}]");
        // card.isUsingBackSide.nonNetworkChange += Setup;
    }

    public void Setup(Card card)
    {
        this.card = card;
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
		// TextureController.Instance.TextureImage(this);
    }

    public void TextureSelf(CardInfo info, Sprite sprite)
    {
        if(info == card.CurrentInfo)
        {
            cardImage.sprite = sprite;
            enabled = true;
        }
    }

    public Card GetCard()
    {
        return card;
    }
}
