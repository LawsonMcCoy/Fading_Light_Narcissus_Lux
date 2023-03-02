using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [SerializeField] protected float resource; //The value of the resource being managed
    [SerializeField] protected float maxResource; //The max value of the resource

    protected virtual void Awake()
    {
        //initialize resource to its max value
        resource = maxResource;
    }

    //A function to add to the resource without going over the max
    public virtual void Add(float amountToAdd)
    {
        resource += amountToAdd;
        if(resource > maxResource)
        {
            resource = maxResource;
        }
    }

    //A virtual function to subtract from the resource. It is expected that 
    //subclasses will override this to add addition function such as killing 
    //entity for health manager
    public virtual void Subtract(float amountToSubtract)
    {
        resource -= amountToSubtract;
    }

    //A function to quiry for the value of the resource
    public float ResourceAmount()
    {
        return resource;
    }
}
