using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObjects/Save")]
public class SaveScrtiptableObject : ScriptableObject
{
    public Vector3 spawnPoint; //is set by the SavePoint script
    public Scenes.ScenesList saveScene; //set this in editor
    public UnityEvent playerDeath;
    public bool saveSuccesful; //bool to know if save was valid
                                //made false if the save was not in correct scene in narrationManager

    private void OnEnable()
    {
        playerDeath = new UnityEvent();
    }
    public void setSpawnPoint(Vector3 point)
    {
        spawnPoint = point;
    }
    
}
