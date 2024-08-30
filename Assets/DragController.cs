using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragController : MonoBehaviour
{
    private Transform? currentDraggedObject;
    private GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;
    private Vector2 offset;

    void Start()
    {
        raycaster = GetComponent<GraphicRaycaster>();
    }
    public bool IsDragging()
    {
        return currentDraggedObject == null;
    }

    private Transform? RaycastForUI()
    {
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);
        results = results.OrderBy(result => result.distance).ToList();
        results = results.OrderBy(result => result.sortingOrder).ToList();
        if(results.Count() == 0)
        {
            return null;
        }
        return results[0].gameObject.transform;

    }

    void Update()
    {
        if(Input.GetMouseButtonDown((int)MouseButton.Left))
        {
            Transform? hit = RaycastForUI();
            if(hit == null)
            {
                return;
            }
            currentDraggedObject = hit;
        }
        if(IsDragging())
        {
            UpdateDraggedCard();
        }
    }

    private void UpdateDraggedCard()
    {
        currentDraggedObject.transform.position = Input.mousePosition + offset;
    }

}
