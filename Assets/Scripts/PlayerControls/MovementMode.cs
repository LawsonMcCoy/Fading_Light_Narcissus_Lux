using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    [SerializeField] protected StaminaManager stamina; //A reference to the stamina manager for the player

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

    [SerializeField] protected Text movementModeText;
    protected Color modeUIColor;

    protected virtual void Awake()
    {
        //get the reference to yourself
        self = this.GetComponent<Player>();

        //initial the move vector to zero
        moveVector = Vector3.zero;

        inputReady = true;
    }

    private void Start()
    {
        //lock the curser when controlling the player
        Cursor.lockState = CursorLockMode.Locked;
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

            movementModeText.text = transitionToMode.ToString();

            //disable current mode
            this.enabled = false;
        }
    }

    //A simple raycast frunction to check if the player is on the ground
    //returns true if the player is one the ground and false if they are
    //in the air
    protected bool IsGrounded(out RaycastHit groundedInfo)
    {
        return Physics.Raycast(this.transform.position, Vector3.down, out groundedInfo, commonData.isGroundedCheckDistance + 0.1f);  //The last 0.1 is in case the raycast ends on the surface of the ground 
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

    protected IEnumerator PerformDash(Vector3 dashVector)
    {
        Vector3 startingPosition = self.rigidbody.position; //the starting position of the dash
        Vector3 endingPosition = startingPosition + dashVector; //the ending position of the dash
        float dashDistance = 0.0f; //the distance the dash as already covered
        float percentageTravel = 0.0f; //how far the play has travel in form of the percentage of total distance

        //loop until the player is at their destination
        while (percentageTravel < 1.0f)
        {
            //go another distance step
            dashDistance += commonData.dashDistanceStep;

            //move the player to new place in the dash
            percentageTravel = dashDistance / dashVector.magnitude;
            self.rigidbody.MovePosition(Vector3.Lerp(startingPosition, endingPosition, percentageTravel));

            //wait for the next time step
            yield return new WaitForSeconds(commonData.dashTimeStep);
        }
    }

    //Most of the difference in computation for dashing between different
    //movement modes is how the direction of the dash is computed and animation.
    //There this function will take a unit vector representing the dash direction
    //and compute the magnitude of the dashVector. Once it scales the dashVector by
    //the computed magnitude, it will perform the dash action.
    protected void ComputeDashVector(Vector3 dashVector)
    {
        //do nothing if you don't have enough stamina
        if (stamina.ResourceAmount() >= commonData.dashStaminaCost)
        {
            RaycastHit dashInfo; //The information for about the dash from the raycast

            //colliderBufferDistance is the distance that must be maintain between the player and another collider to avoid
            //collider clipping. This is because the player is at it's center of mass, which is the center of the player. If we
            //stop to player that the contact point of the raycast, then that is where the center of mass will be and half the 
            //player will still be in the other collider. Right I am setting the buffer distance from the center of mass to the 
            //corners of the player (seeing the player as a rectanglular prism). 
            float xDistance = self.transform.lossyScale.x / 2;
            float yDistance = self.transform.lossyScale.y / 2;
            float zDistance = self.transform.lossyScale.z / 2;
            float colliderBufferDistance = Mathf.Sqrt((xDistance * xDistance) + (yDistance * yDistance) + (zDistance * zDistance));
            
            //raycast to see if the dash path is clear
            if (Physics.Raycast(self.rigidbody.position, dashVector, out dashInfo, commonData.dashDistance))
            {
                //if path is not clear use RaycastHit.distance and colliderBufferDistance to scale the direction vector
                dashVector = (dashInfo.distance - colliderBufferDistance) * dashVector;
            }
            else
            {
                //if path is clear is use the dashDistance to scale the direcciton vector
                dashVector = commonData.dashDistance * dashVector;
            }

            //pay the stamina cost
            stamina.Subtract(commonData.dashStaminaCost);

            //use rigidboby.MovePosition to preform dash (hopefully it will take care of interpolation)
            //if rigidbody's interpolation doesn's work then use lerp to interpolate
            StartCoroutine(PerformDash(dashVector));
        }
    }

    //Player input
    protected virtual void OnMove(InputValue input)
    {
        //x component is turning, y component is tilting
        Vector2 inputVector = input.Get<Vector2>();
        
        //Note that when going from 2D vector to 3D vector, the y in 2D becomes the z in 3D, Thanks Unity
        moveVector = inputVector.x * speed * this.transform.right + inputVector.y * speed * this.transform.forward;
    }

    protected virtual void OnDash(InputValue input)
    {
        if (this.enabled)
        {
            Vector3 dashVector; //A vector that represents the path the player dash is taking

            //Get the dash direction from the moveVector
            //we will dash in the direction we are moving in
            dashVector = moveVector.normalized;

            //Compute the magnitude of dashVector, and perform the dash
            ComputeDashVector(dashVector);
        }
    }
}