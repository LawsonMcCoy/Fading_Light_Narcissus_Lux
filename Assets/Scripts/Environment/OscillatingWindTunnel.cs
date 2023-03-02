using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscillatingWindTunnel : WindTunnel
{
    [Tooltip("When true the oscillation will be truly sinusoid and when false the change will be instant")]
    [SerializeField] private bool smoothOscillation;
    [Tooltip("The period of oscillation")]
    [SerializeField] private float oscillationPeriod = 1.0f;

    ///The total time since the wind started
    private float time; 

    //The amplitude of the wind oscillation, This is the wind value set in the inspector
    private Vector3 windAmplitude;

    private void Awake()
    {
        //initialize the time
        time = 0f;
    }

    protected override void Start()
    {
        base.Start();

        //save the wind value 
        windAmplitude = getWindValue();
    }

    private void Update()
    {
        //update the time
        time += Time.deltaTime;

        //error check for a zero period
        if (oscillationPeriod == 0)
        {
            Debug.LogError("Wind Oscillation period is zero");
            return;
        }

        //update the wind
        if (smoothOscillation)
        {
            //use a sin curve
            UpdateWind(windAmplitude * Mathf.Sin((oscillationPeriod / (2 * Mathf.PI)) * time));
        }
        else
        {
            //check which half period that is currently happening by dividing time by half of 1 period
            int halfPeriod = (int)((2*time) / oscillationPeriod);

            if (halfPeriod % 2 == 0)
            {
                //if on an even halfPeriod then use wind
                UpdateWind(windAmplitude);
            }
            else
            {
                //if on an odd halfPeriod then use negative wind
                UpdateWind(-windAmplitude);
            }
        }
    }
}
