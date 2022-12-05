using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public float maxStandingSlopeAngle; //The max angle a slope can be for player to still
                                        //be able to stand on it

    public float maxForce;

    public float inputDelayTime;

    //Dashing data
    public float dashStaminaCost; //The stamina cost of performing a dash
    public float dashDistance; //How far the player will dash when the button is pressed
    public float dashTimeStep; //The amount of time between each step of the dash
    public float dashDistanceStep = 0.1f; //The amount of distance covered in one time step of the dash
                                          //note 0.0f will break the game
    public float dashBounceHorizontal; //the amount that the player bounces off the wall in the horizontal direction
    public float dashBounceVertical;   //the amount that hte player bounces off the wall in the veritcal direction
    public LayerMask dashBounceMask; //The layer mask for all layers the player can bounce off of.

    // Ui data
    public string controlUiParentName;
}
