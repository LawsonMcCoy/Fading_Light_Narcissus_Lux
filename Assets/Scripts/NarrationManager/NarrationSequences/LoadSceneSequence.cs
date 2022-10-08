using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A NarrationSequence for loading a new scene
[CreateAssetMenu(fileName = "LoadSceneSequence", menuName = "LoadSceneSequence")]
public class LoadSceneSequence : NarrationSequence
{
    //the Accept function for the visitor pattern
    public override void Accept()
    {
        NarrationManager.Instance.ProcessLoadScene(this);
    }

    //The scene that is to be loaded
    public Scenes.ScenesList scene;
}
