using UnityEngine;
using System.Collections;

public class CardDragTiltEffect : MonoBehaviour
{
    [SerializeField] private float maxTiltAngle = 45f; // Increased default max tilt
    [SerializeField] private float tiltSpeed = 15f;
    [SerializeField] private float tiltFactor = 0.2f; // New variable to control tilt sensitivity

    private RectTransform rectTransform;
    private Vector2 lastMousePosition;
    private Quaternion targetRotation;
    private bool isDragging = false;
    private Coroutine dragCoroutine;
    private Vector2 dragStartPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void StartDragging()
    {
        
		isDragging = true;
		lastMousePosition = Input.mousePosition;
		dragStartPosition = lastMousePosition;
		if (dragCoroutine != null)
		{
			StopCoroutine(dragCoroutine);
		}
		dragCoroutine = StartCoroutine(DragUpdate());
    }

    public void StopDragging()
    {

		isDragging = false;
		if (dragCoroutine != null)
		{
			StopCoroutine(dragCoroutine);
		}
		rectTransform.rotation = Quaternion.identity;
    }

    private IEnumerator DragUpdate()
    {
        while (isDragging)
        {
            Vector2 currentMousePosition = Input.mousePosition;
            Vector2 totalDragDelta = currentMousePosition - dragStartPosition;

            // Calculate tilt based on total drag distance from start
            float tiltX = -totalDragDelta.y * tiltFactor;
            float tiltY = totalDragDelta.x * tiltFactor;

            // Clamp tilt angles
            tiltX = Mathf.Clamp(tiltX, -maxTiltAngle, maxTiltAngle);
            tiltY = Mathf.Clamp(tiltY, -maxTiltAngle, maxTiltAngle);

            // Set target rotation
            targetRotation = Quaternion.Euler(tiltX, tiltY, 0);

            lastMousePosition = currentMousePosition;

            yield return null;
        }
    }

    private void Update()
    {
        if (isDragging)
        {
            rectTransform.rotation = Quaternion.Slerp(rectTransform.rotation, targetRotation, Time.deltaTime * tiltSpeed);
        }

    }
}