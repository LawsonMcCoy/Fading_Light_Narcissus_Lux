using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A NarrationSequence is one part of the games narration
//It could be loading a new scene, a save point, an objective
//for the player to complete, spawning entities, etc
//The visitor desgin pattern will be used here to allow
//NarrationManager to process each NarrationSequence
public abstract class NarrationSequence : ScriptableObject
{
    //The accept function for the visitor pattern
    //I am ommiting the visitor parameter because
    //NarrationManager is the visitor and it is 
    //already a singleton
    abstract public void Accept();
    virtual public void Cancel()
    {

    }
}
