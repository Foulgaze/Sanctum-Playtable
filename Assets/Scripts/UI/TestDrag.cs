using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TestDrag : MonoBehaviour
{

	void Start()
	{
		RectTransform rect = GetComponent<RectTransform>();
		Debug.Log(rect.sizeDelta);
		Debug.Log(rect.rect.size);
		float newSize = 700f;
		float moveDistance = newSize - rect.rect.size.y;
		rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,newSize);
		rect.anchoredPosition -= new Vector2(0,moveDistance/2);
		rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.rect.size.x);
		Debug.Log(rect.rect.size);

	}
}