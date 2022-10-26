using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//a simple script to prevent the camera from rotating
public class FreezeRotation : MonoBehaviour
{
    Quaternion frozenRotation; //a variable to save the rotation of the camera

    private void Awake()
    {
        //save the starting rotation of the camera
        frozenRotation = this.transform.rotation;
    }

    private void LateUpdate()
    {
        //at the end of each frame reset the camera's rotation
        this.transform.rotation = frozenRotation;
    }
}
