using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
///
/// This struct reference to a line/sentence in a dialogue. In each sentence, there will be a Character speaking and the text in which they say.
///
public struct Sentence
{
    public Character character; //Each sentence should have it's own speaker.

    [TextArea(2,4)] //This is to make it easier to edit and add sentences in the Unity Editor.
    public string text;
};

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
    public Sentence[] sentences; //Number of sentences in this particular dialogue
}
