using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    [SerializeField] private Player player; //A reference to the player
    [SerializeField] private float lookAheadDistance; //How far in front of the player 
                                                      //does the camera look 
    [SerializeField] private float lookUpDistance;

    //fixed so that the camera update is the same update loop as the player
    //late so that the camera update after the player has updated
    private void LateUpdate()
    {
        // Debug.Log("updating camera");
        Vector3 lookPointPosition; //The position of the point the camera is looking at
        Vector3 lookDirection; //The direction the camera is looking, not necessarily normalized
        Vector3 eulerAngles = Vector3.zero; //The euler angle for the rotation of the camera this frame

        //compute the look point position 
        lookPointPosition = player.transform.position + (lookAheadDistance * player.transform.forward);

        // Debug.Log(player.rigidbody.rotation.eulerAngles);
        float currentRoll = player.rigidbody.rotation.eulerAngles.z;
        float angularDistanceFromSideways;
        if (currentRoll < 180)
        {
            angularDistanceFromSideways = Mathf.Abs(currentRoll - 90);
        } 
        else
        {
            angularDistanceFromSideways = Mathf.Abs(currentRoll - 270);
        }

        float percentageSideways = angularDistanceFromSideways / 90;
        lookPointPosition = lookPointPosition + (lookUpDistance * percentageSideways * player.transform.up);

        //compute the look direciton using the look position, note may not be normalized
        lookDirection = lookPointPosition - this.transform.position;

        //****************************************************
        //Rotate the camera to face lookDirection with no roll
        //****************************************************

        //compute the x euler angle
        eulerAngles.x = -Mathf.Atan2(lookDirection.y, Mathf.Sqrt(Mathf.Pow(lookDirection.x, 2) + Mathf.Pow(lookDirection.z, 2)));

        //compute the y euler angle
        eulerAngles.y = Mathf.Atan2(lookDirection.x, lookDirection.z);

        //convert eulerAngles to degrees
        eulerAngles = Mathf.Rad2Deg * eulerAngles;

        //construct an Quanterion from the euler angles,
        //and set the camera rotation to it
        // Debug.Log($"Rotating {eulerAngles}");
        this.transform.rotation = Quaternion.Euler(eulerAngles);
    }

}
