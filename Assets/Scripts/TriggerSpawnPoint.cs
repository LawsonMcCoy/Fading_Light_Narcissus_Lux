using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSpawnPoint : MonoBehaviour
{
    [SerializeField] private string objTag;
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private string hexColor = "#32a852";
    [SerializeField] private float newSimulationSpeed = 0.4f;

    private ParticleSystem.MainModule main;
    private float originalSpeed;
    private ParticleSystem.MinMaxGradient originalColor;
    private Color newColor;

    private void Start()
    {
        main = particle.main;
        originalSpeed = main.simulationSpeed;
        originalColor = main.startColor;

        if (!hexColor.Contains("#"))
        {
           hexColor = "#" + hexColor; // add # in front of hexcode
        }

        ColorUtility.TryParseHtmlString(hexColor, out newColor);
    }

    private void OnTriggerEnter(Collider other)
    {
        // if player
        if (other.gameObject.tag == objTag)
        {
            Player player = other.gameObject.GetComponent<Player>();
            player.UpdateSpawn(this.transform);

            main.startColor = newColor;
            main.simulationSpeed = newSimulationSpeed;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // if player
        if (other.gameObject.tag == objTag)
        {
            main.startColor = originalColor;
            main.simulationSpeed = originalSpeed;
        }
    }
}
