using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class Player : CombatEntity
{
    [SerializeField] public PlayerInput playerInput;

    private void OnQuit()
    {
        Debug.Log("QUITTING");
        Application.Quit();
    }
}
