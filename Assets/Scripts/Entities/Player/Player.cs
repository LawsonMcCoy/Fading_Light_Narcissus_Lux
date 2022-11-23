using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class Player : CombatEntity
{
    [SerializeField] public PlayerInput playerInput;
    [SerializeField] private Transform spawn;
    [SerializeField] private float yDeathDistance;
    [SerializeField] private float positionToCenterDistance;

    public Vector3 center
    {
        get;
        private set;
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

        //Right now the position of the Ika's model is at the base of the model
        //This results in issues with other parts of the code. This line is meant 
        //to calculate the actually center of the model by translating the position 
        //up (locally) by half of the model y world scale.
        center = this.transform.position + (positionToCenterDistance * this.transform.up);
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
