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
        //disable gravity
        self.rigidbody.useGravity = false;

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
        self.rigidbody.velocity = Vector3.zero;

        //just started hovering, so set hoverTime to zero
        hoverTime = 0f;

        modeUIColor = new Color(0f, 0.8f, 0f, 1f);
        movementModeText.color = modeUIColor;

        // controlUi.TransitionHoverUI();
        // controlUi.IndicateModeChange();
    }

    protected override void FixedUpdate()
    {
        //update the hover time
        hoverTime += Time.fixedDeltaTime;

        //move the player 
        // self.rigidbody.position += moveVector * Time.fixedDeltaTime;
        Vector3 horizontalVelocity = self.rigidbody.velocity;
        // horizontalVelocity.y = 0.0f; //set vertical component to zero
        if (alternateHover)
        {
            moveVector.y = -Mathf.Pow(hoverFallBase, hoverFallExponent * hoverTime); 
        }
        self.rigidbody.AddForce(hoveringDampingCoefficient * (moveVector - self.rigidbody.velocity), ForceMode.Force);

        //rotate the player
        Quaternion newRotation = self.rigidbody.rotation * Quaternion.Euler(0, turnValue * Time.fixedDeltaTime, 0);
        self.rigidbody.rotation = newRotation;

        if (!alternateHover)
        {
            //consume stamina while in hovering mode
            stamina.Subtract(passiveStaminaLostRate * Time.fixedDeltaTime); //lose stamina
        }
        else
        {
            //slowing falls in hover mode
            // AddForce(hoverFallRate * Time.fixedDeltaTime * Vector3.down, ForceMode.Force);
        }

        if (!alternateHover)
        {
            //if all stamina has be lost transition to walking
            if (stamina.ResourceAmount() == 0)
            {
                Transition(Modes.WALKING);
            }
        }
        else
        {
            //if you land on the ground then transition to walking
            if (IsGrounded())
            {
                Transition(Modes.WALKING);
            }
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
            Transition(Modes.FLYING);
        }
    }

    //Space key input
    private void OnJumpTransition(InputValue input)
    {
        if (input.isPressed && inputReady)
        {
            Transition(Modes.WALKING);
        }
    }
}
