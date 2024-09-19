using System.Collections;
using System.Collections.Generic;
using Sanctum_Core;
using UnityEngine;
using UnityEngine.UI;

public class CardIdentifier : MonoBehaviour, ITextureable
{
    private Card? currentHoverCard;
    [SerializeField] Image hoveredCardImage;
    public Player clientPlayer;
    [HideInInspector] public Image? currentlyHeldCardImage;
     public static CardIdentifier Instance;
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

    public Card GetCard()
    {
        return currentHoverCard;
    }

    public void TextureSelf(CardInfo info, Sprite sprite)
    {
        if(currentHoverCard == null || currentHoverCard.CurrentInfo != info)
        {
            return;
        }
        hoveredCardImage.sprite = sprite;
    }

    private Card? GetHoveredCard(CardDrag cardDrag)
    {
        Card hoveredCard = CardFactory.Instance.GetCard(cardDrag.cardId);
        
        // Case 1. Flipped over AND Opponents
        if(cardDrag.isOpponent && hoveredCard.isFlipped.Value) 
        {
            return null;
        }


        if(cardDrag.renderCardBack)
        {
            return null;
        }
        return hoveredCard;
    }
    private void RaycastForCard()
    {
        IDraggable? draggable = DragController.Instance.RaycastForDraggable();
        if(draggable == null || draggable is not CardDrag)
        {
            SetHeldCardOpacity(1f);
            return;
        }
        SetHeldCardOpacity(0.5f);
        Card? hoveredCard = GetHoveredCard((CardDrag)draggable);
        if(hoveredCard == null || hoveredCard == currentHoverCard)
        {
            return;
        }
        currentHoverCard = hoveredCard;
    
        TextureController.Instance.TextureImage(this);
    }

    public void SetHeldCardOpacity(float opacity)
    {
        if(currentlyHeldCardImage == null)
        {
            return;
        }
        Color color = currentlyHeldCardImage.color;
        color.a = opacity;  
        currentlyHeldCardImage.color = color;
    }
    // Update is called once per frame
    void Update()
    {
        RaycastForCard();
    }
}
