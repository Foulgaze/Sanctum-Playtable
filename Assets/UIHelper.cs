using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHelper : MonoBehaviour
{
    public void DisableTransform(Transform transform)
    {
        transform.gameObject.SetActive(false);
    }

    public void EnableTransform(Transform transform)
    {
        transform.gameObject.SetActive(true);
    }
}
