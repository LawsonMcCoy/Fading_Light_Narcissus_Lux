using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerHealth : HealthManager
{
    [SerializeField] private Sprite[] healthbarSprites; //A list of all the different sprites, partitioned by 20
    [SerializeField] private GameObject healthBar; //Reference to the healthbar object.
    private int healthBarIndex = 0;

    //For the shaking animation
    [SerializeField] private float shakeDuration = 0.7f;
    [SerializeField] private float shakeDistance = 5f;
    [SerializeField] private float delayBetweenShakes = 0f;

    //For the changing of colors
    private float lerpValue = 0f;



    public override void Subtract(float damage)
    {
        base.Subtract(damage);

        beginShakeHealthBarCoroutine();

        //Partitioned by 20 (since there are 5 leaves)
        if ((base.maxResource - base.resource) % (base.maxResource / 5) == 0)
        {
            healthBarIndex++;

            healthBar.GetComponent<Image>().sprite = healthbarSprites[healthBarIndex];
        }
    }

    // Start is called before the first frame update
    protected override void OnDeath()
    {
        Debug.Log("You died!");

        //gameObject.transform.position = saveData.spawnPoint;
        Add(1000.0f);
        healthBarIndex = 0; //Reset healthbar index
        healthBar.GetComponent<Image>().sprite = healthbarSprites[healthBarIndex]; //Update it back to full health sprite.
        EventManager.Instance.Notify(EventTypes.Events.PLAYER_DEATH);

    }

    public void beginShakeHealthBarCoroutine()
    {
        Debug.Log("Shaking now");
        StartCoroutine(ShakeHealthbar());
    }

    private IEnumerator ShakeHealthbar()
    {
        Vector3 startPos = healthBar.transform.position;
        Color origColor = healthBar.GetComponent<Image>().color;

        float timer = 0f;
        while (timer < shakeDuration)
        {
            timer += Time.deltaTime;

            lerpValue += Time.deltaTime / shakeDuration;
            healthBar.GetComponent<Image>().color = Color.Lerp(healthBar.GetComponent<Image>().color, Color.red, lerpValue);

            Vector3 newPos = startPos + (Random.insideUnitSphere * shakeDistance);
            newPos.y = healthBar.transform.position.y;
            newPos.z = healthBar.transform.position.z;

            healthBar.transform.position = newPos;

            healthBar.GetComponent<Image>().color = Color.Lerp(healthBar.GetComponent<Image>().color, origColor, lerpValue);

            if (delayBetweenShakes > 0f)
            {
                yield return new WaitForSeconds(delayBetweenShakes);
            }
            else
            {
                
                yield return null;
            }

            
        }

        //healthBar.transform.position = startPos;
        lerpValue = 0f;
    }


}
