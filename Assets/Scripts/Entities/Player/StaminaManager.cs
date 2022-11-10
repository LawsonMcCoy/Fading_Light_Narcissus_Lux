using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaManager : ResourceManager
{
    public override void Subtract(float amountToSubtract)
    {
        base.Subtract(amountToSubtract);

        //make sure you don't get negative stamina
        if (resource < 0)
        {
            resource = 0;
        }

        // Debug.Log($"Stamina left: {resource}");
    }

    //temp function
    public override void Add(float amountToAdd)
    {
        base.Add(amountToAdd);

        // Debug.Log($"Stamina left: {resource}");
    }
}
