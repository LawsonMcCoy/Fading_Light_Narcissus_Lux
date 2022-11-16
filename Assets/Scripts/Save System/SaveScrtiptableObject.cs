using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObjects/Save")]
public class SaveScrtiptableObject : ScriptableObject
{
    public Vector3 spawnPoint;
    public Scenes.ScenesList saveScene;
    public UnityEvent playerDeath;

    private void OnEnable()
    {
        playerDeath = new UnityEvent();
    }
    public void setSpawnPoint(Vector3 point)
    {
        spawnPoint = point;
    }
    public void setScene(Scenes.ScenesList myscene)
    {
        saveScene = myscene;
    }
    
}
