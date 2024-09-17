using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class commanderTaxBtn : MonoBehaviour
{
    [SerializeField]private  EventSystem eventSystem;
    public float pressDepth = 0.2f; 
    public float pressSpeed = 5f;  
    private Vector3 initialPosition; 
    private bool isClickable = true;
    public bool increaseValue = true;
    public event Action<bool> onClick = delegate{};
    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && isClickable && !IsPointerOverUI())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform)
                {
                    StartCoroutine(PressButton());
                }
            }
        }
    }

    // Coroutine to handle the button press animation
    IEnumerator PressButton()
    {
        isClickable = false;
        Vector3 pressedPosition = initialPosition - new Vector3(0, pressDepth, 0); // Downward movement
        onClick(increaseValue);

        while (Vector3.Distance(transform.position, pressedPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, pressedPosition, Time.deltaTime * pressSpeed);
            yield return null;
        }

        yield return new WaitForSeconds(0.1f); // Small pause at the bottom

        while (Vector3.Distance(transform.position, initialPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, initialPosition, Time.deltaTime * pressSpeed);
            yield return null;
        }

        isClickable = true;
    }

    private bool IsPointerOverUI()
    {
        return eventSystem.IsPointerOverGameObject();
    }
}

