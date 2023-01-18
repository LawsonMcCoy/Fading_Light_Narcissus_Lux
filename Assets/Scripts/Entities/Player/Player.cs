using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

[RequireComponent(typeof(PlayerHealth))]
public class Player : CombatEntity
{
    [SerializeField] public PlayerInput playerInput;
    [SerializeField] private Vector3 spawn;
    [SerializeField] private float spawnNumber;
    private PlayerHealth playerHealth;
    [SerializeField] private float yDeathDistance;

    private Vector3 defaultScale;
    
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
        EventManager.Instance.Subscribe(EventTypes.Events.SAVE, UpdateSpawn);

        defaultScale = this.transform.localScale;
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

    public void UpdateSpawn()
    {
        spawn = NarrationManager.Instance.getSpawn();
    }
    private void Respawn()
    {
        playerHealth.Add(1000f);    //restore health
        gameObject.transform.position = spawn;  //reset player position;
    }

    private void OnDestroy()
    {
        //Events unsubscriptions
        EventManager.Instance.Unsubscribe(EventTypes.Events.DIALOGUE_START, DisableInput);
        EventManager.Instance.Unsubscribe(EventTypes.Events.DIALOGUE_END, EnableInput);
        EventManager.Instance.Unsubscribe(EventTypes.Events.PLAYER_DEATH, Respawn);
        EventManager.Instance.Unsubscribe(EventTypes.Events.SAVE, UpdateSpawn);
    }

    //Helper functions:
    public Vector3 GetPlayerScale
    {
        get { return defaultScale; }
        private set { }
    }
}
