using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A script to represent a wind tunnel
//For now this script will be very simple
public class WindTunnel : MonoBehaviour
{
    [SerializeField] private Vector3 wind;

    public Vector3 getWindValue()
    {
        return wind;
    }
}
