using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GenericCardComponents : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI name;
    [SerializeField] TextMeshProUGUI manaCost;
    [SerializeField] TextMeshProUGUI type;
    [SerializeField] TextMeshProUGUI effect;
}
