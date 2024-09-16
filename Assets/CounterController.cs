using System.Collections;
using System.Collections.Generic;
using Sanctum_Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CounterController : MonoBehaviour
{
    [SerializeField] private Button redPlusBtn;
    [SerializeField] private Button redMinusBtn;
    [SerializeField] private TMP_InputField redInputField;

    [SerializeField] private Button greenPlusBtn;
    [SerializeField] private Button greenMinusBtn;
    [SerializeField] private TMP_InputField greenInputField;

    [SerializeField] private Button bluePlusBtn;
    [SerializeField] private Button blueMinusBtn;
    [SerializeField] private TMP_InputField blueInputField;
    [SerializeField] private Button closeBtn;
    
    private Card card;

    public void Setup(Card card)
    {
        this.card = card;
        redInputField.text = card..Value.ToString();
        toughnessInputField.text = card.toughness.Value.ToString();
        InitializeListeners();
    }


}
