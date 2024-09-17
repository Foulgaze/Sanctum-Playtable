using System;
using System.Collections;
using System.Collections.Generic;
using Sanctum_Core;
using UnityEngine;

public class HotkeyController : MonoBehaviour
{
    public Player? clientPlayer = null;
    private bool CheckForCtrlPlusKey(KeyCode key)
    {
        bool ctrlIsDown = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        return ctrlIsDown && Input.GetKeyDown(key);
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
    }
}
