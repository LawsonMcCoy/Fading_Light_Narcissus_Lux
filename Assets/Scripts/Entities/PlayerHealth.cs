using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerHealth : HealthManager
{
    [SerializeField] private Sprite[] healthbarSprites;
    [SerializeField] private GameObject healthBar;

    public override void Subtract(float damage)
    {
        base.Subtract(damage);

        if (base.resource == 80)
        {
            healthBar.GetComponent<Image>().sprite = healthbarSprites[1];
        }
        else if (base.resource == 60)
        {
            healthBar.GetComponent<Image>().sprite = healthbarSprites[2];
        }
        else if (base.resource == 40)
        {
            healthBar.GetComponent<Image>().sprite = healthbarSprites[3];
        }
        else if (base.resource == 20)
        {
            healthBar.GetComponent<Image>().sprite = healthbarSprites[4];
        }
        else if (base.resource == 0)
        {
            healthBar.GetComponent<Image>().sprite = healthbarSprites[5];
        }
    }

    public override void Add(float amountToAdd)
    {
        base.Add(amountToAdd);
        if (base.resource == base.maxResource)
        {
            healthBar.GetComponent<Image>().sprite = healthbarSprites[0];
        }
    }

    // Start is called before the first frame update
    protected override void OnDeath()
    {
        Debug.Log("You died!");

        //gameObject.transform.position = saveData.spawnPoint;
        Add(1000.0f);
        EventManager.Instance.Notify(EventTypes.Events.PLAYER_DEATH);

    }
}
