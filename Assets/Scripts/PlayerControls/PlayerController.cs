using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//The main script for controlling the player
public class PlayerController : MonoBehaviour
{
    //the character controller component for the player
    [SerializeField] private CharacterController Controller; 
    [SerializeField] private float Speed;

    private Vector3 PlayerMovementInput;

    private void Awake()
    {
        //initialize PlayerMovementInput
        PlayerMovementInput = Vector3.zero;
    }

    private void Update()
    {
        MovePlayer();

        // this.transform.RotateAround(this.transform.position, Vector3.up, rotationValue * Time.deltaTime);
    }

    private void MovePlayer()
    {
        Vector3 MoveVector = transform.TransformDirection(PlayerMovementInput);

        Controller.Move((MoveVector * Time.deltaTime * Speed));
    }

    private void OnMove(InputValue value)
    {
        Vector2 movementIn2D = value.Get<Vector2>();
        //Note that when going from 2D vector to 3D vector, the y in 2D becomes the z in 3D, Thanks Unity
        PlayerMovementInput = new Vector3(movementIn2D.x, 0, movementIn2D.y);
    } 


}
