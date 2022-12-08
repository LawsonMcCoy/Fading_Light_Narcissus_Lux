using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SaveSequence", menuName = "SaveSequence")]

public class SaveSequence : NarrationSequence
{
    //the Accept function for the visitor pattern
    public override void Accept()
    {
        NarrationManager.Instance.ProcessSave(this);
    }

    //data

    public Scenes.ScenesList saveScene; //set this in editor
    public bool saveSuccesful; //bool to know if save was valid
                               //made false if the save was not in correct scene in narrationManager
    public int spawnIndex;
}
