using System.Collections;
using System.Collections.Generic;
using Sanctum_Core;
using UnityEngine;
public interface ITextureable
{
    public void TextureSelf(CardInfo info, Sprite sprite);
    public Card GetCard();
}