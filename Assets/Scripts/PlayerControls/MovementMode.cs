using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//I will be breaking up the three movement modes up into different 
//scripts. This is to avoid a bunch of switch statements, and 
//to have a cleaner code base. This class will serve as the parent
//class for each movement mode, so any common behavior can be 
//implemented here
public abstract class MovementMode : MonoBehaviour
{
    protected Player self; //a reference to yourself

    protected virtual void Awake()
    {
        //get the reference to yourself
        self = this.GetComponent<Player>();
    }
}
