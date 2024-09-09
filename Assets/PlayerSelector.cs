using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelector : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI windowNameText;
    [SerializeField] private TextMeshProUGUI submitBtnText;
    [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;
    [SerializeField] private Toggle playerSelectPrefab;
    [SerializeField] private Button submitBtn;
    [SerializeField] private Dictionary<string, Toggle> uuidToToggle = new();
    public void Setup(string windowName, Action<List<string>> submitAction, string submitBtnText)
    {
        this.windowNameText.text = windowName;
        this.submitBtnText.text = submitBtnText;
        submitBtn.onClick.AddListener(() => {submitAction(GetSelectedUUIDs());});
    }

    public List<string> GetSelectedUUIDs()
    {
        List<string> uuids = new();
        foreach(var kvp in uuidToToggle)
        {
            if(kvp.Value.isOn)
            {
                uuids.Add(kvp.Key);
            }
        }
        return returnList;
    }
}
