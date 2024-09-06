using System;
using System.Collections;
using System.Collections.Generic;
using Sanctum_Core;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RightClickMenuController : MonoBehaviour
{
    
    [SerializeField] private RectTransform rightClickMenuButtonHolder;
    [SerializeField] private Transform menuDisableBtn;
    [SerializeField] private SingleIntInputField singleIntInputFieldPrefab;
    [SerializeField] private Transform mainGameplayScreen;
    [SerializeField] private Button buttonPrefab;
    public event Action<NetworkInstruction, string> networkCommand = delegate{};
    public float widthAsAPercentageOfSceen = 0.1f;
    public float heightAsAPercentageOfSceen = 0.1f;

    private int cardHolderMask;
    
    // Start is called before the first frame update
    void Start()
    {
        cardHolderMask = 1 << LayerMask.NameToLayer("CardContainer");
    }
    private void HandleRightClick()
    {
        if(DragController.Instance.IsDragging())
        {
            return;
        }
        RaycastHit? hit = MouseUtility.Instance.RaycastFromMouse(cardHolderMask);
        if(!hit.HasValue)
        {
            return;
        }
        IPhysicalCardContainer container = hit.Value.transform.GetComponent<IPhysicalCardContainer>();
        if(container == null)
        {
            UnityLogger.LogError($"Unable to find container script on - {hit.Value.transform}");
            return;
        }
        CardZone zone = container.GetZone();
        switch(zone)
        {
            case CardZone.Library:
                CreateLibraryMenu();
                break;
            case CardZone.Graveyard:
                CreateGraveyardMenu();
                break;
            case CardZone.Exile:
                CreateExileMenu();
                break;
            default:
                break;
        }
    }

    public void DisableRightClickMenu()
    {
        rightClickMenuButtonHolder.gameObject.SetActive(false);
        menuDisableBtn.gameObject.SetActive(false);
    }
    private void CleanupRightClickMenu()
    {
        foreach(Transform child in rightClickMenuButtonHolder)
        {
            Destroy(child.gameObject);
        }
        rightClickMenuButtonHolder.gameObject.SetActive(false);
    }

    private void SetupSingleIntInput(string name, Action<string> submitBtnAction, string submitBtnName)
    {
        SingleIntInputField singleIntInputField = Instantiate(singleIntInputFieldPrefab,mainGameplayScreen);
        singleIntInputField.Setup(name, submitBtnName);
        singleIntInputField.submitBtn.onClick.AddListener(() => submitBtnAction(singleIntInputField.input.text));
        singleIntInputField.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }
    private Button CreateBtn(string buttonText)
    {
        Button button = Instantiate(buttonPrefab, rightClickMenuButtonHolder);
        button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = buttonText;
        return button;
    }

    private void CreateLibraryMenu()
    {
        CleanupRightClickMenu();
        Button drawCardBtn = CreateBtn("Draw Card");
        drawCardBtn.onClick.AddListener(() => {networkCommand(NetworkInstruction.SpecialAction, $"{(int)SpecialAction.Draw}|1"); CleanupRightClickMenu();});
        Button multipleDrawBtn = CreateBtn("Draw Cards");
        Action<string> drawCards = (inputValue) => 
        {

            if(string.IsNullOrEmpty(inputValue))
            {
                return;
            }
            networkCommand(NetworkInstruction.SpecialAction, $"{(int)SpecialAction.Draw}|{inputValue}");
        };
        multipleDrawBtn.onClick.AddListener(() => {SetupSingleIntInput("Draw Cards",drawCards ,"Draw");CleanupRightClickMenu(); });
        Button shuffle = CreateBtn("Shuffle");
        shuffle.onClick.AddListener(() => {networkCommand(NetworkInstruction.SpecialAction, $"{(int)SpecialAction.Shuffle}|");});
        Button viewLibrary = CreateBtn("View Library");
        // To Do
        Button viewTopCards = CreateBtn("View Top Cards");
        // To Do
        Button revealLibraryTo = CreateBtn("Reveal Library");
        // To Do
        Button revealTopCardsTo = CreateBtn("Reveal Top Cards");
        // To Do
        Button alwaysRevealTopCard = CreateBtn("Flip Top Card");
        // To Do
        Button mill = CreateBtn("Mill");
        // To Do
        Button exile = CreateBtn("Exile");
        // To Do

        SetupRightClickMenu(10);
    }

    private void SetupRightClickMenu(int buttonCount)
    {
        menuDisableBtn.gameObject.SetActive(true);
        Vector2 boxDimensions = new Vector2(Screen.width*widthAsAPercentageOfSceen,Screen.height*heightAsAPercentageOfSceen*buttonCount );
        rightClickMenuButtonHolder.sizeDelta = boxDimensions;
        rightClickMenuButtonHolder.anchoredPosition = MouseUtility.Instance.GetMousePositionOnCanvas() + boxDimensions/2;
        rightClickMenuButtonHolder.gameObject.SetActive(true);

    }
    private void CreateExileMenu()
    {

    }

    private void CreateGraveyardMenu()
    {
        
    }
    

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown((int)MouseButton.Right))
        {
            HandleRightClick();
        }
    }
}
