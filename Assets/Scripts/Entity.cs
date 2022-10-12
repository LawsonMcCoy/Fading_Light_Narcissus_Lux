using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]

public abstract class Entity : MonoBehaviour
{
    //references to required components
    [SerializeField] public Rigidbody rigidbody;
    [SerializeField] public Collider collider;
}