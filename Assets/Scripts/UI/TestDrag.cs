using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TestDrag : MonoBehaviour
{
	private Dictionary<string, List<string>> tokenUUIDtoRelatedCards;

	void Start()
	{
		string relatedPath = $"{Application.streamingAssetsPath}/relatedTokens.txt";
		tokenUUIDtoRelatedCards = JsonConvert.DeserializeObject<Dictionary<string,List<string>>>(File.ReadAllText(relatedPath));
		Debug.Log(tokenUUIDtoRelatedCards["Darien, King of Kjeldor"]);
	}


	// public static int GetClosestGridChildIndex(GridLayoutGroup gridLayoutGroup, Vector2 position, int totalCol, int totalRow)
    // {
    //     RectTransform gridRectTransform = gridLayoutGroup.GetComponent<RectTransform>();

    //     Vector2 cellSize = gridLayoutGroup.cellSize;
    //     Vector2 spacing = gridLayoutGroup.spacing;

		
    //     int totalColumns = 4;
	// 	int totalRows = 13;

    //     int column = Mathf.FloorToInt(position.x / (cellSize.x + spacing.x));
    //     int row = Mathf.FloorToInt(position.y / (cellSize.y + spacing.y)) * -1 - 1;
    //     int totalChildren = gridLayoutGroup.transform.childCount;

    //     column = Mathf.Clamp(column, 0, totalColumns - 1);
    //     row = Mathf.Clamp(row, 0, totalRows - 1);

    //     int childIndex = row * totalColumns + column;

    //     childIndex = Mathf.Clamp(childIndex, 0, totalChildren - 1);
	// 	UnityLogger.Log($"Col : {column} | Row : {row} | Total : {totalChildren} | Total Col : {totalColumns} | Total Rows : {totalRows}");

    //     return childIndex;
    // }

	// public Vector2 GetMousePosition()
	// {
	// 	RectTransformUtility.ScreenPointToLocalPointInRectangle(
	// 			testBox,
	// 			Input.mousePosition,
	// 			null,
	// 			out Vector2 nonAdjustedPos);
	// 	return nonAdjustedPos - new Vector2(testBox.rect.size.x * -1,testBox.rect.size.y)/2;
	// }

	void Update()
	{
		if(Input.GetMouseButtonDown((int)MouseButton.Left))
		{
			// UnityLogger.Log($"{GetClosestGridChildIndex(testBox.GetComponent<GridLayoutGroup>(), GetMousePosition())}");			
		}
	}
}