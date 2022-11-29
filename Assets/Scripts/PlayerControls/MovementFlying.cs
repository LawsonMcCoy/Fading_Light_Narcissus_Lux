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
    [Tooltip("Limits how much the player can tilt torwards the sky, expected positive value from 0-90")]
    [SerializeField] private float maxTiltAngle; //expected value from 270-360
    [Tooltip("Limits how much the player can tilt torwards the ground, expected negative value from 0-90")]
    [SerializeField] private float minTiltAngle; //expected value from 0-90
    [SerializeField] private float maxTurnAngle;

    [SerializeField] private LayerMask collisionLayer;

    //Testing
    [SerializeField] private bool alternateTurning;
    [SerializeField] private bool alternateTilting;
    [SerializeField] private float alternateTiltingError;
    [SerializeField] private float alternateTiltDampingPower; //A fine toning constant to increase the damping power on the velocity
    [SerializeField] private float alternateTurningError;
    [SerializeField] private float alternateTurnDampingPower; //A fine toning constant to increase the damping power on the velocity

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
        //enable gravity
        self.rigidbody.useGravity = true;

        //enable rotation
        self.rigidbody.freezeRotation = false;

        // Debug.Log("enabling movement mode: flying");
        //apply a forward force with this script is enabled
        //Give a starting push everytime the player start flying
        AddForce(transform.forward * testForwardSpeed, ForceMode.Impulse);
        // self.rigidbody.velocity = transform.forward * testForwardSpeed;

        modeUIColor = new Color(0f, 0.8f, 1f, 1f);
        movementModeText.color = modeUIColor;
    }

    protected override void FixedUpdate()
    {
        //get local velocity
        localVelocity = Quaternion.Inverse(self.rigidbody.rotation) * self.rigidbody.velocity;

        //calculate angle of attack
        angleOfAttack = Mathf.Atan2(-localVelocity.y, localVelocity.z) * Mathf.Rad2Deg;

        //apply a thrust force
        // AddForce(transform.forward * forwardThrustMagnitude);

        //apply lift
        AddLift();

        //apply torque for tilt and turn
        AddTorque();

        //test function to add forward speed
        if (speedBoost)
        {
            // AddForce(transform.forward * speedBoostMagnitude);
            AddForce(transform.forward * speedBoostMagnitude, ForceMode.Force);

        }

        base.FixedUpdate();

        // Debug.Log($"tilt angle: {self.rigidbody.rotation.eulerAngles.x}");
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
        AddForce(lift, ForceMode.Force);
        // Vector3 forceApplicationOffset = transform.up * 0.5f;
        // AddForceAtPosition(lift, transform.position + forceApplicationOffset);

        //calculate the induce drag
        inducedDragMagnitude = coefficientOfLift * coefficientOfLift * coefficientOfInducedDrag * horizontalVelocity.sqrMagnitude;
        inducedDrag = -horizontalVelocity.normalized * inducedDragMagnitude;
        
        // Debug.Log(inducedDragMagnitude);
        Debug.DrawLine(transform.position, transform.position + inducedDrag, Color.green);
        AddForce(inducedDrag, ForceMode.Force);
    }

    private void AddTorque()
    {
        float tiltTorqueMagnitude; //the magnitude of the tiltTorque
        Vector3 tiltTorque;
        Vector3 turnTorque = Vector3.zero;

        //test the two ways of tilting
        if (alternateTilting)
        {
            //set the tilt value to the difference of current angle and max or min
        
            //get current angle
            // float currentTiltAngle = self.rigidbody.rotation.eulerAngles.x;
            Vector3 forwardInHorizontalPlane = Vector3.ProjectOnPlane(this.transform.forward, Vector3.up); //This projection is so we can find angle to horizontal plane
            float currentTiltAngle;
            // if (Vector3.Dot(this.transform.up, Vector3.up) > 0)
            // {
            //     currentTiltAngle = Vector3.SignedAngle(forwardInHorizontalPlane, this.transform.forward, this.transform.right);
            // }
            // else
            // {
            //     currentTiltAngle = Vector3.SignedAngle(-forwardInHorizontalPlane, this.transform.forward, this.transform.right);
            // }
            currentTiltAngle = Vector3.SignedAngle(forwardInHorizontalPlane, this.transform.forward, this.transform.right);
            // float currentTiltVelocity = Mathf.Sqrt(Mathf.Pow(self.rigidbody.angularVelocity.x, 2) + Mathf.Pow(self.rigidbody.angularVelocity.z, 2));
            float currentTiltVelocity = Vector3.Dot(self.rigidbody.angularVelocity, this.transform.right);
            Debug.Log($"current tilt angle: {currentTiltAngle}, angular velocity: {self.rigidbody.angularVelocity}, tilt velocity {currentTiltVelocity}");
        

            //Change angle from 360-270 and 0-90, to 0-180
            // currentTiltAngle = (currentTiltAngle + 90) % 360;

            //compute the titlValue
            if (tiltValue < 0)
            {
                //Tilt up (negative direction)
                // Debug.Log("Tilt up");
                
                // tiltValue =  currentTiltAngle - minTiltAngle;
                if (Mathf.Abs(minTiltAngle - currentTiltAngle) > alternateTiltingError)
                {
                    tiltTorqueMagnitude = (minTiltAngle - currentTiltAngle) / Time.fixedDeltaTime; //change in position/change in time = velocity
                    // tiltTorqueMagnitude = minTiltAngle - currentTiltAngle;
                }
                else
                {
                    tiltTorqueMagnitude = 0;
                    self.rigidbody.AddTorque(-currentTiltVelocity * alternateTiltDampingPower * this.transform.right);
                }
            }
            else if (tiltValue > 0)
            {
                //Tilt down (positive direction)
                // Debug.Log("Tilt down");

                if (Mathf.Abs(maxTiltAngle - currentTiltAngle) > alternateTiltingError)
                {
                    tiltTorqueMagnitude = maxTiltAngle - currentTiltAngle;
                }
                else
                {
                    tiltTorqueMagnitude = 0;
                    self.rigidbody.AddTorque(-currentTiltVelocity * alternateTiltDampingPower * this.transform.right);
                }
            }
            else
            {
                // Debug.Log("No tilt");
                //not tilting, reset to 0
                if (Mathf.Abs(currentTiltAngle) > alternateTiltingError)
                {
                    tiltTorqueMagnitude = -currentTiltAngle;
                    // Debug.Log($"Going to angle, distance: {Mathf.Abs(minTiltAngle - currentTiltAngle)}");
                }
                else
                {
                    tiltTorqueMagnitude = 0;
                    // Debug.Log($"At angle, stopping {self.rigidbody.angularVelocity.x}");
                    self.rigidbody.AddTorque(-currentTiltVelocity * alternateTiltDampingPower * this.transform.right);
                    // self.rigidbody.angularVelocity.x = 0;
                }

                //not tilting, don't apply a torque
                // tiltTorqueMagnitude = 0;
            }
        }
        else
        {
            //get current angle
            // float currentTiltAngle = self.rigidbody.rotation.eulerAngles.x;
            Vector3 forwardInHorizontalPlane = Vector3.ProjectOnPlane(this.transform.forward, Vector3.up); //This projection is so we can find angle to horizontal plane
            float currentTiltAngle;
            if (Vector3.Dot(this.transform.up, Vector3.up) > 0)
            {
                currentTiltAngle = Vector3.SignedAngle(forwardInHorizontalPlane, this.transform.forward, this.transform.right);
            }
            else
            {
                currentTiltAngle = Vector3.SignedAngle(-forwardInHorizontalPlane, this.transform.forward, this.transform.right);
            }
            // Debug.Log($"current angle {currentTiltAngle}");

            //get angularVelocity
            float currentTiltVelocity = Vector3.Dot(self.rigidbody.angularVelocity, this.transform.right);
            // Debug.Log($"current velocity {currentTiltVelocity}");

            //Change angle from 360-270 and 0-90, to 0-180
            // currentTiltAngle = (currentTiltAngle + 90) % 360;

            //check if your tilt is withing bounds
            // Debug.Log($"Current tilt angle: {minTiltAngle}");
            if (currentTiltAngle < minTiltAngle)
            {
                //tilted too high (facing sky)
                // Debug.Log($"Sky, angle: {currentTiltAngle}");
                if (tiltValue > 0)
                {
                    // Debug.Log("Allow");
                    tiltTorqueMagnitude = tiltValue * (tiltSpeed - currentTiltVelocity);
                }
                else
                {
                    // Debug.Log("Stopping");
                    // Debug.Log($"positive {currentTiltVelocity}");
                    tiltTorqueMagnitude = -currentTiltVelocity;
                }
            }
            else if (currentTiltAngle > maxTiltAngle)
            {
                //tilted too low (facing ground)
                // Debug.Log($"ground, angle: {currentTiltAngle}");
                if (tiltValue < 0)
                {
                    tiltTorqueMagnitude = tiltValue * (tiltSpeed - currentTiltVelocity);
                }
                else
                {
                    // Debug.Log($"negative {currentTiltVelocity}");
                    tiltTorqueMagnitude = -currentTiltVelocity;
                }
            }
            else
            {
                //in range
                // Debug.Log($"In range, angle: {currentTiltAngle}");
                tiltTorqueMagnitude = tiltValue * (tiltSpeed - Mathf.Abs(currentTiltVelocity));
            }
        }
        tiltTorque = transform.right * tiltTorqueMagnitude;

        //turn the palyer
        if (alternateTurning)
        {
            //set the roll to 90 degrees in appropriate direction
            Vector3 upInVerticalPlane = Vector3.ProjectOnPlane(this.transform.up, Vector3.Cross(this.transform.forward, Vector3.up)); //This projection is so we can find angle to horizontal plane
            float currentRollAngle;
            if (Vector3.Dot(this.transform.up, Vector3.up) > 0)
            {
                currentRollAngle = Vector3.SignedAngle(upInVerticalPlane, this.transform.up, this.transform.forward);
            }
            else
            {
                currentRollAngle = Vector3.SignedAngle(-upInVerticalPlane, this.transform.up, this.transform.forward);
            }
            Debug.Log($"current roll angle {currentRollAngle}, upInPlane {upInVerticalPlane}, actually up (local) {this.transform.up}");

            float changeInPosition = (90 * turnValue) - currentRollAngle;
            // float rollTorqueMagnitude = (changeInPosition) / (Time.fixedDeltaTime * Time.fixedDeltaTime); //change in position / (change in time)^2 = acceleration
            float rollTorqueMagnitude = changeInPosition;

            //compute the roll angular velocity
            float curretnRollVelocity = Vector3.Dot(self.rigidbody.angularVelocity, this.transform.forward);

            if (Mathf.Abs(changeInPosition) > alternateTurningError)
            {
                Debug.Log($"Not Damping {changeInPosition}");
                self.rigidbody.AddTorque(transform.forward * rollTorqueMagnitude, ForceMode.Impulse);
            }
            else
            {
                Debug.Log($"Damping {changeInPosition}");
                //damp the velocity
                // if (Mathf.Abs(currentRollAngle) > alternateTurningError)
                // {
                //     self.rigidbody.AddTorque(-self.rigidbody.angularVelocity.z * alternateTurnDampingPower * this.transform.forward);
                // }
                // else
                // {
                //     self.rigidbody.AddTorque(-self.rigidbody.angularVelocity.z * 5 * alternateTurnDampingPower * this.transform.forward);
                // }
                self.rigidbody.AddTorque(-curretnRollVelocity * alternateTurnDampingPower * this.transform.forward);
            }

            //add a tilt torque for turning
            float turnTiltTorqueMagnitude = -Mathf.Abs(turnSpeed * turnValue); //needs to be negative to tilt up for the turn

            Vector3 turnTiltTorque = transform.right * turnTiltTorqueMagnitude;

            tiltTorque += turnTiltTorque;
        }
        else
        {
            turnTorque = transform.forward * turnValue;
        }

        // Debug.Log($"tilt: {tiltTorque.magnitude}, turn: {turnTorque.magnitude}");
        //apply the torque
        if (alternateTilting)
        {
            self.rigidbody.AddTorque(tiltTorque, ForceMode.Impulse);
        }
        else
        {
            self.rigidbody.AddTorque(tiltTorque + turnTorque, ForceMode.Force);
        }
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

    //Exit flying on collision
    private void OnCollisionEnter(Collision collision)
    {
        LayerMask objectLayer; //the layer mask for the object collided with

        //get the object's layer mask, by right shifting 1 the object's layer
        //number of times
        objectLayer = 1 << collision.gameObject.layer;

        //if the game object is in the allowed collision layer
        //transition to walking
        if ((objectLayer & collisionLayer) != 0)
        {
            Transition(Modes.WALKING);
        }
    }

    //zeroing out rotational motion during movement restricted events
    public override void StartMovementRestrictedEvent()
    {
        //zero parent's motion
        base.StartMovementRestrictedEvent();

        //zero rotational motion
        tiltValue = 0;
        turnValue = 0;
    }

    //************
    //Player input
    //************

    //wasd input
    protected override void OnMove(InputValue input)
    {
        //x component is turning, y component is tilting
        Vector2 moveVector = input.Get<Vector2>();

        // //test the two ways of tilting
        // if (alternateTilting)
        // {
        //     //set the tilt value to the difference of current angle and max or min
            
        //     //get current angle
        //     float currentTiltAngle = self.rigidbody.rotation.eulerAngles.x;

        //     //Change angle from 360-270 and 0-90, to 0-180
        //     currentTiltAngle = (currentTiltAngle + 90) % 360;

        //     //compute the titlValue
        //     if (moveVector.y > 0)
        //     {
        //         //Tilt up (negative direction)
        //         Debug.Log("Tilt up");
                
        //         // tiltValue =  currentTiltAngle - minTiltAngle;
        //         tiltValue = minTiltAngle - currentTiltAngle;
        //     }
        //     else if (moveVector.y < 0)
        //     {
        //         //Tilt down (positive direction)
        //         Debug.Log("Tilt down");

        //         tiltValue = maxTiltAngle - currentTiltAngle;
        //     }
        //     else
        //     {
        //         Debug.Log("No tilt");
        //         //not tilting, reset to 0
        //         tiltValue = -currentTiltAngle;
        //     }
        // }
        // else
        // {
        //     //get current angle
        //     float currentTiltAngle = self.rigidbody.rotation.eulerAngles.x;

        //     //Change angle from 360-270 and 0-90, to 0-180
        //     currentTiltAngle = (currentTiltAngle + 90) % 360;

        //     //check if your tilt is withing bounds
        //     Debug.Log($"Current tilt angle: {currentTiltAngle}");
        //     if (currentTiltAngle < 0)
        //     {
        //         //tilted too high (facing sky)
        //         Debug.Log("Sky");
        //         tiltValue = 0;
        //     }
        //     else if (currentTiltAngle > 180)
        //     {
        //         //tilted too low (facing ground)
        //         Debug.Log("ground");
        //         tiltValue = 0;
        //     }
        //     else
        //     {
        //         //in range
        //         Debug.Log("In range");
        //         tiltValue = -moveVector.y;
        //     }
        // }

        tiltValue = -moveVector.y;
        turnValue = -moveVector.x;
    }

    //space key input
    private void OnJumpTransition(InputValue input)
    {
        if (input.isPressed)
        {
            // Debug.Log((int)Modes.HOVERING);
            Transition(Modes.WALKING);
        }
    }

    //right click input
    protected override void OnDash(InputValue input)
    {
        if (this.enabled)
        {
            Vector3 dashVector; //A vector that represents the path the player dash is taking

            //Get the dash direction from the from the turn value
            //we will dash in the direction the player is rolling
            //always dash to the side
            dashVector = (turnValue * (-transform.right)).normalized;
                
            //compute the dash magnitude and perform dash
            ComputeDashVector(dashVector);
        }
    }

    //test function, shift input
    private void OnSpeedBoost(InputValue input)
    {
        speedBoost = input.isPressed;
    }

}
