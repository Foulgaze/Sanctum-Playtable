using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sanctum_Core;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RightClickMenuController : MonoBehaviour
{
    
    [SerializeField] private RectTransform buttonHolder;
    [SerializeField] private Transform menuDisableBtn;
    [SerializeField] private SingleIntInputField singleIntInputFieldPrefab;
    [SerializeField] private PlayerSelector playerSelectorPrefab;
    [SerializeField] private Transform mainGameplayScreen;
    [SerializeField] private Button buttonPrefab;
    [SerializeField] private ContainerViewer containerViewerPrefab;
    [SerializeField] private Transform moveCardMenuPrefab;
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

    private CardZone? RaycastForCardZone()
    {
        RaycastHit? hit = MouseUtility.Instance.RaycastFromMouse(cardHolderMask);
        if(!hit.HasValue)
        {
            return null;
        }
        IPhysicalCardContainer container = hit.Value.transform.GetComponent<IPhysicalCardContainer>();
        if(container == null)
        {
            UnityLogger.LogError($"Unable to find container script on - {hit.Value.transform}");
            return null;
        }
        return container.GetZone();
    }

    private bool HandleUIRightClick()
    {
        IDraggable? draggableScript = DragController.Instance.RaycastForDraggable();
        if(draggableScript == null || draggableScript is not CardDrag)
        {
            return false;
        }
        CardDrag hitCardScript = (CardDrag) draggableScript;
        CardZone hitCardCurrentZone = CardFactory.Instance.GetCardZone(hitCardScript.cardId);
        var skipZones = new CardZone[]{CardZone.Library, CardZone.Graveyard, CardZone.Exile, CardZone.CommandZone};
        if(skipZones.Contains(hitCardCurrentZone))
        {
            return false;
        }
        var fieldZones = new CardZone[]{CardZone.MainField, CardZone.LeftField, CardZone.RightField};
        if(fieldZones.Contains(hitCardCurrentZone))
        {
            CreateCardOnFieldMenu();
        }
        else
        {
            CreateCardInHandMenu(hitCardScript.cardId);
        }
        return true;
    }

    private void CreateMoveToMenu(int cardId)
    {
        MoveCardMenu menu = Instantiate(moveCardMenuPrefab, mainGameplayScreen).GetComponent<MoveCardMenu>();
        Action moveToGraveyard = () => {GameOrchestrator.Instance.MoveCard(CardZone.Graveyard, new InsertCardData(null,cardId, null, false ));};
        Action moveToExile = () => {GameOrchestrator.Instance.MoveCard(CardZone.Exile, new InsertCardData(null,cardId, null, false ));};
        Action<int> moveXFromTop = (distance) => {GameOrchestrator.Instance.SendSpecialAction(SpecialAction.PutCardXFrom, $"top|{cardId}|{distance}");};
        Action<int> moveXFromBottom = (distance) => {GameOrchestrator.Instance.SendSpecialAction(SpecialAction.PutCardXFrom,$"bottom|{cardId}|{distance}");};
        menu.Setup(moveToGraveyard, moveToExile, moveXFromTop, moveXFromBottom);

    }

    private void CreateCardInHandMenu(int cardId)
    {
        CleanupMenu();
        List<Button> setupButtons = new()
        {
            CreateBtn("Play", () => GameOrchestrator.Instance.MoveCard(CardZone.MainField, new InsertCardData(null, cardId, null, true))),
            CreateBtn("Play Face Down", () => GameOrchestrator.Instance.MoveCard(CardZone.MainField, new InsertCardData(null, cardId, null, true))),
            CreateBtn("Move To", () => {CreateMoveToMenu(cardId);}),
        };
        SetupMenu(setupButtons.Count);
        
    }
    private void CreateCardOnFieldMenu()
    {

    }

    private void HandleGameObjectRightClick()
    {
        CardZone? zone = RaycastForCardZone();
        switch(zone)
        {
            case CardZone.Library:
                CreateLibraryMenu();
                break;
            case CardZone.Graveyard:
            case CardZone.Exile:
                CreateNonLibraryPileMenu((CardZone)zone);
                break;
            default:
                break;
        }
    }
    private void HandleRightClick()
    {
        if(DragController.Instance.IsDragging())
        {
            return;
        }
        if(HandleUIRightClick())
        {
            return;
        }
        HandleGameObjectRightClick();
    }

    public void CleanupMenu()
    {
        foreach(Transform child in buttonHolder)
        {
            Destroy(child.gameObject);
        }
        buttonHolder.gameObject.SetActive(false);
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
        Button button = Instantiate(buttonPrefab, buttonHolder);
        button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = buttonText;
        button.onClick.AddListener(() => onClick());
        button.onClick.AddListener(() => CleanupMenu());
        return button;
    }

    private void ExecuteSpecialAction(SpecialAction action, string parameter = "")
    {
        networkCommand(NetworkInstruction.SpecialAction, $"{(int)action}|{parameter}");
    }

    private void CreateContianerView(CardContainerCollection collection)
    {
        ContainerViewer containerViewer = Instantiate(containerViewerPrefab, buttonHolder.parent);
        containerViewer.Setup(collection, $"{collection.Zone}", false);
    }
    private void CreateContainerRevealCards(CardContainerCollection collection)
    {
        Action<string> revealCards = (rawCardCount) => 
        {
            ContainerViewer containerViewer = Instantiate(containerViewerPrefab, buttonHolder.parent);
            containerViewer.Setup(collection, $"{collection.Zone}", false, int.Parse(rawCardCount));
        };
        SetupSingleIntInput(name : "Reveal Count", revealCards, "Reveal");
    }
    private void CreateOpponentSelectMenu(string windowName, Action<List<string>> submitAction,string submitBtnText)
    {
        PlayerSelector playerSelector = Instantiate(playerSelectorPrefab, mainGameplayScreen);
        playerSelector.Setup(windowName, submitAction,submitBtnText);
    }

    private void RevealLibraryOpponent(int? setValue = null)
    {
        Action<List<string>> revealAction = (uuids) => {GameOrchestrator.Instance.RevealZoneToOpponents(CardZone.Library, uuids, setValue);};
        CreateOpponentSelectMenu("Reveal Library", revealAction, "Reveal"); 
    }

    private void RevealLibraryToOpponentCount(string rawCountValue)
    {
        int? value = !string.IsNullOrWhiteSpace(rawCountValue) ? int.Parse(rawCountValue) : null;
        RevealLibraryOpponent(value);
    }

    public void RevealOpponentZone(NetworkAttribute attribute, Playtable table)
    {
        (CardZone zone,string opponentUUID, int? revealCount) = ((NetworkAttribute<(CardZone,string,int?)>)attribute).Value;
        if(!Enum.IsDefined(typeof(CardZone), zone))
        {
            UnityLogger.LogError($"Invalid cardzone value : {zone}");
            return;
        }
        Player? opponentPlayer = table.GetPlayer(opponentUUID);
        if(opponentPlayer == null)
        {
            UnityLogger.LogError($"Unable to find opponent of id - {opponentUUID}");
            return;
        }
        ContainerViewer containerViewer = Instantiate(containerViewerPrefab, buttonHolder.parent);
        CardContainerCollection collection = opponentPlayer.GetCardContainer(zone);
        containerViewer.Setup(collection, $"{collection.Zone}", true, revealCount);
    }
    
    
    private void CreateLibraryMenu()
    {
        CleanupMenu();
        List<Button> setupButtons = new()
        {
            CreateBtn("Draw Card", () => ExecuteSpecialAction(SpecialAction.Draw, "1")),
            CreateBtn("Draw Cards", () => SetupSingleIntInput("Draw Cards",(input) => ExecuteSpecialAction(SpecialAction.Draw, input), "Draw" )),
            CreateBtn("Shuffle", () => ExecuteSpecialAction(SpecialAction.Shuffle)),
            CreateBtn("View All Cards", () => {CreateContianerView(clientPlayer.GetCardContainer(CardZone.Library));}),
            CreateBtn("View Top Cards", () => {CreateContainerRevealCards(clientPlayer.GetCardContainer(CardZone.Library));}),
            CreateBtn("Reveal To", () => {RevealLibraryOpponent();} ),
            CreateBtn("Reveal Top Cards To", () => {SetupSingleIntInput("Reveal To Opponent Card Count", RevealLibraryToOpponentCount, "Select Opponents");} ),
            CreateBtn("Flip Top Card", () => {GameOrchestrator.Instance.FlipLibraryTop();} ),
            CreateBtn("Mill Cards", () => SetupSingleIntInput("Mill Cards",(input) => ExecuteSpecialAction(SpecialAction.Mill, input), "Mill" )),
            CreateBtn("Exile Cards", () => SetupSingleIntInput("Exile Cards",(input) => ExecuteSpecialAction(SpecialAction.Exile, input), "Exile" )),
        };
        SetupMenu(setupButtons.Count);
    }

    private void CreateNonLibraryPileMenu(CardZone zone)
    {
        CleanupMenu();
        List<Button> setupButtons = new()
        {
            CreateBtn($"View {zone}", () => CreateContianerView(clientPlayer.GetCardContainer(zone))),
        };
        SetupMenu(setupButtons.Count);
    }

    private void SetupMenu(int buttonCount)
    {
        menuDisableBtn.gameObject.SetActive(true);
        buttonHolder.gameObject.SetActive(true);
        Vector2 boxDimensions = new Vector2(Screen.width*widthAsAPercentageOfSceen,Screen.height*heightAsAPercentageOfSceen*buttonCount );
        buttonHolder.sizeDelta = boxDimensions;
        buttonHolder.anchoredPosition = MouseUtility.Instance.GetMousePositionOnCanvas() + boxDimensions/2;

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
