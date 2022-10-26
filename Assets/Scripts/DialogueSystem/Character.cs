using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character")]
public class Character : ScriptableObject
{
    //This SO should hold information about the speaker in the current dialogue.
    //If we wanted to add a sprite for the character, we can store it HERE.
    public string fullName;
    //[SerializeField] private Sprite speakerPortrait;
}
