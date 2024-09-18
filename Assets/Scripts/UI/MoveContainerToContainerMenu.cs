using System.Collections;
using System.Collections.Generic;
using Sanctum_Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoveContainerToContainerMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI windowText;
    [SerializeField] private Button topOfLibraryBtn;
    [SerializeField] private Button bottomOfLibraryBtn;
    [SerializeField] private Button graveyardBtn;
    [SerializeField] private Button exileBtn;
    [SerializeField] private Button closeBtn;

    public void Setup(CardZone zone)
    {
        windowText.text = $"Move Cards from {zone} to";
        SetupButtons(zone);
    }

    private void SetupButtons(CardZone zone)
    {
        topOfLibraryBtn.onClick.AddListener(() => GameOrchestrator.Instance.SendSpecialAction(SpecialAction.MoveContainerCardsTo, $"{(int)zone}|{(int)CardZone.Library}"));
        topOfLibraryBtn.onClick.AddListener(() => Destroy(gameObject));
        bottomOfLibraryBtn.onClick.AddListener(() => GameOrchestrator.Instance.SendSpecialAction(SpecialAction.MoveContainerCardsTo, $"{(int)zone}|{(int)CardZone.Library}|bottom"));
        bottomOfLibraryBtn.onClick.AddListener(() => Destroy(gameObject));
        if(zone != CardZone.Graveyard)
        {
            graveyardBtn.onClick.AddListener(() => GameOrchestrator.Instance.SendSpecialAction(SpecialAction.MoveContainerCardsTo, $"{(int)zone}|{(int)CardZone.Graveyard}"));
        }
        graveyardBtn.onClick.AddListener(() => Destroy(gameObject));
        if(zone != CardZone.Exile)
        {
            exileBtn.onClick.AddListener(() => GameOrchestrator.Instance.SendSpecialAction(SpecialAction.MoveContainerCardsTo, $"{(int)zone}|{(int)CardZone.Exile}"));
        }
        exileBtn.onClick.AddListener(() => Destroy(gameObject));
        closeBtn.onClick.AddListener(() => Destroy(gameObject));
    }
}
