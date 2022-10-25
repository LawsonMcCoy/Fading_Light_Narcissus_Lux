using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct Sentence
{
    public Character character; //Each sentence should have it's own speaker.

    [TextArea(2,4)] //This is to make it easier to edit and add sentences in the Unity Editor.
    public string text;
};


[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue")]
public class Dialogue : ScriptableObject
{
    public Character[] characters; //Depending on the number of speakers in a dialogue
    public Sentence[] sentences; //Number of sentences in this particular dialogue
}
