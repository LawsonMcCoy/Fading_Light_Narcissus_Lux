using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Requires a trigger attached to object
public class AttachToMovingObject : MonoBehaviour
{
    [SerializeField] private string objTag;
    [SerializeField] private Transform attachBase;  
    
    // When gameobject is on moving object, attach to it by making them
    // a child when triggered.
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == objTag)
        {
            Debug.LogWarning("Attached");
            other.gameObject.transform.SetParent(attachBase, true);
        }
    }

    // When off trigger, remove gameobject from moving object.
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == objTag)
        {
            Debug.LogWarning("Dettached");
            other.gameObject.transform.SetParent(null, true);
        }
    }
}
