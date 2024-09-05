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
    [SerializeField] private CanvasScaler scaler;
    public static float scaleFactor;
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
        scaleFactor = scaler.scaleFactor;
        pointerEventData = new(eventSystem);
    }
    public bool IsDragging()
    {
        return currentDragScript != null;
    }

    private (GameObject?, IDraggable?) RaycastForUI()
    {
        List<RaycastResult> results = new List<RaycastResult>();
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;
        eventSystem.RaycastAll(pointerEventData, results);
        if(results.Count == 0)
        {
            return (null,null);
        }
        
        results = results.OrderBy(result => result.distance).ThenByDescending(result => result.sortingOrder).ToList();
        RaycastResult hit = results[0];
        IDraggable dragScript = hit.gameObject.GetComponent<IDraggable>();
        if(dragScript == null)
        {
            //UnityLogger.LogError($"Could not find drag script on {hit.gameObject.name}");
        }        
        return (hit.gameObject, dragScript);
    }

    private void CheckForStartDrag()
    {
        if(Input.GetMouseButtonDown((int)MouseButton.Left))
        {
            (GameObject? dragObj , IDraggable? dragScript) = RaycastForUI();
            if(dragScript == null)
            {
                return;
            }
            currentDragScript = dragScript;
            dragScript.StartDrag(dragParent);
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
