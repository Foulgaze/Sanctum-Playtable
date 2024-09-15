using UnityEngine;

public interface IDraggable
{
	public void Release();
	public void UpdateDrag();
	public bool IsPickupable();
	public void StartDrag(Transform dragParent);
}