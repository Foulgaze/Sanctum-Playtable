using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TestDrag : MonoBehaviour
{
	[SerializeField] RectTransform testBox;
	void Start()
	{
		

	}


	public static int GetClosestGridChildIndex(GridLayoutGroup gridLayoutGroup, Vector2 position, int totalCol, int totalRow)
    {
		UnityLogger.Log($"Input Position - {position}");
        // Get the RectTransform of the GridLayoutGroup
        RectTransform gridRectTransform = gridLayoutGroup.GetComponent<RectTransform>();

        // Get the cell size and spacing
        Vector2 cellSize = gridLayoutGroup.cellSize;
        Vector2 spacing = gridLayoutGroup.spacing;

		
        // Get the number of columns in the grid
        int totalColumns = 4;
		int totalRows = 13;

        // Calculate the row and column based on the given position
        int column = Mathf.FloorToInt(position.x / (cellSize.x + spacing.x));
        int row = Mathf.FloorToInt(position.y / (cellSize.y + spacing.y)) * -1 - 1;
		UnityLogger.Log($"Row Value - {position.y / (cellSize.y + spacing.y)}");
        int totalChildren = gridLayoutGroup.transform.childCount;
        // Calculate the total number of rows

        // Clamp the row and column to ensure they stay within valid grid indices
        column = Mathf.Clamp(column, 0, totalColumns - 1);
        row = Mathf.Clamp(row, 0, totalRows - 1);

        // Calculate the child index based on the clamped row and column
        int childIndex = row * totalColumns + column;

        // Clamp the child index to ensure it's within the total number of children
        childIndex = Mathf.Clamp(childIndex, 0, totalChildren - 1);
		UnityLogger.Log($"Col : {column} | Row : {row} | Total : {totalChildren} | Total Col : {totalColumns} | Total Rows : {totalRows}");

        return childIndex;
    }

	public Vector2 GetMousePosition()
	{
		RectTransformUtility.ScreenPointToLocalPointInRectangle(
				testBox,
				Input.mousePosition,
				null,
				out Vector2 nonAdjustedPos);
		return nonAdjustedPos - new Vector2(testBox.rect.size.x * -1,testBox.rect.size.y)/2;
	}

	void Update()
	{
		if(Input.GetMouseButtonDown((int)MouseButton.Left))
		{
			// UnityLogger.Log($"{GetClosestGridChildIndex(testBox.GetComponent<GridLayoutGroup>(), GetMousePosition())}");			
		}
	}
}