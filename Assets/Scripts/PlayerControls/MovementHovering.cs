using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementHovering : MovementMode
{
    [SerializeField] private float hoverSpeed;
    [SerializeField] private float turnSpeed;

    private float turnValue;

    private void Awake()
    {
        base.Awake();

        turnValue = 0;
    }

    private void OnEnable()
    {
        Debug.Log("Now hovering");
        //make the movement kinematic
        self.rigidbody.isKinematic = true;

        //Update to hover speed
        speed = hoverSpeed;

        //reset the rotate transform.up is the same as Vector3.up
        Vector3 currentEuler = self.rigidbody.rotation.eulerAngles; //get the Euler angles
        currentEuler.x = 0.0f; //set the rotation around x axis to 0
        currentEuler.z = 0.0f; //set the rotation around z axis to 0
        //Now we only have an rotation around the y axis, so up with this rotation is Vector3.up
        self.rigidbody.rotation = Quaternion.Euler(currentEuler);

        //mark the mode as newly transition to
        justEnabled = true;
    }

    private void Start()
    {
        Debug.Log("Hovering start");
        justEnabled = false;
    }

    protected override void FixedUpdate()
    {
        //move the player 
        self.rigidbody.position += moveVector * Time.fixedDeltaTime;

        //rotate the player
        Quaternion newRotation = self.rigidbody.rotation * Quaternion.Euler(0, turnValue * Time.fixedDeltaTime, 0);
        self.rigidbody.rotation = newRotation;
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
        //if justEnabled is true and the space key is pressed
        //then the space key is still pressed from transitioning
        //to this state
        // if (justEnabled)
        // {
        //     //if the space key is no longer held down set
        //     //justEnabled to false so the script knows that
        //     //next time the player presses the space key
        //     //it is to transition out of this state
        //     if (!input.isPressed)
        //     {
        //         justEnabled = false;
        //     }
        // }
        // else
        // {
        //     if (input.isPressed)
        //     {
        //         Transition(Modes.FLYING);
        //     }
        // }

        if (input.isPressed)
        {
            Transition(Modes.FLYING);
        }
    }
}
