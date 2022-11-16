using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

[RequireComponent(typeof(PlayerHealth))]
public class Player : CombatEntity
{
    [SerializeField] public PlayerInput playerInput;
    [SerializeField] private Transform spawn;
    [SerializeField] private float spawnNumber;
    private PlayerHealth playerHealth;
    protected override void Start()
    {
        playerHealth = gameObject.GetComponent<PlayerHealth>();
        if(gameObject.GetComponent<HealthManager>() != null)
        {
            Destroy(gameObject.GetComponent<HealthManager>());
        }
    }
    private void Update()
    {
        if (this.transform.position.y <= spawnNumber && spawn != null)
        {
            //this.transform.position = spawn.position;
            //kill player
            playerHealth.Subtract(1000.0f);
        }
    }

    private void OnQuit()
    {
        Debug.Log("QUITTING");
        Application.Quit();
    }
}
