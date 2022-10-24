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

        turnValue = 0;
    }

    private void OnEnable()
    {
        CheckGroundStatus();

        //Update to walk speed
        speed = walkSpeed;
    }

    //a helper function to check if the player is on the ground
    //and update values accordingly
    private bool CheckGroundStatus()
    {
        if (IsGrounded())
        {
            //Set the rigidbody to kinematic when it is grounded
            self.rigidbody.isKinematic = true;

            onGround = true;

            return true;
        }
        else
        {
            //and to nonkinematic when it is not grounded
            self.rigidbody.isKinematic = false;

            onGround = false;

            return false;
        }
    }

    protected override void FixedUpdate()
    {
        //check to see if the player is on the ground or in midair
        //I will likely changed this later it doesn't have to check
        //every update loop
        if(CheckGroundStatus())
        {
            //On ground 

            //move the player 
            // self.rigidbody.MovePosition(self.rigidbody.position + (moveVector * Time.fixedDeltaTime));
            Move(moveVector * Time.fixedDeltaTime);
            // Debug.Log(moveVector * Time.fixedDeltaTime);
            // self.rigidbody.AddForce((moveVector * Time.fixedDeltaTime));//, ForceMode.VelocityChange);
            // character.Move(self.rigidbody.position + (moveVector * Time.fixedDeltaTime));

            //rotate the player
            Quaternion newRotation = self.rigidbody.rotation * Quaternion.Euler(0, turnValue * Time.fixedDeltaTime, 0);
            self.rigidbody.rotation = newRotation;
        }
        else
        {
            //In Midair
        }
    }

    //helper function to move the player
    private void Move(Vector3 displacement)
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(self.rigidbody.position, displacement, out hitInfo, displacement.magnitude))
        {
            //collider in the way, move to the hit position
            self.rigidbody.MovePosition(hitInfo.point);
        }
        else
        {
            //path is clear, move to location
            Debug.Log(self.rigidbody.position + displacement);
            self.rigidbody.MovePosition(self.rigidbody.position + displacement);
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
        //If on ground then jump otherwise transition to Hover
        if (onGround)
        {
            //On Ground, jump into the air
            if (!input.isPressed)
            {
                //enable forces
                self.rigidbody.isKinematic = false; 

                //jump with impulse
                self.rigidbody.AddForce(Vector3.up, ForceMode.Impulse);
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
