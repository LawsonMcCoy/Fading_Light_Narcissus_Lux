using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class Player : CombatEntity
{
    [SerializeField] public PlayerInput playerInput;
    [SerializeField] private Transform spawn;
    [SerializeField] private float yDeathDistance;

    //A property with the most updated active movement mode
    public MovementMode activeMovementMode
    {
        get;
        set;
    }

    private void Start()
    {
        //events subscriptions
        EventManager.Instance.Subscribe(EventTypes.Events.DIALOGUE_START, DisableInput);
        EventManager.Instance.Subscribe(EventTypes.Events.DIALOGUE_END, EnableInput);
    }

    private void Update()
    {
        if (this.transform.position.y <= yDeathDistance && spawn != null)
        {
            this.transform.position = spawn.position;
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

    private void OnDestroy()
    {
        //Events unsubscriptions
        EventManager.Instance.Unsubscribe(EventTypes.Events.DIALOGUE_START, DisableInput);
        EventManager.Instance.Unsubscribe(EventTypes.Events.DIALOGUE_END, EnableInput);
    }
}
