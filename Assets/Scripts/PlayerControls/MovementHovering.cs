using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementHovering : MovementMode
{
    [SerializeField] private float hoverSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float passiveStaminaLostRate; //The amount of stamina lost per second while hovering

    private float turnValue;

    private void Awake()
    {
        base.Awake();

        turnValue = 0;

        //Update to hover speed
        speed = hoverSpeed;
    }

    private void OnEnable()
    {
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
    }

    protected override void FixedUpdate()
    {
        //move the player 
        // self.rigidbody.position += moveVector * Time.fixedDeltaTime;
        Vector3 horizontalVelocity = self.rigidbody.velocity;
        horizontalVelocity.y = 0.0f; //set vertical component to zero
        self.rigidbody.AddForce(moveVector - self.rigidbody.velocity, ForceMode.Force);

        //rotate the player
        Quaternion newRotation = self.rigidbody.rotation * Quaternion.Euler(0, turnValue * Time.fixedDeltaTime, 0);
        self.rigidbody.rotation = newRotation;

        //consume stamina while in hovering mode
        stamina.Subtract(passiveStaminaLostRate * Time.fixedDeltaTime); //lose stamina

        //if all stamina has be lost transition to walking
        if (stamina.ResourceAmount() == 0)
        {
            Transition(Modes.WALKING);
        }
    }

    //************
    //player input
    //************

    //mouse input
    private void OnTurn(InputValue input)
    {
        Vector2 inputVector = input.Get<Vector2>();

        //the x component will rotate the player
        turnValue = inputVector.x * turnSpeed;
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
