using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSpawnPoint : MonoBehaviour
{
    [SerializeField] private string objTag;

    // When gameobject is on moving object, attach to it by making them
    // a child when triggered.
    private void OnTriggerEnter(Collider other)
    {
        // if player
        if (other.gameObject.tag == objTag)
        {
            Player player = other.gameObject.GetComponent<Player>();
            player.UpdateSpawn(this.transform.position);
        }
    }
}
