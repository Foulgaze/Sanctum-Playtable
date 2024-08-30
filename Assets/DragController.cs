using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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
    [SerializeField] private EventSystem eventSystem;
    private Vector2 offset;
    private int draggableMask;

    void Start()
    {
        raycaster = GetComponent<GraphicRaycaster>();
        pointerEventData = new(eventSystem);
        draggableMask = LayerMask.NameToLayer("Draggable");
    }
    public bool IsDragging()
    {
        return currentDraggedObject != null;
    }

    private Transform? RaycastForUI()
    {
        List<RaycastResult> results = new List<RaycastResult>();
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;
        raycaster.Raycast(pointerEventData, results);
        if(results.Count == 0)
        {
            return null;
        }
        
        results = results.OrderBy(result => result.distance).ThenByDescending(result => result.sortingOrder).ToList();
        RaycastResult hit = results[0];
        Debug.Log($"{hit.gameObject.name}");
        if(hit.gameObject.layer != draggableMask)
        {
            return null;
        }
        
        return hit.gameObject.transform;
    }

    private void CheckForStartDrag()
    {
        if(Input.GetMouseButtonDown((int)MouseButton.Left))
        {
            Transform? hit = RaycastForUI();
            if(hit == null)
            {
                return;
            }
            currentDraggedObject = hit;
            offset = hit.position - Input.mousePosition;
        }
    }
    private void CheckForReleaseDrag()
    {
        if(!IsDragging() || Input.GetMouseButton((int) MouseButton.Left))
        {
            return;
        }
        currentDraggedObject = null;
    }

    void Update()
    {
        CheckForStartDrag();
        CheckForReleaseDrag();
        UpdateDragPosition();
    }

    private void UpdateDragPosition()
    {
        if(!IsDragging())
        {
            return;
        }
        currentDraggedObject.position = (Vector2)Input.mousePosition + offset;
    }

}
