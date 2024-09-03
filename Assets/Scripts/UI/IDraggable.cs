using UnityEngine;

public interface IDraggable
{
	public void Release();
	public void UpdateDrag();
	public void StartDrag(Transform dragParent);
}