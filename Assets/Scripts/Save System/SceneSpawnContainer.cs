using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSpawnContainer : MonoBehaviour
{
    [SerializeField] private List<GameObject> SpawnPoints;
    public Vector3 getSpawn(int index)
    {
        return SpawnPoints[index].transform.position;
    }
    
}
