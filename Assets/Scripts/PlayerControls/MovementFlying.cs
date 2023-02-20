using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementFlying : MovementMode
{
    //movement force values
    [SerializeField] private float forwardThrustMagnitude;
    [Tooltip("A fine toning value for the magnitude of lift")]
    [SerializeField] private float liftPower; //A fine toning value for the magnitude of lift
    [Tooltip("a fine toning value for the magnitude of induce drag (This is a side effect of lift an always points parrallel to air flow over wings)")]
    [SerializeField] private float coefficientOfInducedDrag; //a fine toning value for the magnitude of induce drag
    [Tooltip("an animation curve used to compute the coefficient of lift from the angle of attack")]
    [SerializeField] private AnimationCurve coefficientOfLiftCurve; //an animation curve used to compute the coefficient 
                                                                    //of lift from the angle of attack

    //values used in calculation of the flight forces, some are properties to allow UI to read the values
    private float angleOfAttack; //the angle between the velocity and the horizontal plane
    private Vector3 localVelocity;

    //turning torques values
    [Tooltip("The speed that Ika changes his pitch at when W or S is pressed")]
    [SerializeField] private float tiltSpeed;
    [Tooltip("A value that affect how sharpely Ika is able to turn")]
    [SerializeField] private float turnSpeed;
    [Tooltip("Limits how much the player can tilt torwards the sky, expected positive value from 0-90")]
    [SerializeField] private float maxTiltAngle; //expected value from 270-360
    [Tooltip("Limits how much the player can tilt torwards the ground, expected negative value from 0-90")]
    [SerializeField] private float minTiltAngle; //expected value from 0-90
    [Tooltip("The Angle to which the player will roll to when turning during flight")]
    [SerializeField] private float maxTurnAngle = 90;

    //other values
    [Tooltip("A layermask for that include all layer that causes Ika to fall out of flight when collided with")]
    [SerializeField] private LayerMask collisionLayer;
    [Tooltip("The particle system used for to visual the relative wind")]
    [SerializeField] private ParticleSystem windParticles;



    //Testing
    [SerializeField] private bool windImpactsLift;
    [SerializeField] private bool alternateTurning;
    [SerializeField] private float alternateTiltDampingPower; //A fine toning constant to increase the damping power on the velocity
    [SerializeField] private float alternateTurnDampingPower; //A fine toning constant to increase the damping power on the velocity
    [SerializeField] private float alternateTurnSpringConstant; //A fine toning constant that acts as the spring constant for the
                                                                //virtual spring holding the roll angle at equilibrium

    //inputs
    private float tiltValue;
    private float turnValue;

    //testing variables
    private bool speedBoost; //a variable that will add forward speed when it is true
    [SerializeField] float speedBoostMagnitude;
    [SerializeField] float speedBoostStaminaCost;

    protected override void Awake()
    {
        base.Awake();

        //initialize variables
        tiltValue = 0f;

        speedBoost = false;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
        //enable gravity
        // self.rigidbody.useGravity = true;

        //enable rotation
        self.rigidbody.freezeRotation = false;

        modeUIColor = new Color(0f, 0.8f, 1f, 1f);
        movementModeText.color = modeUIColor;

        //I feel that this could be done better using either
        //the event system or MovementUpdateReciever interface
        controlUi.TransitionGlideUI();
        controlUi.IndicateModeChange();

        //Play the wind particles, so the player sees wind when in flight
        windParticles.Play();
    }

    private void OnDisable()
    {
        //stop the wind particles, so the player doesn't see wind in other movement modes
        windParticles.Stop();
    }

    protected override void FixedUpdate()
    {
        //make sure that MovementMode fixed update is called first
        base.FixedUpdate();

        //get local velocity
        localVelocity = Quaternion.Inverse(self.rigidbody.rotation) * self.rigidbody.velocity;

        //calculate angle of attack
        angleOfAttack = Mathf.Atan2(-localVelocity.y, localVelocity.z) * Mathf.Rad2Deg;

        // //get local velocity
        // localVelocity = Quaternion.Inverse(self.rigidbody.rotation) * (-relativeWind);

        // //calculate angle of attack
        // angleOfAttack = Mathf.Atan2(-localVelocity.y, localVelocity.z) * Mathf.Rad2Deg;

        //compute the angleOfAttack
        // Vector3 windInYZPlane = Vector3.ProjectOnPlane(relativeWind, transform.right); //start by finding the wind in the players yz plane
        // angleOfAttack = Vector3.SignedAngle(-windInYZPlane, transform.forward, transform.right); //find the angle between player's yz plane wind and forwards
        // Debug.Log($"angle of attack {angleOfAttack}");

        //apply a thrust force
        // AddForce(transform.forward * forwardThrustMagnitude);

        //apply lift
        AddLift();

        //apply torque for tilt and turn
        AddTorque();

        //test function to add forward speed
        if (speedBoost && stamina.ResourceAmount() > 0)
        {
            AddForce(transform.forward * speedBoostMagnitude, ForceMode.Force);

            //This may no longer be a test feature only, making it cost stamina is an option we could give players
            stamina.Subtract(speedBoostStaminaCost * Time.fixedDeltaTime);
        }

        //update the wind particle system
        UpdateWindVisulation();

        Debug.DrawLine(transform.position, transform.position + self.rigidbody.velocity, Color.green);
    }

    private void AddLift()
    {
        //variables for calculating lift
        Vector3 forwardWind;  //The component of relative wind that flows over Ika's wings in the correct direction
        float coefficientOfLift;
        float liftMagnitude;
        Vector3 lift;
        Vector3 localDragScaleValues; //drage scale values after rotating them to local space
        Vector3 drag;
        float inducedDragMagnitude;
        Vector3 inducedDrag;

        //calculate the relative wind, for now just the opposite of velocity
        //later add in the absolute wind vector
        // relativeWind = -self.rigidbody.velocity;
        
        //calculate lift

        //Calculate the magnitude of the horizontal velocity
        if (windImpactsLift)
        {
            forwardWind = Vector3.Project(relativeWind, this.transform.forward);
        }
        else 
        {
            forwardWind = Vector3.Project(-self.rigidbody.velocity, this.transform.forward);
        }

        //Calculate the coefficient of lift using animation curves
        coefficientOfLift = coefficientOfLiftCurve.Evaluate(angleOfAttack);
        if (Mathf.Abs(coefficientOfLift) < 0.01f)
        {
            coefficientOfLift = 0.0f;
        }

        //compute lift Magnitude 
        liftMagnitude = coefficientOfLift * liftPower * forwardWind.sqrMagnitude;
        // Debug.Log($"lift magnitude: {liftMagnitude}, coefficient of lift {coefficientOfLift}, angle of attack {angleOfAttack}");

        //apply lift in the perpendicular to air flow and right side
        lift = Vector3.Cross(transform.right, forwardWind.normalized) * liftMagnitude;

        // Debug.Log($"lift force {lift}, transform.up {transform.up}, cross product {Vector3.Cross(transform.right, forwardWind.normalized)}");
        AddForce(lift, ForceMode.Force);

        //calculate the induce drag
        inducedDragMagnitude = coefficientOfLift * coefficientOfLift * coefficientOfInducedDrag * forwardWind.sqrMagnitude;
        inducedDrag = forwardWind.normalized * inducedDragMagnitude;
        
        AddForce(inducedDrag, ForceMode.Force);
    }

    private void AddTorque()
    {
        Vector3 totalTorque; //The net torque to apply, sum of all indvivdual torques

        //tilting the player
        float tiltTorqueMagnitude;

        //turning the player
        float rollTorqueMagnitude;
        Vector3 rollTorque; //The roll torque from turning
        float turnTiltTorqueMagnitude; 
        Vector3 turnTiltTorque; //The tilt torque from turning

        /**************
        *
        *Tilt the Player
        *
        **************/

        /****************************************************************************
        The first step to computing the torque needed for tilting the player is to
        measure the player's current pitch angle and pitch angular velocity. To measure
        the player's pitch angle we measure the angle between the player's forward vector and
        a plane that the forward vector would be in if the player had a pitch angle of 0. This
        plane will have be orientated horizontally (contain the vector Vector3.forward) and 
        slice through the player along the x-axis (contain the vector this.transform.right).
        We can then compute the normal of the plane with a cross product. Knowing this we
        can project the player's forward vector into the plane to produce a vector with the same
        angle with this.transform.forward as the plane would have. This angle can then be computed
        using Vector3.SignedAngle. (Note using euler angles do not work as they do not have 
        a unique representation. The player cannot alter their yaw, so we want the roll with
        0 yaw, but the eular angle that Unity gives us may not be this.) Measuring the player's 
        pitch angular velocity is much easier as rigidbody keep tracks of the player's
        total angular velocity. We can then dot the total angular velocity with the player's
        right vector to get their pitch angular velocity.
        *****************************************************************************/

        //compute the zero pitch plane normal 
        Vector3 zeroPitchPlaneNormal = Vector3.Cross(this.transform.right, Vector3.forward);

        //project forward vector in the horizontal plane
        Vector3 forwardInHorizontalPlane = Vector3.ProjectOnPlane(this.transform.forward, Vector3.up); //This projection is so we can find angle to horizontal plane
       
        //Measure the pitch angle, note that if the angle is greater than 90 then the projected vector
        //will flipped 180 degrees. In that case we need to flip it back by multiplying by -1. We can 
        //check if this is the case by checking if the dot product between both world and local ups is 
        //negative
        float currentPitchAngle = Vector3.SignedAngle(forwardInHorizontalPlane, this.transform.forward, this.transform.right);

        //measure the pitch angularVelocity
        float currentPitchVelocity = Vector3.Dot(self.rigidbody.angularVelocity, this.transform.right);

        /***********************************************************
        The next step is compute the magnitude of the tilt torque. Note that there are 
        three different cases the player can be in here. First is that
        the player is above the upper limit. In this case the player cannot
        continue to tilt up, but may still tilt down to go back in bounds.
        The second case is that the player is below the lower limit. Here the
        player cannot continue to tilt down, but may tilt up to go back in bounds.
        Lastly the player can be in bounds in which case they can tilt up or down.
        To tilt the play we will set the ptich angular velocity to the apporiate
        value. This is done by setting the magnitude to the difference of the 
        target velocity and the actually velocity that was measured. The we the 
        player is and allow to tilt then their angular velocity is set to the 
        tiltSpeed that is set in the inspector. Otherwise the angular velocity is
        set to 0.
        ************************************************************/
        //check if your tilt is withing bounds
        if (currentPitchAngle < minTiltAngle)
        {
            //tilted too high (facing sky)
            if (tiltValue > 0)
            {
                tiltTorqueMagnitude = tiltValue * (tiltSpeed - currentPitchVelocity);
            }
            else
            {
                tiltTorqueMagnitude = -currentPitchVelocity;
            }
        }
        else if (currentPitchAngle > maxTiltAngle)
        {
            //tilted too low (facing ground)
            if (tiltValue < 0)
            {
                tiltTorqueMagnitude = tiltValue * (tiltSpeed - currentPitchVelocity);
            }
            else
            {
                tiltTorqueMagnitude = -currentPitchVelocity;
            }
        }
        else
        {
            //in range
            tiltTorqueMagnitude = tiltValue * (tiltSpeed - Mathf.Abs(currentPitchVelocity));
        }

        /********************************************************************************
        Once we computed the magnitude of the tiltTorque, and can compute the tilt torque
        and set it equal the total torque (since this is the first torque we computed)
        *********************************************************************************/
        totalTorque = transform.right * tiltTorqueMagnitude;

        /**************
        *
        *Turn the Player
        *
        **************/
        if (alternateTurning)
        {
            /****************************************************************************
            The first step to computing the torque needed for turning the player is to
            measure the player's current roll angle and roll angular velocity. To measure
            the player's roll angle we measure the angle between the player's up vector and
            a plane that the up vector would be in if the player had a roll angle of 0. This
            plane will have be orientated vertically (contain the vector Vector3.up) and 
            slice through the player along the z-axis (contain the vector this.transform.forward).
            We can then compute the normal of the plane with a cross product. Knowing this we
            can project the player's up vector into the plane to produce a vector with the same
            angle with this.transform.up as the plane would have. This angle can then be computed
            using Vector3.SignedAngle. (Note using euler angles do not work as they do not have 
            a unique representation. The player cannot alter their yaw, so we want the roll with
            0 yaw, but the eular angle that Unity gives us may not be this.) Measuring the player's 
            roll angular velocity is much easier as rigidbody keep tracks of the player's
            total angular velocity. We can then dot the total angular velocity with the player's
            forward vector to get their roll angular velocity.
            *****************************************************************************/

            //compute zero roll plane normal
            Vector3 zeroRollPlaneNormal = Vector3.Cross(this.transform.forward, Vector3.up);

            //project up into zero roll plane
            Vector3 upInVerticalPlane = Vector3.ProjectOnPlane(this.transform.up, zeroRollPlaneNormal); 

            //Measure the roll angle, note that if the angle is greater than 90 then the projected vector
            //will flipped 180 degrees. In that case we need to flip it back by multiplying by -1. We can 
            //check if this is the case by checking if the dot product between both world and local ups is 
            //negative
            float currentRollAngle;
            if (Vector3.Dot(this.transform.up, Vector3.up) > 0)
            {
                currentRollAngle = Vector3.SignedAngle(upInVerticalPlane, this.transform.up, this.transform.forward);
            }
            else
            {
                currentRollAngle = Vector3.SignedAngle(-upInVerticalPlane, this.transform.up, this.transform.forward);
            }

            //measure the roll angular velocity
            float currentRollVelocity = Vector3.Dot(self.rigidbody.angularVelocity, this.transform.forward);

            /****************************************************************************
            With both the roll angle and angular velocity, the next step is to compute the 
            torque to apply. Turing requires a combination of both roll and pitch torque, so
            this computation will be done over two steps. Let us first compute the roll torque.
            For that we want to set the roll to a specfic value determined by using input.
            We can imagine creating virtual spring that pulls the roll angle to some equilibrium.
            If we set this equilibrium to be the desire roll angle then the spring will pull the angle
            to our desire angle. If we also add damping to the spring then with the right spring constant
            and damping coefficient (fine toning constants set in the inspector), the virtual will
            pull and hold the roll angle at the desire value. We will then compute the roll torque
            using the apporiate spring and damping torques.
            *****************************************************************************/

            //We want to hold the roll angle at some equilibrium determined by player input
            //To do that we will use a virtual spring with the correct equilibrium point for
            //this frame. This equilibrium point is between -90 and 90 and is computed using
            //(maxTurnAngle * turnValue), where turnValue is a value from -1 to 1 set by user input
            float distanceFromEquilibrium = (maxTurnAngle * turnValue) - currentRollAngle; 

            //Use a harmonic osciallator torque to pull and hold the angle at an equilibrium (either -90, 0, or 90 base on player input) 
            rollTorqueMagnitude = (alternateTurnSpringConstant * distanceFromEquilibrium) - (alternateTurnDampingPower * currentRollVelocity);
            rollTorque = rollTorqueMagnitude * transform.forward;

            /********************************************************************************
            The next step is to compute the pitch or tilt torque for turning. With a roll angle of
            90 (or -90) degrees, the player's up vector is pointing in the direction we want to turn.
            Since a negative pitch/tilt angle rotates the player towards their up vector we will apply
            a negative torque. We will set the magnitude of this torque to a fine toning constant that
            determine the speed the player will turn at.
            *********************************************************************************/

            //add a tilt torque for turning
            turnTiltTorqueMagnitude = turnSpeed * turnValue; //needs to be negative to tilt up for the turn
            // Debug.Log($"turn tilt magnitude {turnTiltTorqueMagnitude}, speed {turnSpeed}, turn value {turnValue}");

            turnTiltTorque = Vector3.down * turnTiltTorqueMagnitude;

            /***********************************************************************************
            With both components of the turn torque computed, we can compute the full turn torque
            by computing their sum. We will also add this to the pitch torque from tilting the player
            to get the total torque being applied this frame.
            ***********************************************************************************/
            
            //Add the full turn torque to the total torque
            totalTorque += turnTiltTorque + rollTorque;
        }
        else
        {
            //When the player have full control of their roll angle then
            //just apply the torque from use input. There is no set values
            //or constraints
            totalTorque += transform.forward * turnValue;
        }

        //apply the torque
        self.rigidbody.AddTorque(totalTorque);
    }

    //A function to update the appear of the particle system
    //that create the visulation of relative wind for the player
    private void UpdateWindVisulation()
    {
        // Debug.Log($"Wind visulation value {relativeWind}");
        //update direction, wind particles should move in the direction of relative wind
        ParticleSystem.ShapeModule windShape = windParticles.shape;
        Quaternion windDirection = Quaternion.Inverse(transform.rotation) * Quaternion.LookRotation(relativeWind); //create a rotation to point in the wind direction 
                                                                                                                   //then convert it to local coordinates
        windShape.rotation = windDirection.eulerAngles;

        //update speed
    }

    //A visitor function to determine which type of movement mode this script is
    public override void GetMovementUpdate(MovementUpdateReciever updateReciever)
    {
        updateReciever.FlyingUpdate(this);
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

        tiltValue = moveVector.y;
        turnValue = -moveVector.x;
    }

    //space key input
    private void OnJumpTransition(InputValue input)
    {
        if (input.isPressed)
        {
            Transition(Modes.WALKING);
            StartCoroutine(DisableControlForTime(commonData.transitionMovementLockTime));
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
            dashVector = (turnValue * (Vector3.Cross(transform.forward, Vector3.up))).normalized;
                
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
