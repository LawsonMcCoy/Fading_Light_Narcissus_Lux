using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementWalking : MovementMode
{
    [SerializeField] private float walkSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float jumpForceVertical; //How strong the player can jump upwards
    [SerializeField] private float jumpForceHonrizontal; //How strong the player can jump horizontally
    [SerializeField] private float jumpForceStationary; //How strong the player can jump when not moving
    [SerializeField] private float staminaRegainRate; //The amount of stamina regain per second while walking
    [SerializeField] private float dashJumpForce; //How strong the player can jump at the end of dash


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

        modeUIColor = new Color(1f, 0.8f, 0f, 1f);
        movementModeText.color = modeUIColor;
    }

    //a helper function to check if the player is on the ground
    //and update values accordingly
    private bool CheckGroundStatus()
    {
        RaycastHit groundInfo;
        if (IsGrounded(out groundInfo))
        {
            onGround = true;

            //Disable physics rotation
            self.rigidbody.freezeRotation = true;

            //reset the rotate transform.up is the same as Vector3.up
            Vector3 currentEuler = self.rigidbody.rotation.eulerAngles; //get the Euler angles
            currentEuler.x = 0.0f; //set the rotation around x axis to 0
            currentEuler.z = 0.0f; //set the rotation around z axis to 0

            //Now we only have an rotation around the y axis, so up with this rotation is Vector3.up
            self.rigidbody.MoveRotation(Quaternion.Euler(currentEuler));

            return true;
        }
        else
        {
            onGround = false;

            //enable physics rotation while falling
            self.rigidbody.freezeRotation = false;

            return false;
        }
    }

    protected override void FixedUpdate()
    {
        //check to see if the player is on the ground or in midair
        //I will likely changed this later it doesn't have to check
        //every update loop
        // Debug.Log(CheckGroundStatus());
        //Debug.Log($"Test is onGround {onGround}");

        if (CheckGroundStatus())
        {
            //On ground 

            //move the player 
            // self.rigidbody.MovePosition(self.rigidbody.position + (moveVector * Time.fixedDeltaTime));
            Vector3 horizontalVelocity = self.rigidbody.velocity;
            horizontalVelocity.y = 0.0f; //set vertical component to zero
            self.rigidbody.AddForce(moveVector - self.rigidbody.velocity, ForceMode.Force);

            //rotate the player
            Quaternion newRotation = self.rigidbody.rotation * Quaternion.Euler(0, turnValue * Time.fixedDeltaTime, 0);
            self.rigidbody.rotation = newRotation;

            //Regain stamina when on ground only
            stamina.Add(staminaRegainRate * Time.fixedDeltaTime);
        }
        else
        {
            //In Midair
        }

        DisplayedControlUi();

        //Regain stamina whenever in walking mode whether on ground or falling
        // stamina.Add(staminaRegainRate * Time.fixedDeltaTime);
    }

    private IEnumerator PerformDashJump()
    {
        Debug.Log("Dash Jump");
        //wait until dashing is completed
        yield return new WaitUntil(() => !dashing);

        Debug.Log($"Perform Dash Jump {dashJumpForce * Vector3.up}");
        //perform the dash jump
        self.rigidbody.AddForce(dashJumpForce * Vector3.up, ForceMode.Impulse);
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
            if (dashing)
            {
                //perform a dash jump
                StartCoroutine(PerformDashJump());
            }
            else
            {
                //If on ground then jump otherwise transition to Hover
                if (onGround)
                {
                    //On Ground, jump into the air
                    if (input.isPressed)
                    {
                        Vector3 jumpForceVector; //A vector that will represent the force the player
                                                 //is jumping with

                        //compute vertical component
                        if (moveVector == Vector3.zero)
                        {
                            jumpForceVector = Vector3.up * jumpForceStationary;
                        }
                        else
                        {
                            jumpForceVector = Vector3.up * jumpForceVertical;
                        }

                        //add the horizontal component to allow the player to 
                        //jump over a distance by doing a running jump
                        jumpForceVector += moveVector.normalized * jumpForceHonrizontal;

                        //jump with impulse
                        self.rigidbody.AddForce(jumpForceVector, ForceMode.Impulse);
                    } //end if (!input.isPressed)
                }//end if (onGround)
                else
                {
                    //In midair, transition into Hovering
                    //note that you can only hover if you have stamina
                    if (input.isPressed && stamina.ResourceAmount() > 0)
                    {
                        Transition(Modes.HOVERING);
                    }
                } //end else (if (onGround))
            } //end else (if (dashing))
        } //end if (inputReady)
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

    //please remember to delete me!!!!!!!
    bool something = true;
    private void DisplayedControlUi()
    {
        if (onGround != something)
        {
            something = onGround;
            Debug.Log($"Is on ground? {onGround}");
        }
        if (onGround)
        {
            controlHelperTexts[0].text = "Jump";
            controlHelperTexts[1].text = "Sprint";
            controlHelperTexts[2].text = "Dash";
        }
        else
        {
            controlHelperTexts[0].text = "Hover";
            controlHelperTexts[1].text = "Fly";
            controlHelperTexts[2].text = "Dash";
        }
    }
}
