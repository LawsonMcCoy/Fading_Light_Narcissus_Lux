using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A NarrationSequence for delaying the narration by some amount of time
[CreateAssetMenu(fileName = "DelaySequence", menuName = "DelaySequence")]
public class DelaySequence : NarrationSequence
{
    //the Accept function for the visitor pattern
    public override void Accept()
    {
        NarrationManager.Instance.ProcessDelay(this);
    }

    //data
    public float delayTime;
}
