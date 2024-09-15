using System.Collections;
using System.Collections.Generic;
using Sanctum_Core;
using UnityEngine;
using UnityEngine.UI;

public class cardIdentifier : MonoBehaviour, ITextureable
{
    private Card? currentHoverCard;
    [SerializeField] Image hoveredCardImage;
    public Player clientPlayer;
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

        // Case 2. On opponents library AND flipped over OR Client library And Flipped over
        bool inLibrary = CardFactory.Instance.GetCardZone(cardDrag.cardId) == CardZone.Library;
        bool hasOpponents = GameOrchestrator.Instance.opponentRotator.opponentUUIDs.Count != 0;
        bool opponentTopCardFlipped = hasOpponents && GameOrchestrator.Instance.opponentRotator.GetCurrentOpponent().GetCardContainer(CardZone.Library).revealTopCard.Value;
        bool clientTopCardFlipped = clientPlayer.GetCardContainer(CardZone.Library).revealTopCard.Value;
        if (inLibrary && ((!cardDrag.isOpponent && !clientTopCardFlipped) || (cardDrag.isOpponent && !opponentTopCardFlipped)))
        {
            return null;
        }

        return hoveredCard;
        // Case 3???
    }
    private void RaycastForCard()
    {
        IDraggable? draggable = DragController.Instance.RaycastForDraggable();
        if(draggable == null || draggable is not CardDrag)
        {
            return;
        }
        Card? hoveredCard = GetHoveredCard((CardDrag)draggable);
        if(hoveredCard == null)
        {
            return;
        }
        currentHoverCard = hoveredCard;
        TextureController.Instance.TextureImage(this);
    
    }
    // Update is called once per frame
    void Update()
    {
        RaycastForCard();
    }
}
