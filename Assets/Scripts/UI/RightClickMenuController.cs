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
    [SerializeField] private PlayerSelector playerSelectorPrefab;
    [SerializeField] private Transform mainGameplayScreen;
    [SerializeField] private Button buttonPrefab;
    [SerializeField] private ContainerViewer containerViewerPrefab;
    public event Action<NetworkInstruction, string> networkCommand = delegate{};
    public float widthAsAPercentageOfSceen = 0.1f;
    public float heightAsAPercentageOfSceen = 0.1f;
    public Player clientPlayer;
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

    public void CleanupRightClickMenu()
    {
        foreach(Transform child in rightClickMenuButtonHolder)
        {
            Destroy(child.gameObject);
        }
        rightClickMenuButtonHolder.gameObject.SetActive(false);
        menuDisableBtn.gameObject.SetActive(false);

    }

    private void SetupSingleIntInput(string name, Action<string> submitBtnAction, string submitBtnName)
    {
        SingleIntInputField singleIntInputField = Instantiate(singleIntInputFieldPrefab,mainGameplayScreen);
        singleIntInputField.Setup(name, submitBtnAction,submitBtnName);
        singleIntInputField.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }
    private Button CreateBtn(string buttonText, Action onClick)
    {
        Button button = Instantiate(buttonPrefab, rightClickMenuButtonHolder);
        button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = buttonText;
        button.onClick.AddListener(() => onClick());
        button.onClick.AddListener(() => CleanupRightClickMenu());
        return button;
    }

    private void ExecuteSpecialAction(SpecialAction action, string parameter = "")
    {
        networkCommand(NetworkInstruction.SpecialAction, $"{(int)action}|{parameter}");
    }

    private void CreateContianerView(CardContainerCollection collection)
    {
        ContainerViewer containerViewer = Instantiate(containerViewerPrefab, rightClickMenuButtonHolder.parent);
        containerViewer.Setup(collection, $"{collection.Zone}", false);
    }
    private void CreateContainerRevealCards(CardContainerCollection collection)
    {
        Action<string> revealCards = (rawCardCount) => 
        {
            ContainerViewer containerViewer = Instantiate(containerViewerPrefab, rightClickMenuButtonHolder.parent);
            containerViewer.Setup(collection, $"{collection.Zone}", false, int.Parse(rawCardCount));
        };
        SetupSingleIntInput(name : "Reveal Count", revealCards, "Reveal");
    }
    private void CreateOpponentSelectMenu(string windowName, Action<List<string>> submitAction,string submitBtnText)
    {
        PlayerSelector playerSelector = Instantiate(playerSelectorPrefab, mainGameplayScreen);
        playerSelector.Setup(windowName, submitAction,submitBtnText);
    }

    private void RevealLibraryOpponent()
    {
        Action<List<string>> revealAction = (uuids) => {GameOrchestrator.Instance.RevealZoneToOpponents(CardZone.Library, uuids, null);};
        CreateOpponentSelectMenu("Reveal Library", revealAction, "Reveal"); 
    }

    public void RevealOpponentZone(NetworkAttribute attribute, Player player)
    {
        (CardZone zone, int? revealCount) = ((NetworkAttribute<(CardZone,int?)>)attribute).Value;
        if(!Enum.IsDefined(typeof(CardZone), zone))
        {
            UnityLogger.LogError($"Invalid cardzone value : {zone}");
            return;
        }
        CardContainerCollection collection = player.GetCardContainer(zone);
        ContainerViewer containerViewer = Instantiate(containerViewerPrefab, rightClickMenuButtonHolder.parent);
        containerViewer.Setup(collection, $"{collection.Zone}", false, revealCount);
    }
    
    
    private void CreateLibraryMenu()
    {
        CleanupRightClickMenu();

        List<Button> setupButtons = new()
        {
            CreateBtn("Draw Card", () => ExecuteSpecialAction(SpecialAction.Draw, "1")),
            CreateBtn("Draw Cards", () => SetupSingleIntInput("Draw Cards",(input) => ExecuteSpecialAction(SpecialAction.Draw, input), "Draw" )),
            CreateBtn("Shuffle", () => ExecuteSpecialAction(SpecialAction.Shuffle)),
            CreateBtn("View All Cards", () => {CreateContianerView(clientPlayer.GetCardContainer(CardZone.Library));}),
            CreateBtn("View Top Cards", () => {CreateContainerRevealCards(clientPlayer.GetCardContainer(CardZone.Library));}),
            CreateBtn("Reveal To", () => {RevealLibraryOpponent();} ),
            CreateBtn("Reveal Top Cards To", () => {} ),
            CreateBtn("Flip Top Card", () => {GameOrchestrator.Instance.FlipLibraryTop();} ),
            CreateBtn("Mill Cards", () => SetupSingleIntInput("Mill Cards",(input) => ExecuteSpecialAction(SpecialAction.Mill, input), "Mill" )),
            CreateBtn("Exile Cards", () => SetupSingleIntInput("Mill Cards",(input) => ExecuteSpecialAction(SpecialAction.Exile, input), "Mill" )),
        };

        SetupRightClickMenu(setupButtons.Count);
    }

    private void SetupRightClickMenu(int buttonCount)
    {
        menuDisableBtn.gameObject.SetActive(true);
        rightClickMenuButtonHolder.gameObject.SetActive(true);
        Vector2 boxDimensions = new Vector2(Screen.width*widthAsAPercentageOfSceen,Screen.height*heightAsAPercentageOfSceen*buttonCount );
        rightClickMenuButtonHolder.sizeDelta = boxDimensions;
        rightClickMenuButtonHolder.anchoredPosition = MouseUtility.Instance.GetMousePositionOnCanvas() + boxDimensions/2;

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
