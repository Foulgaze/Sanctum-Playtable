using System.Collections;
using System.Collections.Generic;
using Sanctum_Core;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class powerToughenssController : MonoBehaviour
{
    [SerializeField] private Button powerPlusBtn;
    [SerializeField] private Button powerMinusBtn;
    [SerializeField] private TMP_InputField powerInputField;

    [SerializeField] private Button toughnessPlusBtn;
    [SerializeField] private Button toughnessMinusBtn;
    [SerializeField] private TMP_InputField toughnessInputField;
    [SerializeField] private Button closeBtn;

    private Card card;

    public void Setup(Card card)
    {
        this.card = card;
        powerInputField.text = card.power.Value.ToString();
        toughnessInputField.text = card.toughness.Value.ToString();
        InitializeListeners();
    }
    private void InitializeListeners()
    {
        powerInputField.onEndEdit.AddListener((_) => card.power.SetValue(int.Parse(powerInputField.text)));
        toughnessInputField.onEndEdit.AddListener((_) => card.toughness.SetValue(int.Parse(toughnessInputField.text)));
        powerPlusBtn.onClick.AddListener(() => card.isIncreasingPower.SetValue(true));
        powerMinusBtn.onClick.AddListener(() => card.isIncreasingPower.SetValue(false));
        toughnessPlusBtn.onClick.AddListener(() => card.isIncreasingToughness.SetValue(true));
        toughnessMinusBtn.onClick.AddListener(() => card.isIncreasingToughness.SetValue(false));
        closeBtn.onClick.AddListener(() => Destroy(this.gameObject));

        card.power.nonNetworkChange += (_) => {powerInputField.text = card.power.Value.ToString();};
        card.toughness.nonNetworkChange += (_) => {toughnessInputField.text = card.toughness.Value.ToString();};
    }


    private void OnDestroy()
    {
        card.power.nonNetworkChange -= (_) => {powerInputField.text = card.power.Value.ToString();};
        card.toughness.nonNetworkChange -= (_) => {toughnessInputField.text = card.toughness.Value.ToString();};
    }
}
