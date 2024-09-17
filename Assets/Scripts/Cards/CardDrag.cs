

using System.Collections.Generic;
using Sanctum_Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDrag : MonoBehaviour, IDraggable
{
	public int cardId;
	private RectTransform rect;
	private RectTransform draggableRect;
	private Vector2 offset;
	public bool renderCardBack = false;
	public bool isOpponent = false;
	

	void Start()
	{
		rect = GetComponent<RectTransform>();
	}

	public void StartDrag(Transform dragParent)
    {
        GameOrchestrator.Instance.handController.currentHeldCardId = cardId;
		Transform cardImage = CardFactory.Instance.GetCardImage(cardId, false,  renderCardBack : renderCardBack);
		draggableRect = cardImage.GetComponent<RectTransform>();
		CardIdentifier.Instance.currentlyHeldCardImage = cardImage.GetComponent<GenericCardComponents>().cardImage;

		cardImage.GetComponent<Image>().raycastTarget = false;
		draggableRect.transform.SetParent(dragParent);
		SetupDraggedCard(draggableRect);
		offset = GetDragOffset();
    }

	private Vector2 GetDragOffset()
	{
		if(GameOrchestrator.Instance.handController.CardInHand(cardId)) // In hand
		{
			return MouseUtility.Instance.GetRectPositionInCanvasSpace(rect.position) -  MouseUtility.Instance.GetMousePositionOnCanvas();
		}
		draggableRect.anchoredPosition =  MouseUtility.Instance.GetMousePositionOnCanvas();
		return Vector2.zero;
	}
	public void UpdateDrag()
	{
		draggableRect.anchoredPosition = MouseUtility.Instance.GetMousePositionOnCanvas() + offset;
	}

	private void SetupDraggedCard(RectTransform rectToSetup)
    {
		rectToSetup.anchorMin = new Vector2(0.5f, 0.5f);
		rectToSetup.anchorMax = new Vector2(0.5f, 0.5f);
        rectToSetup.rotation = Quaternion.Euler(0, 0,0);
        rectToSetup.sizeDelta = HandController.CalculateCardDimensions();
		rectToSetup.localScale = Vector3.one;
    }
    public void Release()
    {
		GameOrchestrator.Instance.handController.currentHeldCardId = null;
		CardIdentifier.Instance.SetHeldCardOpacity(1f);
		CardIdentifier.Instance.currentlyHeldCardImage = null;
		HandleCardRelease();
    }

	private IDroppable? FindFirstDroppableUIElement(out bool droppedInHand)
	{
		droppedInHand = false;
		int draggableLayer = LayerMask.NameToLayer("Draggable");

		List<RaycastResult> results = DragController.Instance.PerformUIRaycast(Input.mousePosition);
		results = DragController.Instance.FilterAndSortRaycastResults(results);
		if (results.Count == 0)
		{
			return null;
		}
		int? droppableIndex = SkipDraggableResultsUnlessInHand(results, draggableLayer);
		if(droppableIndex == null)
		{
			droppedInHand = true;
			GameOrchestrator.Instance.handController.DropCard(cardId,(CardDrag)results[0].gameObject.GetComponent<IDraggable>());
			return null;
		}
		if (droppableIndex >= results.Count)
		{
			return null;
		}		
		return results[(int)droppableIndex].gameObject.GetComponent<IDroppable>();
	}

	private int? SkipDraggableResultsUnlessInHand(List<RaycastResult> results, int draggableLayer)
	{
		int index = 0;
		if(CheckIfCardIsInHand(results,draggableLayer))
		{
			return null;
		}
		while (index < results.Count && results[index].gameObject.layer == draggableLayer)
		{
			++index;
		}
		return index;
	}

	private bool CheckIfCardIsInHand(List<RaycastResult> results, int draggableLayer)
	{
		if(results.Count == 0 || results[0].gameObject.layer != draggableLayer)
		{
			return false;
		}
		CardDrag? cardDrag = (CardDrag)results[0].gameObject.GetComponent<IDraggable>();
		if(cardDrag == null)
		{
			return false;
		}
		if(CardFactory.Instance.GetCardZone(cardDrag.cardId) == CardZone.Hand)
		{
			return true;
		}
		return false;
	}

	private void HandleCardRelease()
	{
		// renderCardBack = false;
		draggableRect.GetComponent<Image>().raycastTarget = true;

		CardFactory.Instance.DisposeOfCard(cardId, draggableRect.transform, onField: false);

		if (TryDropOnUIElement())
		{
			return;
		}

		if (TryDropOnPhysicalContainer())
		{
			return;
		}

	}

	private bool TryDropOnUIElement()
	{
		IDroppable? droppableElement = FindFirstDroppableUIElement(out bool droppedInHand);
		if (droppableElement == null)
		{
			return droppedInHand;
		}

		droppableElement.DropCard(cardId);
		return true;
	}

	private bool TryDropOnPhysicalContainer()
	{
		RaycastHit? hit = MouseUtility.Instance.RaycastFromMouse(BoardController.cardContainerLayermask);
		if (!hit.HasValue)
		{
			return false;
		}

		IPhysicalCardContainer? container = hit.Value.transform.GetComponent<IPhysicalCardContainer>();
		if (container == null)
		{
			UnityLogger.LogError($"Unable to find container script on object - {hit.Value.transform.name}");
			return false;
		}

		if(container.IsOpponent())
		{
			return false;
		}
		
		container.AddCard(cardId);
		return true;
	}

	public bool IsPickupable()
	{
		return !isOpponent;
	}
}