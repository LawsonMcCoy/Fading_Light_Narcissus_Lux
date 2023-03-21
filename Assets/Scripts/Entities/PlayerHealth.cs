using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerHealth : HealthManager
{
    [SerializeField] private Sprite[] healthbarSprites; //A list of all the different sprites, partitioned by 20
    [SerializeField] private GameObject healthBar; //Reference to the healthbar object.
    private int healthBarIndex = 0; //Index for the array of healthbar sprites.
    private bool isBeingDamaged = false; //A boolean value to check if we're currently being damaged (vulnerability cooldown)

    //For the shaking animation
    [SerializeField] private float shakeDuration = 0.7f;
    [SerializeField] private float shakeDistance = 5f;
    private Vector3 startPos;

    //For the changing of colors
    private float lerpValue = 0f;

    private void Start()
    {
        startPos = healthBar.transform.position;
    }


    public override void Subtract(float damage)
    {
        //Check to see if we're currently being damaged. If not,
        if (isBeingDamaged == false)
        {
            //We are now being damaged.
            isBeingDamaged = true;
            base.Subtract(damage);

            beginShakeHealthBarCoroutine();

            //Partitioned by 20 (since there are 5 leaves)
            //Make sure that the health isn't 100 (b/c 100-100 or 0 mod 20 is also 0)
            if ((base.maxResource - base.resource) % (base.maxResource / 5) == 0 && base.maxResource != base.resource)
            {

                healthBarIndex++;
                healthBar.GetComponent<Image>().sprite = healthbarSprites[healthBarIndex];
            }
        }
    }

    // Start is called before the first frame update
    protected override void OnDeath()
    {
        Debug.Log("You died!");

        //Restore player's health back to full.
        Add(1000.0f);
        healthBarIndex = 0; //Reset healthbar index
        healthBar.GetComponent<Image>().sprite = healthbarSprites[healthBarIndex]; //Update it back to the full health sprite.
        isBeingDamaged = false; //Reset the boolean that checks if we're being damaged back to false.
        EventManager.Instance.Notify(EventTypes.Events.PLAYER_DEATH);

    }

    public void beginShakeHealthBarCoroutine()
    {
        StartCoroutine(ShakeHealthbar());
    }

    private IEnumerator ShakeHealthbar()
    {
        float timer = 0f;
        while (timer < shakeDuration)
        {
            timer += Time.deltaTime;

            lerpValue += Time.deltaTime / shakeDuration;
            healthBar.GetComponent<Image>().color = Color.Lerp(healthBar.GetComponent<Image>().color, Color.red, lerpValue);

            Vector3 newPos = startPos + (Random.insideUnitSphere * shakeDistance);
            //newPos.y = healthBar.transform.position.y;
            //newPos.z = healthBar.transform.position.z;

            healthBar.transform.position = newPos;

            healthBar.GetComponent<Image>().color = Color.Lerp(healthBar.GetComponent<Image>().color, Color.white, lerpValue);

            yield return null;

        }

        healthBar.transform.position = startPos;
        lerpValue = 0f;
        //healthBar.GetComponent<Image>().color = Color.white;
        isBeingDamaged = false;
    }


}
