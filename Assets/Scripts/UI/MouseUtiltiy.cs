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
    
}