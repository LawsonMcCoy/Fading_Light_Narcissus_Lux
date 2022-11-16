using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : HealthManager
{
    [SerializeField]private SaveScrtiptableObject saveData;
    // Start is called before the first frame update
    protected override void OnDeath()
    {
        Debug.Log("You died!");
        saveData.playerDeath.Invoke();
        gameObject.transform.position = saveData.spawnPoint;
    }
}
