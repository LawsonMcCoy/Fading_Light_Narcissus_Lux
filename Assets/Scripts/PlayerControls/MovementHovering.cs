using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementHovering : MovementMode
{
    [SerializeField] private float hoverSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float passiveStaminaLostRate; //The amount of stamina lost per second while hovering
    [SerializeField] private float hoveringDampingCoefficient; //The damping coefficient to the stop the player


    private float turnValue;

    //testing variables
    [SerializeField] private bool alternateHover;
    [SerializeField] private float hoverFallBase;
    [SerializeField] private float hoverFallExponent;
    [Tooltip("The rate that the hover fall rate recovery to zero when Ika is being pushed up by wind")]
    [SerializeField] private float hoverRecoveryRate;
    [SerializeField] private float walkModeHoverPunishment;
    private float hoverTime; //The amount of time Ika as been hovering, used to calculate hover fall rate

    protected override void Awake()
    {
        base.Awake();

        turnValue = 0;

        //Update to hover speed
        speed = hoverSpeed;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        StartCoroutine(DelayInput());

        
        //disable rotation
        self.rigidbody.freezeRotation = true;

        //reset the rotate transform.up is the same as Vector3.up
        Vector3 currentEuler = self.rigidbody.rotation.eulerAngles; //get the Euler angles
        currentEuler.x = 0.0f; //set the rotation around x axis to 0
        currentEuler.z = 0.0f; //set the rotation around z axis to 0
        //Now we only have an rotation around the y axis, so up with this rotation is Vector3.up
        // self.rigidbody.rotation = Quaternion.Euler(currentEuler);
        self.rigidbody.MoveRotation(Quaternion.Euler(currentEuler));

        //stop vertical motion
        //The formule is negaitve y velocity (to stop the player) plus the exponential (to reduce the power)
        //If the exponential has a larger magnitude then just set the scale factor to zero (don't affect veritical motion)
        float verticalStopScaleFactor = Mathf.Pow(hoverFallBase, hoverFallExponent * hoverTime);
        if (verticalStopScaleFactor >= Mathf.Abs(self.rigidbody.velocity.y))
        {
            verticalStopScaleFactor = 0;
        }
        else 
        {
            verticalStopScaleFactor -=self.rigidbody.velocity.y;
        }
        AddForce(verticalStopScaleFactor * Vector3.up, ForceMode.Impulse);

        modeUIColor = new Color(0f, 0.8f, 0f, 1f);
        movementModeText.color = modeUIColor;

        controlUi.TransitionHoverUI();
        controlUi.IndicateModeChange();
    }

    protected override void FixedUpdate()
    {
        //make sure that MovementMode fixed update is called first
        base.FixedUpdate();

        //update the hover time
        if (self.rigidbody.velocity.y > 0)
        {
            //if something is pushing the player up, then reduce the hover time
            hoverTime = Mathf.MoveTowards(hoverTime, 0, hoverRecoveryRate); //The move towards is used to prevent negative times
        }
        else
        {
            //Otherwise counts the seconds the player has been hovering for
            hoverTime += Time.fixedDeltaTime;
        }

        //Calculate the hover force to keep the player in the air
        Vector3 hoverForce = -Physics.gravity - Mathf.Pow(hoverFallBase, hoverFallExponent * hoverTime) * Vector3.up;
        if (Vector3.Dot(Physics.gravity, hoverForce) > 0)
        {
            hoverForce = Vector3.zero; //Don't apply hoverforce after it becomes a downward force
        }
        // Debug.Log($"Hover time: {hoverTime}, Hover force: {hoverForce}, Relative wind: {relativeWind}");

        //move the player 
        Vector3 moveForce = Vector3.zero;
        if (!forceNoMovement)
        {
            moveForce = hoveringDampingCoefficient * (moveVector - self.rigidbody.velocity); //use controls to determine the horizontal components
            moveForce.y = 0; //player input does not affect motion in the vertical direction
            // if (self.rigidbody.velocity.y <= 0)
            // {
            //     moveForce.y = 0; //player input does not affect motion in the vertical direction
            // }
        }

        //apply the forces
        AddForce(moveForce + hoverForce, ForceMode.Force);

        //rotate the player
        Quaternion newRotation = self.rigidbody.rotation * Quaternion.Euler(0, turnValue * Time.fixedDeltaTime, 0);
        self.rigidbody.rotation = newRotation;

        //if you land on the ground then transition to walking
        if (IsGrounded())
        {
            //reset the hover time when the player touches the ground
            hoverTime = 0f;

            Transition(Modes.WALKING);
        }
    }

    //A visitor function to determine which type of movement mode this script is
    public override void GetMovementUpdate(MovementUpdateReciever updateReciever)
    {
        updateReciever.HoverUpdate(this);
    }

    //zeroing out rotational motion during movement restricted events
    public override void StartMovementRestrictedEvent()
    {
        //zero parent's motion
        base.StartMovementRestrictedEvent();

        //zero rotational motion
        turnValue = 0;
    }

    //************
    //player input
    //************

    //mouse input
    protected override void OnLook(InputValue input)
    {
        base.OnLook(input);

        //the x component will rotate the player
        turnValue = mouseInput.x * turnSpeed;
    }

    //Shift key input
    private void OnSprintFly(InputValue input)
    {
        // Debug.Log($"Hover space {input.isPressed}");

        if (input.isPressed)
        {
            Transition(Modes.GLIDING);
        }
    }

    //Space key input
    private void OnJumpTransition(InputValue input)
    {
        if (input.isPressed && inputReady)
        {
            //punish the player with extra hover time
            hoverTime += walkModeHoverPunishment;

            Transition(Modes.WALKING);
            StartCoroutine(DisableControlForTime(commonData.transitionMovementLockTime));
        }
    }
}
