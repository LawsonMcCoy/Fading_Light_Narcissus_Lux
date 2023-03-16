using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : HealthManager
{
    // Start is called before the first frame update
    protected override void OnDeath()
    {
        Debug.Log("You died!");

        //gameObject.transform.position = saveData.spawnPoint;
        Add(1000.0f);
        EventManager.Instance.Notify(EventTypes.Events.PLAYER_DEATH);

    }
}
