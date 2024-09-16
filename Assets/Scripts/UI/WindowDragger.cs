using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowDragger : MonoBehaviour, IDraggable
{
	private RectTransform rect;
	private Vector2 offset;
	private Transform originalParent;

	void Start()
	{
		rect = GetComponent<RectTransform>();
		originalParent = transform.parent;
	}

	public void StartDrag(Transform dragParent)
    {
		rect.transform.SetParent(dragParent);
        dragParent.SetAsLastSibling();
		offset = GetDragOffset();
    }

	private Vector2 GetDragOffset()
	{
        return rect.anchoredPosition -  MouseUtility.Instance.GetMousePositionOnCanvas();
	}
	public void UpdateDrag()
	{
		rect.anchoredPosition = MouseUtility.Instance.GetMousePositionOnCanvas() + offset;
	}

    public void Release()
    {
        if(MouseUtility.Instance.IsRectTransformOutsideCanvas(rect))
        {
            rect.anchoredPosition = Vector2.zero;
        }
		transform.SetParent(originalParent);
    }

	public bool IsPickupable()
	{
		return true;
	}

}
