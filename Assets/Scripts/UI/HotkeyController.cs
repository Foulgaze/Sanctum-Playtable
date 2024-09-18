using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sanctum_Core;
using Unity.VisualScripting;
using UnityEngine;

public class HotkeyController : MonoBehaviour
{
    public Player? clientPlayer = null;
    public static bool IsCtrlDown()
    {
        return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
    }
    private bool CheckForCtrlPlusKey(KeyCode key)
    {
        return IsCtrlDown() && Input.GetKeyDown(key);
    }

    private void TapUntapCard()
    {
        IDraggable draggable = DragController.Instance.RaycastForDraggable();
        if(draggable is not CardDrag)
        {
            return;
        }
        CardDrag cardDrag = (CardDrag) draggable;
        CardZone cardZone = CardFactory.Instance.GetCardZone(cardDrag.cardId);
        var fieldZones = new CardZone[]{CardZone.MainField, CardZone.LeftField, CardZone.RightField};
        if(cardDrag.isOpponent || !fieldZones.Contains(cardZone))
        {
            return;
        }
        Card card = CardFactory.Instance.GetCard(cardDrag.cardId);
        card.isTapped.SetValue(!card.isTapped.Value);
    }

    private void Update()
    {
        if(clientPlayer == null)
        {
            return;
        }
        if(CheckForCtrlPlusKey(KeyCode.D))
        {
            GameOrchestrator.Instance.SendSpecialAction(SpecialAction.Draw, 1.ToString());
        }
        if(CheckForCtrlPlusKey(KeyCode.S))
        {
            GameOrchestrator.Instance.SendSpecialAction(SpecialAction.Shuffle,string.Empty);
        }
        if(CheckForCtrlPlusKey(KeyCode.M))
        {
            GameOrchestrator.Instance.SendSpecialAction(SpecialAction.Mulligan, string.Empty);
        }
        if(IsCtrlDown() && Input.GetMouseButtonDown((int)MouseButton.Left))
        {
            TapUntapCard();
        }
    }
}
