

using System.Collections.Generic;
using Sanctum_Core;
using UnityEditor.Experimental.GraphView;
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
	

	void Start()
	{
		rect = GetComponent<RectTransform>();
	}

	public void StartDrag(Transform dragParent)
    {
        GameOrchestrator.Instance.handController.currentHeldCardId = cardId;
		Transform cardImage = CardFactory.Instance.GetCardImage(cardId, false,  renderCardBack : renderCardBack);
		draggableRect = cardImage.GetComponent<RectTransform>();
		draggableRect.transform.SetParent(dragParent);
		SetupDraggedCard(draggableRect);
		offset = GetDragOffset();
    }

	private Vector2 GetDragOffset()
	{
		if(GameOrchestrator.Instance.handController.CardInHand(cardId)) // In hand
		{
			return rect.transform.position -  Input.mousePosition;

			// return rect.transform.position -  MouseUtility.Instance.GetMousePositionOnCanvas();
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
		HandleCardRelease();
    }

	private IDroppable FindFirstDroppableUIElement()
	{
		int draggableLayer = LayerMask.NameToLayer("Draggable");

		List<RaycastResult> results = DragController.Instance.PerformUIRaycast(Input.mousePosition);
		results = DragController.Instance.FilterAndSortRaycastResults(results);
		if (results.Count == 0)
		{
			return null;
		}
		
		int droppableIndex = SkipDraggableResults(results, draggableLayer);
		if (droppableIndex >= results.Count)
		{
			return null;
		}
		UnityLogger.LogError($"GO - {results[droppableIndex].gameObject} - {results[droppableIndex].gameObject.layer}");
		
		return results[droppableIndex].gameObject.GetComponent<IDroppable>();
	}

	private int SkipDraggableResults(List<RaycastResult> results, int draggableLayer)
	{
		int index = 0;
		UnityLogger.LogError($"Layer - {results[index].gameObject.layer} - Mask - {draggableLayer}");
		while (index < results.Count && results[index].gameObject.layer == draggableLayer)
		{
			index++;
		}
		return index;
	}

	private void HandleCardRelease()
	{
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
		IDroppable? droppableElement = FindFirstDroppableUIElement();
		if (droppableElement == null)
		{
			UnityLogger.LogError("Could not find drop script");
			return false;
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

		container.AddCard(cardId);
		return true;
	}
}