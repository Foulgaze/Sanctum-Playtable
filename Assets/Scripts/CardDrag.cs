

using Sanctum_Core;
using UnityEngine;

public class CardDrag : MonoBehaviour, IDraggable
{
	public int cardId;
	private RectTransform rect;
	private RectTransform draggableRect;
	private int raycastLayermask;
	private Vector2 offset;

	void Start()
	{
		rect = GetComponent<RectTransform>();
		raycastLayermask = 1 << LayerMask.NameToLayer("CardContainer");
	}

	public void StartDrag(Transform dragParent)
    {
        GameOrchestrator.Instance.handController.currentHeldCardId = cardId;
		Transform cardImage = CardFactory.Instance.GetCardImage(cardId, false);
		draggableRect = cardImage.GetComponent<RectTransform>();
		draggableRect.transform.SetParent(dragParent);
		SetupDraggedCard(draggableRect);
		offset = GetDragOffset();
    }

	private Vector2 GetDragOffset()
	{
		if(GameOrchestrator.Instance.handController.CardInHand(cardId)) // In hand
		{
			return rect.anchoredPosition -  MouseUtility.Instance.GetMousePositionOnCanvas();
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
		RaycastForRelease();
    }

	private void RaycastForRelease()
	{
		Destroy(draggableRect.gameObject);
		if(GameOrchestrator.Instance.handController.MouseInHand())
		{
			UnityLogger.Log($"Releasing in hand");

			GameOrchestrator.Instance.handController.AddCard(cardId);
			return;
		}

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (!Physics.Raycast(ray, out hit, Mathf.Infinity, raycastLayermask))
		{
			UnityLogger.Log($"Found nothing to raycast too!");
			// Test
			return;
		}
		IPhysicalCardContainer? container = hit.transform.GetComponent<IPhysicalCardContainer>();
		if(container == null)
		{
			
			UnityLogger.LogError($"Unable to find container script on object - {hit.transform.name} - {hit.transform.gameObject.layer} - {raycastLayermask}");
			GameOrchestrator.Instance.handController.AddCard(cardId); // Add to hand if breaks :)
			return;
		}
		container.AddCard(cardId);
	}
}