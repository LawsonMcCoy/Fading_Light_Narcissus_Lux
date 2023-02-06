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
        GLIDING
    }

    protected Player self; //a reference to yourself

    //true when the player is dash and false otherwise
    public bool isDashing
    {
        get;
        private set;
    }

    protected static bool forceNoMovement = false; //when true the player cannot walk or move while hovering

    private Vector2 wasdInput;
    protected Vector3 moveVector;
    protected float speed; //speed variable to be set by the child class

    protected bool inputEnabled = true; //A boolean value to determine player control should be enabled
    protected bool inputReady; //A variable value to prevent immediately transiting back to 
                               //a mode that you just transition from when the button to transition
                               //is the same

    //drag
    [Tooltip("a fine toning value for regular drag (this is combination of all forms of drag expect for induced drag, it is also scaled by drageScalingValues)")]
    [SerializeField] private float coefficientOfDrag; //a fine toning value for regular drag
    [Tooltip("A scaling vector to allow drag to unevenly applied in different directions")]
    [SerializeField] private Vector3 dragScalingValues; //A scaling vector to allow drag to unevenly applied in different directions

    //Wind
    private Vector3 absoluteWind = Vector3.zero;
    public Vector3 relativeWind 
    {
        get;
        protected set;
    } //the wind for Ika's frame of reference
    [SerializeField] private LayerMask windTunnelMask;

    #region Ui Section
    [SerializeField] protected Text movementModeText;
    protected Color modeUIColor;
    protected ControlUi controlUi;
    #endregion

    //Input data
    public Vector2 mouseInput
    {
        get;
        private set;
    }

    protected virtual void Awake()
    {
        //get the reference to yourself
        self = this.GetComponent<Player>();

        //initial the move vector to zero
        moveVector = Vector3.zero;

        inputReady = true;

        controlUi = GameObject.Find(commonData.controlUiParentName).GetComponent<ControlUi>();
    }

    protected virtual void OnEnable()
    {
        //inform the player class that the active movement mode has changed
        //pass in reference to new active movement mode (this)
        self.activeMovementMode = this;
    }

    protected virtual void Start()
    {
        //lock the curser when controlling the player
        Cursor.lockState = CursorLockMode.Locked;

        //Event subscriptions
        EventManager.Instance.Subscribe(EventTypes.Events.DIALOGUE_START, StartMovementRestrictedEvent);
        EventManager.Instance.Subscribe(EventTypes.Events.DIALOGUE_START, DisableInput);
        EventManager.Instance.Subscribe(EventTypes.Events.DIALOGUE_END, EnableInput);
    }

    protected virtual void FixedUpdate()
    {
        //update the moveVector
        moveVector = wasdInput.x * speed * this.transform.right + wasdInput.y * speed * this.transform.forward;

        //compute new relative wind value
        relativeWind = absoluteWind - self.rigidbody.velocity;
        // Debug.Log($"relative wind {relativeWind}, absolute wind {absoluteWind}"); 

        //use the relativeWind to compute the regular drag force
        Vector3 localDragScaleValues = Quaternion.Inverse(self.rigidbody.rotation) * dragScalingValues;
        Vector3 drag = coefficientOfDrag * Vector3.Scale(relativeWind, localDragScaleValues);

        //apply the drag force
        AddForce(drag, ForceMode.Force);
    }

    private void OnTriggerEnter(Collider triggered)
    {
        Debug.Log("Trigger Enter");
        if (Utilities.ObjectInLayer(triggered.gameObject, windTunnelMask))
        {
            Debug.Log("IN layer");
            //entered a wind tunnel, add its wind to the absolute wind
            absoluteWind += triggered.GetComponent<WindTunnel>().getWindValue();
        }
    }

    private void OnTriggerExit(Collider triggered)
    {
        if (Utilities.ObjectInLayer(triggered.gameObject, windTunnelMask))
        {
            //exited a wind tunnel, subtract its wind to the absolute wind
            absoluteWind -= triggered.GetComponent<WindTunnel>().getWindValue();
        }
    }

    //A visitor function to determine which type of movement mode this script is
    public abstract void GetMovementUpdate(MovementUpdateReciever updateReciever);

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
        if (Physics.Raycast(self.center, Vector3.down, out groundedInfo, commonData.isGroundedCheckDistance + 0.1f))  //The last 0.1 is in case the raycast ends on the surface of the ground 
        {
            //if the raycast collides with the ground, check to make sure the slope is not too steep to stand on

            //compute slope angle
            float slopeAngle = Vector3.Angle(Vector3.up, groundedInfo.normal);
            //Debug.Log($"Slope Angle: {slopeAngle}");
            //The player is only grounded iff the slope is not too steep to stand on
            return slopeAngle <= commonData.maxStandingSlopeAngle;
        }
        else
        {
            //didn't even detect the ground, definitely not grounded
            return false;
        }
    }

    //overload of IsGrounded so it can be called without returning the RaycastHit
    protected bool IsGrounded()
    {
        RaycastHit unsedGroundedInfo;
        return IsGrounded(out unsedGroundedInfo);
    }

    protected void AddForce(Vector3 force, ForceMode mode)
    {
        //limit the force to max force
        // if (force.magnitude > commonData.maxForce)
        // {
        //     force = force.normalized * commonData.maxForce;
        // }

        //apply force to rigidbody
        Debug.DrawLine(transform.position, transform.position + force, Color.red);
        self.rigidbody.AddForce(force, mode);
    }

    //A function design to set all motion to zero
    //The initially purpose of this function is to fix a bug
    //where motion persists after player input is disabled.
    public virtual void StartMovementRestrictedEvent()
    {
        moveVector = Vector3.zero;
    }

    //A simple function to enable player input for controlling the player
    public void EnableInput()
    {
        inputEnabled = true;
    }

    //A simple function to diable player input for controlling the player
    public void DisableInput()
    {
        inputEnabled = false;
    }

    protected virtual void OnDestroy()
    {
        //Events unsubscriptions
        EventManager.Instance.Unsubscribe(EventTypes.Events.DIALOGUE_START, StartMovementRestrictedEvent);
        EventManager.Instance.Unsubscribe(EventTypes.Events.DIALOGUE_START, DisableInput);
        EventManager.Instance.Unsubscribe(EventTypes.Events.DIALOGUE_END, EnableInput);
    }

    protected IEnumerator DelayInput()
    {
        inputReady = false;

        yield return new WaitForSeconds(commonData.inputDelayTime);

        inputReady = true;
    }

    protected IEnumerator PerformDash(Vector3 dashVector)
    {
        yield return PerformDash(dashVector, Vector3.zero);
    }

    protected IEnumerator PerformDash(Vector3 dashVector, Vector3 bounceForce)
    {
        Vector3 startingPosition = self.rigidbody.position; //the starting position of the dash
        Vector3 endingPosition = startingPosition + dashVector; //the ending position of the dash
        float dashDistance = 0.0f; //the distance the dash as already covered
        float percentageTravel = 0.0f; //how far the play has travel in form of the percentage of total distance

        //mark the player as dashing
        isDashing = true;

        //loop until the player is at their destination
        while (percentageTravel < 1.0f)
        {
            //go another distance step
            dashDistance += commonData.dashDistanceStep;

            //move the player to new place in the dash
            percentageTravel = dashDistance / dashVector.magnitude;
            self.rigidbody.MovePosition(Vector3.Lerp(startingPosition, endingPosition, percentageTravel));
            // self.transform.position = Vector3.Lerp(startingPosition, endingPosition, percentageTravel);

            //wait for the next time step
            yield return new WaitForSeconds(commonData.dashTimeStep);
        }

        //Add the bounce force and disable movement to prevent overwriting the bounce force
        if (bounceForce != Vector3.zero)
        {
            AddForce(bounceForce, ForceMode.Impulse);
            StartCoroutine(DisableControlForTime(1.0f)); //To do change to inspector variable
        }

        //dash is completed, mark the player as not dashing
        isDashing = false;
    }

    //Most of the difference in computation for dashing between different
    //movement modes is how the direction of the dash is computed and animation.
    //There this function will take a unit vector representing the dash direction
    //and compute the magnitude of the dashVector. Once it scales the dashVector by
    //the computed magnitude, it will perform the dash action.
    protected void ComputeDashVector(Vector3 dashVector)
    {
        //do nothing if dash vector is zero, you don't have enough stamina, or you are already dashing
        if (dashVector != Vector3.zero && stamina.ResourceAmount() >= commonData.dashStaminaCost && !isDashing)
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
            if (Physics.Raycast(self.center, dashVector, out dashInfo, commonData.dashDistance))
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
            if (dashVector.magnitude >= colliderBufferDistance)
            {
                //You have enough space to dash
                StartCoroutine(PerformDash(dashVector));
            }
            else if (Utilities.ObjectInLayer(dashInfo.collider.gameObject, commonData.dashBounceMask)) //prevent bouncing on nonbouncable objects
            {
                Vector3 cancelForce = Vector3.Project(self.rigidbody.velocity, dashInfo.normal); //cancel out the current velocity in the normal direction
                //dashing into an object you are standing next to
                //bounce off of it at 
                Vector3 bounceForceHorizontal = commonData.dashBounceHorizontal * dashInfo.normal;
                Vector3 bounceForceVertical = commonData.dashBounceVertical * Vector3.up;
                StartCoroutine(PerformDash(dashVector, bounceForceHorizontal + bounceForceVertical - cancelForce));
            }
        }
    }

    protected IEnumerator DisableControlForTime(float disableTime)
    {
        forceNoMovement = true;

        yield return new WaitForSeconds(disableTime);

        forceNoMovement = false;
    }

    //Player input
    protected virtual void OnMove(InputValue input)
    {
        //x component is turning, y component is tilting
        wasdInput = input.Get<Vector2>();
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

    protected virtual void OnLook(InputValue input)
    {
        mouseInput = input.Get<Vector2>();
    }

    // private void OnTestNotUsedForGameplay(InputValue input)
    // {
    //     // Vector2 value = input.Get<Vector2>;
    //     Debug.Log($"Test input value {input.Get<Vector2>()}");
    // }
}