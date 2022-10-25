using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementWalking : MovementMode
{
    [SerializeField] private float walkSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float jumpForce; //How strong the player can jump

    private float turnValue;
    private bool onGround; //A bool value that is true when on the ground and false otherwise
                           //updated in the CheckGroundStatus function

    private void Awake()
    {
        base.Awake();

        //Update to walk speed
        speed = walkSpeed;

        turnValue = 0;
    }

    private void OnEnable()
    {
        StartCoroutine(DelayInput());

        CheckGroundStatus();

        //Enable gravity when walking
        self.rigidbody.useGravity = true; 

        //Disable physics rotation
        self.rigidbody.freezeRotation = true;
    }

    //a helper function to check if the player is on the ground
    //and update values accordingly
    private bool CheckGroundStatus()
    {
        if (IsGrounded())
        {
            onGround = true;

            return true;
        }
        else
        {
            onGround = false;

            return false;
        }
    }

    protected override void FixedUpdate()
    {
        //check to see if the player is on the ground or in midair
        //I will likely changed this later it doesn't have to check
        //every update loop
        // Debug.Log(CheckGroundStatus());
        if(CheckGroundStatus())
        {
            //On ground 

            //move the player 
            self.rigidbody.MovePosition(self.rigidbody.position + (moveVector * Time.fixedDeltaTime));

            //rotate the player
            Quaternion newRotation = self.rigidbody.rotation * Quaternion.Euler(0, turnValue * Time.fixedDeltaTime, 0);
            self.rigidbody.rotation = newRotation;
        }
        else
        {
            //In Midair
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

    //space key input
    private void OnJumpTransition(InputValue input)
    {
        if (inputReady)
        {
            //If on ground then jump otherwise transition to Hover
            if (onGround)
            {
                //On Ground, jump into the air
                if (!input.isPressed)
                {
                    //jump with impulse
                    self.rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                }
            }
            else
            {
                //In midair, transition into Hovering
                if (input.isPressed)
                {
                    Transition(Modes.HOVERING);
                }
            }
        }
    }

    //Shift key input
    private void OnSprintFly(InputValue input)
    {
        if (inputReady)
        {
            if (onGround)
            {
                //TODO implement sprinting
            }
            else
            {
                if (input.isPressed)
                {
                    Transition(Modes.FLYING);
                }
            }
        }
    }
}
