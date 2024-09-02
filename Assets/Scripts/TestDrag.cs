using Unity.VisualScripting;
using UnityEngine;

public class TestDrag : MonoBehaviour,  IDraggable
{

	public int cardId;
	private RectTransform rect;
	private RectTransform draggableRect;
	private int raycastLayermask;
	private Vector2 offset;
	Vector2 screenDimensions;
	[SerializeField] Canvas canvas;

	void Start()
	{
		rect = GetComponent<RectTransform>();
		Debug.Log($"Scale factor : {DragController.scaleFactor}");
		raycastLayermask = 1 << LayerMask.NameToLayer("CardContainer");
		draggableRect = rect;
		screenDimensions = new Vector2(Screen.width, Screen.height)/2;
	}

	public void StartDrag(Transform dragParent)
    {
		draggableRect.transform.SetParent(dragParent);
		offset = GetDragOffset();
    }

	private Vector2 GetDragOffset()
	{
		return Vector2.zero;
		return draggableRect.anchoredPosition - ((Vector2)Input.mousePosition - new Vector2(Screen.width, Screen.height)/2);
	}
	public void UpdateDrag()
	{
		draggableRect.anchoredPosition = MouseUtility.Instance.GetMousePositionOnCanvas();
	}
    public void Release()
    {
		
    }

	private void RaycastForRelease()
	{
		
	}
}