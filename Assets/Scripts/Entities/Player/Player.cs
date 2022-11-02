using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class Player : CombatEntity
{
    [SerializeField] public PlayerInput playerInput;
    [SerializeField] private Transform spawn;
    [SerializeField] private float spawnNumber;

    private void Update()
    {
        if (this.transform.position.y <= spawnNumber && spawn != null)
        {
            this.transform.position = spawn.position;
        }
    }

    private void OnQuit()
    {
        Debug.Log("QUITTING");
        Application.Quit();
    }
}
