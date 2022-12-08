using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A NarrationSequence for loading a new scene
//The game will save every time a scene is loaded
//the way we will implement this is by making load scene 
//sequence be a save scene sequence
[CreateAssetMenu(fileName = "LoadSceneSequence", menuName = "LoadSceneSequence")]
public class LoadSceneSequence : SaveSequence
{
    //the Accept function for the visitor pattern
    public override void Accept()
    {
        NarrationManager.Instance.ProcessLoadScene(this);
    }

    //The scene that is to be loaded
    public Scenes.ScenesList scene;

    //A boolean to determine if a scene is a game scene or not
    //If it is a game scene then the sequence will perform additional
    //processing after the scene is loaded (such as saving the game).
    public bool isGameScene;
}
