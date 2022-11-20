using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaManager : ResourceManager
{
    [SerializeField] Slider staminaUI;

    public override void Subtract(float amountToSubtract)
    {
        base.Subtract(amountToSubtract);

        //make sure you don't get negative stamina
        if (resource < 0)
        {
            resource = 0;
        }

        staminaUI.value = resource;
    }

    //temp function
    public override void Add(float amountToAdd)
    {
        base.Add(amountToAdd);

        staminaUI.value = resource;
    }
}
