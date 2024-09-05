using System;
using System.Collections;
using System.Collections.Generic;
using Sanctum_Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RightClickMenuController : MonoBehaviour
{
    
    [SerializeField] private RectTransform rightClickMenuButtonHolder;
    [SerializeField] private SingleIntInputField singleIntInputField;
    [SerializeField] private Button buttonPrefab;
    public event Action<NetworkInstruction, string> networkCommand = delegate{};

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
    private void CleanupRightClickMenu()
    {
        foreach(Transform child in rightClickMenuButtonHolder)
        {
            Destroy(child.gameObject);
        }
        rightClickMenuButtonHolder.gameObject.SetActive(false);
    }

    private void SetupSingleIntInput(string name, Action submitBtnAction, string submitBtnName)
    {

    }

    private void CreateLibraryMenu()
    {
        CleanupRightClickMenu();
        Button drawCardBtn = Instantiate(buttonPrefab, rightClickMenuButtonHolder);
        drawCardBtn.onClick.AddListener(() => {networkCommand(NetworkInstruction.SpecialAction, $"{(int)SpecialAction.Draw}|1"); CleanupRightClickMenu();});
        Button multipleDrawBtn = Instantiate(buttonPrefab, rightClickMenuButtonHolder);
        Action drawCards = () => 
        {
            if(string.IsNullOrEmpty(singleIntInputField.input.text))
            {
                return;
            }
            networkCommand(NetworkInstruction.SpecialAction, $"{(int)SpecialAction.Draw}|{int.Parse(singleIntInputField.input.text)}");
        };
        multipleDrawBtn.onClick.AddListener(() => SetupSingleIntInput("Draw Cards",drawCards ,"Draw"));
        Button shuffle = Instantiate(buttonPrefab, rightClickMenuButtonHolder);
        shuffle.onClick.AddListener(() => {networkCommand(NetworkInstruction.NetworkAttribute, ((int)SpecialAction.Shuffle).ToString());});
        Button viewLibrary = Instantiate(buttonPrefab, rightClickMenuButtonHolder);
        // To Do
        Button viewTopCards = Instantiate(buttonPrefab, rightClickMenuButtonHolder);
        // To Do
        Button revealLibraryTo = Instantiate(buttonPrefab, rightClickMenuButtonHolder);
        // To Do
        Button revealTopCardsTo = Instantiate(buttonPrefab, rightClickMenuButtonHolder);
        // To Do
        Button alwaysRevealTopCard = Instantiate(buttonPrefab, rightClickMenuButtonHolder);
        // To Do
        Button mill = Instantiate(buttonPrefab, rightClickMenuButtonHolder);
        // To Do
        Button exile = Instantiate(buttonPrefab, rightClickMenuButtonHolder);
        // To Do

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
