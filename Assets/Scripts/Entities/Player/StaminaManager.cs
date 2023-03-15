using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaManager : ResourceManager
{
    [SerializeField] private Slider staminaUI;
    [SerializeField] private GameObject staminaUIBackground;
    private float lerpValue = 0f;
    private float lerpDuration = 0.2f;

    public override void Subtract(float amountToSubtract)
    {
        base.Subtract(amountToSubtract);

        //make sure you don't get negative stamina
        if (resource < 0)
        {
            resource = 0;
        }


        if (lerpValue < 1f)
        {
            lerpValue += Time.deltaTime / lerpDuration;
            staminaUIBackground.GetComponent<Image>().color = Color.Lerp(staminaUI.GetComponentInChildren<Image>().color, new Color32(255, 165, 0, 255), lerpValue);
        }
        

        staminaUI.value = resource;
        lerpValue = 0f;
    }

    //temp function
    public override void Add(float amountToAdd)
    {
        base.Add(amountToAdd);

        if (lerpValue < 1f)
        {
            lerpValue += Time.deltaTime / lerpDuration;
            staminaUIBackground.GetComponent<Image>().color = Color.Lerp(staminaUI.GetComponentInChildren<Image>().color, Color.white, lerpValue);
        }

        staminaUI.value = resource;
        lerpValue = 0f;
    }
}
