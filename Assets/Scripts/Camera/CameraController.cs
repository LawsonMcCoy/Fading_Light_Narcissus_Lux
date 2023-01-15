using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour, MovementUpdateReciever
{
    [SerializeField] Player player; //The game object for the player's character
    [Tooltip("The camera's spherical position around the player. x component is the distance the camera wants to be from the player. " +
              "The y component is the angle theta rotated around the Vector3 up axis NOT player's (-180 to 180). The z component is the angle phi " +
              "rotated from Vector3 up around the player's right axis (0 to 180). Note that an angle of theta=0 always places the camera directly behind the " +
              "camera, and an angle of phi=0 always places the camera directly above the player. This vector only affects camera position, not it's " +
              "orientation.")]
    [SerializeField] Vector3 sphericalPosition = new Vector3(10, 0, 0); //will change default value later
    private Vector3 lookDirection = Vector3.forward; //A unit vector representing where the camera is looking

    private void LateUpdate()
    {
        //Use the visitor pattern to get an update of player input from the active movement script
        player.activeMovementMode.GetMovementUpdate(this);
    }

    /*******************************************
    Camera behavior functions
    ********************************************/

    //A function to update the camera spherical position in order
    //to follow the player duing walking or hovering
    private void WalkHoverFollow(MovementMode movement)
    {
        //place camera behind the player, theta = 0
        sphericalPosition.y = 0;

        //use vertical mouse input to adjust phi
        sphericalPosition.z = AdjustPhi(sphericalPosition.z, movement.mouseInput.y);
    }



    /*****************************************
    Camera movement and rotation functions
    ******************************************/

    //The actual function that will move the camera to its current spherical position
    private void MoveCamera()
    {
        /*
        The first step to finding the camera position for current spherical position around the player
        is to compute a unit vector point from the player to the correct camera position. To do this we
        will start with Vector3.up. We want any rotation of the player that modified the player's up vector
        to change the position of the camera. Doing so causes disorientation and it is clear from playtesting
        that making these rotations of the player is displeasing to the players. Therefore we will use Vector3.up
        here instead. Next we will rotate this unit vector -phi degrees around the player right axis. Unlike rotations
        that modify the player's up vector, rotations around the player's up vector is something we want to mimic. 
        Doing so will make it easy for us to position the camera directly behind the player. The use of the player's
        right axis instead of Vector3.right here ensure that a theta value of 0 will always place the camera behind the
        player. As for the negative sign on phi, that is because normal rotations are clockwise in Unity. This means 
        that this rotation will move the camera in front of the player with theta=0. To recover the ability to have the 
        camera behind the player by setting theta to 0 we are introducing a negative sign on phi. Next will apply a second
        rotation to the unit vector, this time negative theta degrees around Vector3.up. After both rotations we will
        have successfully obtained a unit vector pointing from player to the desire camera location. 
        */

        //start with Vector3.up
        Vector3 cameraDirection = Vector3.up;

        //create the first rotation, -phi degrees around the player's right axis projected into the xz plane
        Vector3 projectedRight = Vector3.ProjectOnPlane(player.transform.right, Vector3.up);
        Quaternion cameraDirectionRotation = Quaternion.AngleAxis(-sphericalPosition.z, projectedRight);

        //create the second rotation, theta degrees around Vector3.up, combine it with first rotations to create a single complete rotation
        cameraDirectionRotation = Quaternion.AngleAxis(sphericalPosition.y, Vector3.up) * cameraDirectionRotation;

        //apply total rotation to the unit vector
        cameraDirection = cameraDirectionRotation * cameraDirection;

        /*
        With the camera direction vector all that is left to do is to scale it by the distance the camera should be from the player and 
        the resultant vector to the player's position. However there is one little complication. The x component of the spherical position
        is the desire distance between the camera and the player, but this distance may or may not place the camera behind a wall. To check 
        for this we will use the player's position and the camera direction to perform a raycast. If the path is clear then we are good to
        put the camera at the distance we want. However, if the raycast does hit a wall then we have to place the camera at the surface of 
        the wall instead.
        */

        //Declare RaycastHit object
        RaycastHit playerToCameraInfo;
        
        //Perform the raycast
        if (Physics.Raycast(player.transform.position, cameraDirection, out playerToCameraInfo, sphericalPosition.x))
        {
            //There is an object blocking the view of the camera, place the camera at the collision point, so its
            //view is not blocked
            transform.position = playerToCameraInfo.point;
        }
        else
        {
            //nothing blocking the camera's view, we are good to place it at the desired distance
            transform.position = player.transform.position + (sphericalPosition.x * cameraDirection);
        }
    }

    //This is a simple function that rotates the camera so it looks in the direction of its look vector
    private void RotateCamera()
    {
        //make sure lookDirection is nonzero
        if (lookDirection == Vector3.zero)
        {
            Debug.LogError("The Camera's looking direction vector is zero");
        }

        //make the camera look in the correct direction, note the camera should not be upside down so we 
        //are good to use Vector3.up as the camera's reference for up
        transform.rotation = Quaternion.LookRotation(lookDirection);
    }

    /*****************************************
    Computation Functions
    ******************************************/
    /*A function to adjust an angle from within its domain using input

    Parameters:
        currentValue - the currentValue of the angle
        adjustmentValue - the input used to adjust the angle, it will be scaled by fixedDeltaTime
        maxAngle - The max value of the angle's domain
        minAngle - the min value of the angle's domain
        loopValues - If true the new value will be looped into the domain, if false it will be clamped into the domain, defaults to false

    Returns the new value of the angle after being adjusted by adjustmentValue and the domain of the angle
    */
    private float AdjustAngleWithinRange(float currentValue, float adjustmentValue, float minAngle, float maxAngle, bool loopValues=false)
    {
        float newValue = currentValue + (adjustmentValue * Time.deltaTime);
        Debug.Log($"currentValue {currentValue}, adjustmentValue {adjustmentValue}, newValue {newValue}, min {minAngle}, max {maxAngle}");

        if (newValue < minAngle)
        {
            //new value is too small
            if (loopValues)
            {
                Debug.Log("too small loop");
                //need to loop the value into the range

                //start by subtracting newValue and maxAngle by minAngle, so the min of the range is zero
                float readjustedValue = newValue - minAngle; //note will be negative
                float readjustedMax = maxAngle - minAngle;

                //find the quotient, make it positive by multiplying by -1, and take the ceiling, 
                //adding this value * readjustedMax gives you the final looped value
                float numberOfLoops = Mathf.Ceil((-readjustedValue) / readjustedMax);

                return newValue + (numberOfLoops * readjustedMax) + minAngle;
            }
            else
            {
                Debug.Log("too small no loop");
                //no need to loop, just return the minAngle
                return minAngle;
            }
        }
        else if (newValue > maxAngle)
        {
            //new value is too big
            if (loopValues)
            {
                Debug.Log("too big loop");
                //need to loop the value into the range

                //start by subtracting newValue and maxAngle by minAngle, so the min of the range is zero
                float readjustedValue = newValue - minAngle; //note will be positive
                float readjustedMax = maxAngle - minAngle;

                //since all values are positive we can just use Unity's Mathf.Repeat function
                return Mathf.Repeat(readjustedValue, readjustedMax) + minAngle;
            }
            else
            {
                Debug.Log("too big no loop");
                //no need to loop, just return the maxAngle
                return maxAngle;
            }
        }
        else
        {
            Debug.Log("In Range");
            //newValue is in range, simply return it
            return newValue;
        }
    }

    //A short hand function to adjust the value of theta within its entire range of -180 to 180
    //This function will loop the value of theta
    private float AdjustTheta(float currentValue, float adjustmentValue)
    {
        return AdjustAngleWithinRange(currentValue, adjustmentValue, -180, 180, true);
    }

    //A short hand function to adjust the value of phi within its entire range of 0 to 180
    //This function will NOT loop the value of phi
    private float AdjustPhi(float currentValue, float adjustmentValue)
    {
        return AdjustAngleWithinRange(currentValue, adjustmentValue, 0, 180, false);
    }

    /****************************************
    Interface functions
    *****************************************/

    public void FlyingUpdate(MovementFlying movement)
    {
        //Compute the camera's new angles

        //move the camera to the correct position for this frame
        MoveCamera();

        //Compute the camera's new look direction unit vector

        //for now just have the camera look at the player, may change later
        lookDirection = Vector3.Normalize(player.transform.position - transform.position);

        //rotate the camera to its look direction
        RotateCamera();
    }

    public void HoverUpdate(MovementHovering movement)
    {
        //Compute the camera's new angles
        WalkHoverFollow(movement);

        //move the camera to the correct position for this frame
        MoveCamera();

        //Compute the camera's new look direction unit vector

        //for now just have the camera look at the player, may change later
        lookDirection = Vector3.Normalize(player.transform.position - transform.position);

        //rotate the camera to its look direction
        RotateCamera();
    }

    public void WalkingUpdate(MovementWalking movement)
    {
        //Compute the camera's new angles
        
        if (movement.onGround)
        {
            //if the player is walking then do a walk/hover follow
            WalkHoverFollow(movement);
        }

        //move the camera to the correct position for this frame
        MoveCamera();

        //Compute the camera's new look direction unit vector

        //for now just have the camera look at the player, may change later
        lookDirection = Vector3.Normalize(player.transform.position - transform.position);

        //rotate the camera to its look direction
        RotateCamera();
    }
}
