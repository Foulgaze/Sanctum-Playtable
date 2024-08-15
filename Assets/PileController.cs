using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PileController : MonoBehaviour
{
    
    private Vector3 extents;
    void Start()
    {
        extents = this.transform.GetComponent<MeshRenderer>().bounds.extents;
    }

    public void UpdateBoardState()
    {
        
    }
}
