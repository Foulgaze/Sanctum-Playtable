 using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoveCardMenu : MonoBehaviour
{
    [SerializeField] private Button moveToGraveyardBtn;
    [SerializeField] private Button moveToExileBtn;
    [SerializeField] private Button moveToTopBtn;
    [SerializeField] private Button moveToBottomBtn;
    [SerializeField] private Button closeBtn;
    [SerializeField] private TMP_InputField countInputField;

    void HandleButtonPress(Action<int> buttonAction)
    {
        if(string.IsNullOrEmpty(countInputField.text))
        {
            Destroy(this.gameObject);
            return;
        }
        buttonAction(int.Parse(countInputField.text));
        Destroy(this.gameObject);
    }

    public void Setup(Action graveyardAction, Action exileAction, Action<int> topBtn, Action<int> bottomBtn)
    {
        moveToGraveyardBtn.onClick.AddListener(() => {graveyardAction();Destroy(this.gameObject);});
        moveToExileBtn.onClick.AddListener(() => {exileAction(); Destroy(this.gameObject);});
        moveToTopBtn.onClick.AddListener(() => {HandleButtonPress(topBtn);});
        moveToBottomBtn.onClick.AddListener(() => {HandleButtonPress(bottomBtn);});
        closeBtn.onClick.AddListener(() => Destroy(this.gameObject));
    }
}
