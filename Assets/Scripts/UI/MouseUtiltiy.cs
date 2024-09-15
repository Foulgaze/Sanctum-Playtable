using UnityEngine;

public class MouseUtility : MonoBehaviour
{
	public static MouseUtility Instance;
    [SerializeField] private Canvas targetCanvas;
	Camera camera;
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
		camera = Camera.main;
	}
	

    public Vector2 GetMousePositionOnCanvas()
    {
        // Ensure we have a canvas
        if (targetCanvas == null)
        {
            Debug.LogError("Target canvas is not set!");
            return Vector2.zero;
        }

        // Convert screen position to canvas position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            targetCanvas.transform as RectTransform, 
            Input.mousePosition, 
            null, 
            out Vector2 localPoint);

        return localPoint;
    }

    public Vector2 GetRectPositionInCanvasSpace(Vector2 screenPoint)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(targetCanvas.transform as RectTransform, screenPoint, null, out Vector2 localPoint);

        return localPoint;
    }

    public RaycastHit? RaycastFromMouse(int layermask)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (!Physics.Raycast(ray, out hit, Mathf.Infinity, layermask))
		{
			return null;
		}
		return hit;
    }

    public bool IsRectTransformOutsideCanvas(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        
        Rect canvasRect = targetCanvas.GetComponent<RectTransform>().rect;
        
        Vector2 canvasMin = RectTransformUtility.WorldToScreenPoint(targetCanvas.worldCamera, targetCanvas.transform.TransformPoint(canvasRect.min));
        Vector2 canvasMax = RectTransformUtility.WorldToScreenPoint(targetCanvas.worldCamera, targetCanvas.transform.TransformPoint(canvasRect.max));
        
        foreach (Vector3 corner in corners)
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(targetCanvas.worldCamera, corner);
            if (screenPoint.x >= canvasMin.x && screenPoint.x <= canvasMax.x &&
                screenPoint.y >= canvasMin.y && screenPoint.y <= canvasMax.y)
            {
                return false; 
            }
        }
        
        return true;
    }
    
}