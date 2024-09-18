// using System.Collections;
// using System.Collections.Generic;
// using Sanctum_Core;
// using UnityEngine;

// public class InGameCard : MonoBehaviour, ITextureable
// {
//     public Card card;
//     public TextMeshProUGUI name;
//     public Image cardImage;
//     public bool renderCardBack = false;

//     void Start()
//     {
//         card.isUsingBackSide.nonNetworkChange += SetAttributes;
//     }

//     public void Setup(Card card, bool renderCardBack)
//     {
//         this.card = card;
//         this.renderCardBack = renderCardBack;
//         SetAttributes(null);
//     }

//     private void SetAttributes(NetworkAttribute? _)
//     {
//         if(RenderCardImage())
//         {
//             SetComponentState(false);
//             return;
//         }
//         SetComponentState(true);
// 		name.text = card.name.Value;
//         cardImage.enabled = false;
        
//     }

//     public bool RenderCardImage(bool? renderBack = null)
//     {
//         if(renderBack != null)
//         {
//             renderCardBack = (bool)renderBack || card.isFlipped.Value;
//         }
    
//         if(renderCardBack)
//         {
//             SetComponentState(false);
//             return TextureController.Instance.TextureBackOfCard(this);
//         }
//         else
//         {
//             SetComponentState(false);
// 		    return TextureController.Instance.TextureImage(this);
//         }
//     }

//     private void SetComponentState(bool state)
//     {
//         name.transform.parent.gameObject.SetActive(state);
//         manaCost.transform.parent.gameObject.SetActive(state);
//         type.transform.parent.gameObject.SetActive(state);
//         description.transform.parent.gameObject.SetActive(state);
//     }

//     public void TextureSelf(CardInfo info, Sprite sprite)
//     {
//         if(info.name == card.CurrentInfo.name)
//         {
//             SetComponentState(false);
//             cardImage.sprite = sprite;
//             cardImage.enabled = true;
//         }
//     }

//     public Card GetCard()
//     {
//         return card;
//     }

//     private void OnDestroy()
//     {
//         card.isUsingBackSide.nonNetworkChange -= SetAttributes;
//     }

//     public void TextureSelf(CardInfo info, Sprite sprite)
//     {
//         throw new System.NotImplementedException();
//     }

//     Card ITextureable.GetCard()
//     {
//         throw new System.NotImplementedException();
//     }
// }
