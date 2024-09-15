using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowDrag : MonoBehaviour, IDraggable
{
	private RectTransform rect;
	private Vector2 offset;
	

	void Start()
	{
		rect = GetComponent<RectTransform>();
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
    }

}
