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
        card.isUsingBackSide.nonNetworkChange += SetAttributes;
    }

    public void Setup(Card card, bool renderCardBack)
    {
        this.card = card;
        this.renderCardBack = renderCardBack;
        SetAttributes(null);
    }

    private void SetAttributes(NetworkAttribute? _)
    {
        if(RenderCardImage())
        {
            SetComponentState(false);
            return;
        }
        SetComponentState(true);
		name.text = card.name.Value;
        manaCost.text = card.CurrentInfo.manaCost;
        type.text = card.CurrentInfo.type;
        description.text = card.CurrentInfo.text;
        cardImage.enabled = false;
        
    }

    public bool RenderCardImage(bool? renderBack = null)
    {
        if(renderBack != null)
        {
            if(renderCardBack == renderBack)
            {
                return true;
            }
            renderCardBack = (bool)renderBack;
        }
    
        if(renderCardBack)
        {
            return TextureController.Instance.TextureBackOfCard(this);
        }
        else
        {
		    return TextureController.Instance.TextureImage(this);
        }
    }

    private void SetComponentState(bool state)
    {
        name.gameObject.SetActive(state);
        manaCost.gameObject.SetActive(state);
        type.gameObject.SetActive(state);
        description.gameObject.SetActive(state);
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
