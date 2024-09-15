using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerViewerDropHelper : MonoBehaviour, IDroppable
{
    [SerializeField] private ContainerViewer containerViewer;

    public void DropCard(int cardId)
    {
        containerViewer.DropCard(cardId);
    }


}
