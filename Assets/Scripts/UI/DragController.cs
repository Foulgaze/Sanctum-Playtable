using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragController : MonoBehaviour
{
    private IDraggable? currentDragScript;
    private PointerEventData pointerEventData;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private Transform dragParent;
    private int ignoreLayer = 0;
    private Vector2 offset;
    public static DragController Instance;
    private void Awake() 
    {         
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }
    void Start()
    {
        ignoreLayer = LayerMask.NameToLayer("IgnoreRaycast");
        pointerEventData = new(eventSystem);
    }
    public bool IsDragging()
    {
        return currentDragScript != null;
    }

    public List<RaycastResult> PerformUIRaycast(Vector2 screenPosition)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = screenPosition;
        eventSystem.RaycastAll(pointerEventData, results);
        return results;
    }

    public List<RaycastResult> FilterAndSortRaycastResults(List<RaycastResult> results)
    {
        return results
            .Where(result => result.gameObject.layer != ignoreLayer)
            .OrderBy(result => result.distance)
            .ThenByDescending(result => result.sortingOrder)
            .ToList();
    }

    public T GetComponentFromRaycastResult<T>(RaycastResult hit) where T : class
    {
        return hit.gameObject.GetComponent<T>();
    }

    public IDraggable? RaycastForDraggable()
    {
        var results = PerformUIRaycast(Input.mousePosition);
        var filteredResults = FilterAndSortRaycastResults(results);

        if (filteredResults.Count == 0)
        {
            return null;
        }

        var hit = filteredResults[0];
        var dragScript = GetComponentFromRaycastResult<IDraggable>(hit);
        
        return dragScript;
    }

    private void CheckForStartDrag()
    {
        if(Input.GetMouseButtonDown((int)MouseButton.Left))
        {
            IDraggable? dragScript = RaycastForDraggable();
            if(dragScript == null || !dragScript.IsPickupable())
            {
                return;
            }
            currentDragScript = dragScript;
            dragScript.StartDrag(dragParent);
            GameOrchestrator.Instance.DisableRightClickMenu();
        }
    }
    private void CheckForReleaseDrag()
    {
        if(Input.GetMouseButton((int) MouseButton.Left))
        {
            return;
        }
        currentDragScript.Release();
        currentDragScript = null;
    }

    void Update()
    {
        CheckForStartDrag();
        if(IsDragging())
        {
            UpdateDragPosition();
            CheckForReleaseDrag();
        }
        
    }

    private void UpdateDragPosition()
    {
        currentDragScript.UpdateDrag();
    }
    public static Vector2 ClampNewPosition(Vector2 newPosition)
    {
        // newPosition.x = Math.Clamp(newPosition.x, screenDimensions.x/-2, screenDimensions.x/2);
        // newPosition.y = Math.Clamp(newPosition.y, screenDimensions.y/-2, screenDimensions.y/2);
        return newPosition;
    }
}
