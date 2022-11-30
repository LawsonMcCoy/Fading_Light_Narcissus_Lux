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
    [SerializeField] private float yDeathDistance;

    private void Start()
    {
        playerHealth = gameObject.GetComponent<PlayerHealth>();
        if (gameObject.GetComponent<HealthManager>() != null)
        {
            Destroy(gameObject.GetComponent<HealthManager>());
        }

        //events subscriptions
        EventManager.Instance.Subscribe(EventTypes.Events.DIALOGUE_START, DisableInput);
        EventManager.Instance.Subscribe(EventTypes.Events.DIALOGUE_END, EnableInput);
        EventManager.Instance.Subscribe(EventTypes.Events.PLAYER_DEATH, Respawn);
    }

    private void Update()
    {
        if (this.transform.position.y <= yDeathDistance && spawn != null)
        {
            //this.transform.position = spawn.position;
            //kill player
            playerHealth.Subtract(1000.0f);
        }
    }

    //A simple function to enable player input for controlling the player
    public void EnableInput()
    {
        playerInput.enabled = true;
    }

    //A simple function to diable player input for controlling the player
    public void DisableInput()
    {
        playerInput.enabled = false;
    }
    
    private void OnQuit()
    {
        Debug.Log("QUITTING");
        Application.Quit();
    }

    public void UpdateSpawn(Transform newSpawn)
    {
        spawn = newSpawn;
    }
    private void Respawn()
    {
        playerHealth.Add(1000f);    //restore health
        gameObject.transform.position = spawn.position;  //reset player position;
    }

    private void OnDestroy()
    {
        //Events unsubscriptions
        EventManager.Instance.Unsubscribe(EventTypes.Events.DIALOGUE_START, DisableInput);
        EventManager.Instance.Unsubscribe(EventTypes.Events.DIALOGUE_END, EnableInput);
        EventManager.Instance.Unsubscribe(EventTypes.Events.PLAYER_DEATH, Respawn);
    }
}
