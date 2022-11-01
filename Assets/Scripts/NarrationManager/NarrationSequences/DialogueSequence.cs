using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A NarrationSequence for starting a dialogue
[CreateAssetMenu(fileName = "DialogueSequence", menuName = "DialogueSequence")]
public class DialogueSequence : NarrationSequence
{
    //the Accept function for the visitor pattern
    public override void Accept()
    {
        NarrationManager.Instance.ProcessDialogue(this);
    }

    //data
    public Dialogue dialogueToStart;
}
