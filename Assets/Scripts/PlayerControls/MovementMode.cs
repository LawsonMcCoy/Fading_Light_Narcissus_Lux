using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//I will be breaking up the three movement modes up into different 
//scripts. This is to avoid a bunch of switch statements, and 
//to have a cleaner code base. This class will serve as the parent
//class for each movement mode, so any common behavior can be 
//implemented here
public abstract class MovementMode : MonoBehaviour
{
    [SerializeField] protected MovementModeData commonData; //A scriptable object containing all serialize data
    [SerializeField] protected MovementMode[] movementModes = new MovementMode[3]; //an array of the three movement modes scipts
                                                                                   //index 0 is walking
                                                                                   //index 1 is hovering
                                                                                   //index 2 is flying

    //an enum for the movement modes define in the order 
    //walking, hovering, and flying, so that there int values
    //matches the index of the corresponding movement mode in 
    //the array
    public enum Modes
    {
        WALKING,
        HOVERING,
        FLYING
    }

    protected Player self; //a reference to yourself

    protected Vector3 moveVector;
    protected float speed; //speed variable to be set by the child class

    protected bool inputReady; //A variable value to prevent immediately transiting back to 
                                //a mode that you just transition from when the button to transition
                                //is the same

    protected virtual void Awake()
    {
        //get the reference to yourself
        self = this.GetComponent<Player>();

        //initial the move vector to zero
        moveVector = Vector3.zero;

        inputReady = true;
    }

    protected virtual void FixedUpdate()
    {
        //cap the speed
        if (self.rigidbody.velocity.magnitude > commonData.maxSpeed)
        {
            self.rigidbody.velocity = self.rigidbody.velocity.normalized * commonData.maxSpeed;
        }
    }

    //A function to transition from one movement mode to another.
    //It will enable the script being transitioned to and disable its
    //own script. NOTE calling this function to transition to itself
    //will break the game
    protected void Transition(Modes transitionToMode)
    {
        //by defualt enabled only prevent Update and FixedUpdate from
        //being called. Other functions should be manually checked
        //if the script is enabled  
        if (this.enabled)
        {
            //enable new movement mode
            Debug.Log($"Transitioning to {transitionToMode}");
            movementModes[(int)transitionToMode].enabled = true;

            //disable current mode
            this.enabled = false;
        }
    }

    //A simple raycast frunction to check if the player is on the ground
    //returns true if the player is one the ground and false if they are
    //in the air
    protected bool IsGrounded()
    {
        return Physics.Raycast(this.transform.position, Vector3.down, commonData.isGroundedCheckDistance + 0.1f);  //The last 0.1 is in case the raycast ends on the surface of the ground 
    }

    protected void AddForce(Vector3 force, ForceMode mode)
    {
        //limit the force to max force
        if (force.magnitude > commonData.maxForce)
        {
            force = force.normalized * commonData.maxForce;
        }

        //apply force to rigidbody
        self.rigidbody.AddForce(force, mode);
    }

    protected IEnumerator DelayInput()
    {
        inputReady = false;

        yield return new WaitForSeconds(commonData.inputDelayTime);

        inputReady = true;
    }

    //Player input
    protected virtual void OnMove(InputValue input)
    {
        //x component is turning, y component is tilting
        Vector2 inputVector = input.Get<Vector2>();
        
        //Note that when going from 2D vector to 3D vector, the y in 2D becomes the z in 3D, Thanks Unity
        moveVector = inputVector.x * speed * this.transform.right + inputVector.y * speed * this.transform.forward;
    }
}
