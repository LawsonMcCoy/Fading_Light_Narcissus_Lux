using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementFlying : MovementMode
{
    [SerializeField] private float testForwardSpeed;
    [SerializeField] private float forwardThrustMagnitude;
    [SerializeField] private float liftPower; //A fine toning value for the magnitude of lift
    [SerializeField] private float coefficientOfInducedDrag; //a fine toning value for the magnitude of induce drag
    [SerializeField] private AnimationCurve coefficientOfLiftCurve; //an animation curve used to compute the coefficient 
                                                                    //of lift from the angle of attack

    private float angleOfAttack; //the angle between the velocity and the horizontal plane
    private Vector3 localVelocity;

    [SerializeField] private float tiltSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float maxTiltAngle; //expected positive
    [SerializeField] private float minTiltAngle; //expected negative
    [SerializeField] private float maxTurnAngle;

    //inputs
    private float tiltValue;
    private float turnValue;

    //testing variables
    private bool speedBoost; //a variable that will add forward speed when it is true
    [SerializeField] float speedBoostMagnitude;

    protected override void Awake()
    {
        base.Awake();

        //initialize variables
        tiltValue = 0f;

        speedBoost = false;
    }

    private void OnEnable()
    {
        Debug.Log("Now Flying");
        //set the rigidbody to nonkinematic, so it can be control with forces
        self.rigidbody.isKinematic = false;

        // Debug.Log("enabling movement mode: flying");
        //apply a forward force with this script is enabled
        Debug.Log(self);
        Debug.Log(self.rigidbody);
        //Give a starting push everytime the player start flying
        self.rigidbody.AddForce(transform.forward * testForwardSpeed, ForceMode.Impulse);
        // self.rigidbody.velocity = transform.forward * testForwardSpeed;

        //mark the mode as newly transition to
        justEnabled = true;
    }

    protected override void FixedUpdate()
    {
        //get local velocity
        localVelocity = Quaternion.Inverse(self.rigidbody.rotation) * self.rigidbody.velocity;

        //calculate angle of attack
        angleOfAttack = Mathf.Atan2(-localVelocity.y, localVelocity.z) * Mathf.Rad2Deg;

        //apply a thrust force
        // self.rigidbody.AddForce(transform.forward * forwardThrustMagnitude);

        //apply lift
        AddLift();

        //apply torque for tilt and turn
        AddTorque();

        //test function to add forward speed
        if (speedBoost)
        {
            self.rigidbody.AddForce(transform.forward * speedBoostMagnitude);
        }

        base.FixedUpdate();
    }

    private void AddLift()
    {
        //variables for calculating lift
        Vector3 horizontalVelocity;
        float coefficientOfLift;
        float liftMagnitude;
        Vector3 lift;
        float inducedDragMagnitude;
        Vector3 inducedDrag;
        
        //calculate lift

        //Calculate the magnitude of the horizontal velocity
        // Debug.Log($"velocity: {self.rigidbody.velocity}");
        horizontalVelocity = Vector3.ProjectOnPlane(self.rigidbody.velocity, this.transform.right);
        // Debug.Log($"horizontal velocity: {horizontalVelocity}");
        // Debug.Log($"squared velocity: {horizontalVelocity.sqrMagnitude}");

        //Calculate the coefficient of lift using animation curves
        coefficientOfLift = coefficientOfLiftCurve.Evaluate(angleOfAttack);
        if (Mathf.Abs(coefficientOfLift) < 0.01f)
        {
            coefficientOfLift = 0.0f;
        }
        // Debug.Log($"coefficient of lift: {coefficientOfLift}");

        //compute lift Magnitude 
        liftMagnitude = coefficientOfLift * liftPower * horizontalVelocity.sqrMagnitude;
        // Debug.Log($"lift magnitude: {liftMagnitude}");

        //apply lift in the perpendicular to air flow and right side
        lift = Vector3.Cross(horizontalVelocity.normalized, transform.right) * liftMagnitude;

        // Debug.Log($"adding lift {lift}");
        Vector3 horizontalLift = Vector3.ProjectOnPlane(lift, Vector3.up);
        float horizontalLiftBoost = 10.0f;
        horizontalLift = horizontalLift * horizontalLiftBoost;
        // Debug.Log($"horizonatal lift magnitude {horizontalLift.magnitude}");
        Debug.DrawLine(transform.position, transform.position + lift, Color.red);
        self.rigidbody.AddForce(lift);
        // Vector3 forceApplicationOffset = transform.up * 0.5f;
        // self.rigidbody.AddForceAtPosition(lift, transform.position + forceApplicationOffset);

        //calculate the induce drag
        inducedDragMagnitude = coefficientOfLift * coefficientOfLift * coefficientOfInducedDrag * horizontalVelocity.sqrMagnitude;
        inducedDrag = -horizontalVelocity.normalized * inducedDragMagnitude;
        
        // Debug.Log(inducedDragMagnitude);
        Debug.DrawLine(transform.position, transform.position + inducedDrag, Color.green);
        self.rigidbody.AddForce(inducedDrag);
    }

    private void AddTorque()
    {
        Vector3 tiltTorque;
        Vector3 turnTorque;

        // Debug.Log(angleOfAttack);
        // if ((angleOfAttack < minTiltAngle && tiltValue < 0.0f) || (angleOfAttack > maxTiltAngle && tiltValue > 0.0f))
        // {
        //     Debug.Log("STOP!!");
        //     //out of bounds tilt, stop tilting
        //     tiltTorque = Vector3.zero;
        // }
        // else
        // {
        //     //tilt the player
        //     tiltTorque = transform.right * tiltValue;
        // }
        tiltTorque = transform.right * tiltValue;

        //turn the palyer
        turnTorque = transform.forward * turnValue;

        //apply the torque
        self.rigidbody.AddTorque(tiltTorque + turnTorque);
    }

    private void rotatePlayer()
    {
        float tiltRotation; //about the x axis
        float turnRotation; //about the z axis
        Quaternion rotationMatrix;

        //get the tilt rotation angle
        tiltRotation = tiltValue * maxTiltAngle;

        //get the turn rotation angle
        turnRotation = turnValue * maxTurnAngle;

        //Construct the Quaternion
        //rotate tiltRotation degrees about x axis
        //no rotation about the y axis
        //rotate turnRotation degrees about the z axis
        rotationMatrix = Quaternion.Euler(tiltRotation, 0.0f, turnRotation); 

        //rotate the player
        self.rigidbody.MoveRotation(rotationMatrix);
    }

    //************
    //Player input
    //************

    //wasd input
    protected override void OnMove(InputValue input)
    {
        //x component is turning, y component is tilting
        Vector2 moveVector = input.Get<Vector2>();
        tiltValue = -moveVector.y;
        turnValue = -moveVector.x;
    }

    //space key input
    private void OnJumpTransition(InputValue input)
    {
        Debug.Log($"Flying space {input.isPressed}");
        //if justEnabled is true and the space key is pressed
        //then the space key is still pressed from transitioning
        //to this state
        if (justEnabled)
        {
            //if the space key is no longer held down set
            //justEnabled to false so the script knows that
            //next time the player presses the space key
            //it is to transition out of this state
            if (!input.isPressed)
            {
                justEnabled = false;
            }
        }
        else
        {
            if (input.isPressed)
            {
                Debug.Log((int)Modes.HOVERING);
                Transition(Modes.HOVERING);
            }
        }
    }

    //test function, shift input
    private void OnSpeedBoost(InputValue input)
    {
        speedBoost = input.isPressed;
    }

}
