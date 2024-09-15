using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sanctum_Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class tokenMenuController : MonoBehaviour
{
    [SerializeField] private Button buttonPrefab;
    [SerializeField] private RectTransform viewPort;
    [SerializeField] private RectTransform contentRect;
    [SerializeField] private int buttonViewCount = 0;
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button createBtn;
    [SerializeField] private TMP_InputField tokenCountField;


    private string selectedTokenUUID;
    private string defaultUUID;

    public void Setup(List<(string,string)> uuidNamePair)
    {
        // uuidNamePair.Sort((name_one, name_two) => name_one.Item2.CompareTo(name_two.Item2));
        selectedTokenUUID = defaultUUID = uuidNamePair[0].Item1;
        closeBtn.onClick.AddListener(() => {transform.gameObject.SetActive(false);});

        createBtn.onClick.AddListener(() => HandleCreateClick());
        SetupButtons(uuidNamePair);
    }

    private void HandleCreateClick()
    {
        gameObject.SetActive(false);
        if(string.IsNullOrEmpty(tokenCountField.text))
        {
            return;
        }
        for(int i = 0; i < int.Parse(tokenCountField.text); ++i)
        {
            GameOrchestrator.Instance.SendSpecialAction(SpecialAction.CreateToken, selectedTokenUUID.ToString());
        }
    }

    void OnEnable()
    {
        selectedTokenUUID = defaultUUID;
        transform.SetAsLastSibling();
    }

    private void SetupButtons(List<(string,string)> uuidNamePair)
    {
        float buttonHeight = viewPort.rect.size.y/buttonViewCount;
        Vector2 buttonDimensions = new Vector2(buttonPrefab.GetComponent<RectTransform>().sizeDelta.x, buttonHeight);
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, buttonHeight * uuidNamePair.Count);
        foreach(var kvp in uuidNamePair)
        {
            Button button = Instantiate(buttonPrefab, contentRect.transform);
            RectTransform rt = button.GetComponent<RectTransform>();
            rt.sizeDelta = buttonDimensions;
            button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = kvp.Item2;
            button.onClick.AddListener(() => {selectedTokenUUID = kvp.Item1;});
        }
    }

    private void OnClose()
    {
        
    }


}
