using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A NarrationSequence for starting an objective
[CreateAssetMenu(fileName = "ObjectiveSequence", menuName = "ObjectiveSequence")]
public class ObjectiveSequence : NarrationSequence
{
    //the Accept function for the visitor pattern
    public override void Accept()
    {
        NarrationManager.Instance.ProcessObjective(this);
    }

    //data
    public ObjectiveScriptableObject objective;
}
