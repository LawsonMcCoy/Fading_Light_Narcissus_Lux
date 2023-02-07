using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A script to represent a wind tunnel
//For now this script will be very simple
public class WindTunnel : MonoBehaviour
{
    [SerializeField] private Vector3 wind;
    [SerializeField] private ParticleSystem windParticles;

    private void Awake()
    {
        Debug.Log("Test");
        //make the wind particles blow in the correct directions
        ParticleSystem.ShapeModule windShape = windParticles.shape;
        Quaternion windDirection = Quaternion.LookRotation(wind);//this.transform.rotation * wind); //create a rotation to point in the wind direction 
                                                                                                                   //then convert it to local coordinates
        windShape.rotation = windDirection.eulerAngles;
    }

    public Vector3 getWindValue()
    {
        return this.transform.rotation * wind;
    }
}
