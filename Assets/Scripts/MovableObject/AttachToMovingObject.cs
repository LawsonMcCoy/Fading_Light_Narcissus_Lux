using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Requires a trigger attached to object
public class AttachToMovingObject : MonoBehaviour
{
    [SerializeField] private string objTag;
    [SerializeField] private Transform attachBase;

    private GameObject player;
    // https://forum.unity.com/threads/object-is-changing-size-when-becoming-a-child-in-game.711854/
    // The link i used to solve the scaling issue

    // Instructions:
    /* When you want to attach an object to a moving platform, you must set have an empty object with the scale of (1,1,1) first.
     * Attach the moving script in this empty object. Then place your platform as a child of this empty object.
     * Be mindful that the platform scale will change so you would need to replace the old values again.
     * Attach trigger to platform object and attach this script.
     * Attachbase should be the parent empty object, NOT THE PLATFORM. It should work properly afterwards.
     * Ex)
     * Parent empty object (scale 1,1,1) 
     *      Moving platform (scale whatever)
     *      Player (scale whatever)
     */

    // When gameobject is on moving object, attach to it by making them
    // a child when triggered.
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == objTag)
        {
            Debug.LogWarning("Attached");
            player = other.gameObject;
            player.transform.SetParent(attachBase, true);
        }
    }
   
    // When off trigger, remove gameobject from moving object.
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == objTag)
        {
            Debug.LogWarning("Dettached");
            player = other.gameObject;
            Vector3 playerDefaultScale = player.GetComponent<Player>().GetPlayerScale;

            player.transform.SetParent(null, true);
            player.transform.localScale = playerDefaultScale;
        }
    }
}
