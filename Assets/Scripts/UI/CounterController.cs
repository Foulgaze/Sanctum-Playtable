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
        SetCounterValues();
        InitializeButtons();
        InitializeListeners();
    }

    private void InitializeButtons()
    {
        redPlusBtn.onClick.AddListener(() => card.changeCounters.SetValue((1,0,0)));
        redMinusBtn.onClick.AddListener(() => card.changeCounters.SetValue((-1,0,0)));
        greenPlusBtn.onClick.AddListener(() => card.changeCounters.SetValue((0,1,0)));
        greenMinusBtn.onClick.AddListener(() => card.changeCounters.SetValue((0,-1,0)));
        bluePlusBtn.onClick.AddListener(() => card.changeCounters.SetValue((0,0,1)));
        blueMinusBtn.onClick.AddListener(() => card.changeCounters.SetValue((0,0,-1)));
        closeBtn.onClick.AddListener(() => Destroy(this.gameObject));
    }

    private void InitializeListeners()
    {
        card.redCounters.nonNetworkChange += HandleCounterChange;
        card.greenCounters.nonNetworkChange += HandleCounterChange;
        card.blueCounters.nonNetworkChange += HandleCounterChange;
        redInputField.onEndEdit.AddListener((value) => UpdateCounter(value, card.redCounters));
        greenInputField.onEndEdit.AddListener((value) => UpdateCounter(value, card.greenCounters));
        blueInputField.onEndEdit.AddListener((value) => UpdateCounter(value, card.blueCounters));
    }

    private void UpdateCounter(string inputValue, NetworkAttribute attribute)
    {
        int value = string.IsNullOrEmpty(inputValue) ? 0: int.Parse(inputValue);
        attribute.SetValue(value);
    }
    private void SetCounterValues()
    {
        redInputField.text = card.redCounters.Value.ToString();
        greenInputField.text = card.greenCounters.Value.ToString();
        blueInputField.text = card.blueCounters.Value.ToString();
    }

    private void HandleCounterChange(NetworkAttribute _)
    {
        SetCounterValues();
    }
    
    private void OnDestroy()
    {
        card.redCounters.nonNetworkChange -= HandleCounterChange;
        card.greenCounters.nonNetworkChange -= HandleCounterChange;
        card.blueCounters.nonNetworkChange -= HandleCounterChange;
    }


}
