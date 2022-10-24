using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A scriptable object containing all the data for the abstract 
//class MovementMode. The purpose of this scriptable object is
//to prevent us from having a copy of MovementMode's data from 
//each movement mode. Now MovementMode can have a single serialize
//field that is of type MovementModeData, and we can populate the 
//data in one place (this scriptable object)
[CreateAssetMenu(fileName = "MovementModeData", menuName = "MovementModeData")]
public class MovementModeData : ScriptableObject
{
    public float maxSpeed; //a value for a max speed to prevent the game from breaking
    public float isGroundedCheckDistance; //the distance that the player needs to be 
                                          //off the ground in order to be considered in
                                          //the air

    public float maxForce;
}
