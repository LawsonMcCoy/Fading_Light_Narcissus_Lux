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

        Debug.Log("Now hovering");
        
        //disable rotation
        self.rigidbody.freezeRotation = true;

        //reset the rotate transform.up is the same as Vector3.up
        Vector3 currentEuler = self.rigidbody.rotation.eulerAngles; //get the Euler angles
        currentEuler.x = 0.0f; //set the rotation around x axis to 0
        currentEuler.z = 0.0f; //set the rotation around z axis to 0
        //Now we only have an rotation around the y axis, so up with this rotation is Vector3.up
        // self.rigidbody.rotation = Quaternion.Euler(currentEuler);
        self.rigidbody.MoveRotation(Quaternion.Euler(currentEuler));

        //stop all motion
        // self.rigidbody.velocity = Vector3.zero;

        //just started hovering, so set hoverTime to zero
        hoverTime = 0f;

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
        hoverTime += Time.fixedDeltaTime;

        //Calculate the hover force to keep the player in the air
        Vector3 hoverForce = -Physics.gravity - Mathf.Pow(hoverFallBase, hoverFallExponent * hoverTime) * Vector3.up;
        if (Vector3.Dot(Physics.gravity, hoverForce) > 0)
        {
            hoverForce = Vector3.zero; //Don't apply hoverforce after it becomes a downward force
        }

        //move the player 
        Vector3 moveForce = Vector3.zero;
        if (!forceNoMovement)
        {
            moveForce = hoveringDampingCoefficient * (moveVector - self.rigidbody.velocity); //use controls to determine the horizontal components
            if (self.rigidbody.velocity.y <= 0)
            {
                moveForce.y = 0; //player input does not affect motion in the vertical direction
            }
        }

        //apply the forces
        AddForce(moveForce + hoverForce, ForceMode.Force);

        //rotate the player
        Quaternion newRotation = self.rigidbody.rotation * Quaternion.Euler(0, turnValue * Time.fixedDeltaTime, 0);
        self.rigidbody.rotation = newRotation;

        //if you land on the ground then transition to walking
        if (IsGrounded())
        {
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
            Transition(Modes.WALKING);
            StartCoroutine(DisableControlForTime(commonData.transitionMovementLockTime));
        }
    }
}
