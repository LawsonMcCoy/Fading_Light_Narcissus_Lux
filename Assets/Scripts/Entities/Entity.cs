using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//code worked on by david arenas

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(HealthManager))]

public abstract class Entity : MonoBehaviour
{
    //references to required components
    [SerializeField] public new Rigidbody rigidbody;
    [SerializeField] public new Collider collider;

    private HealthManager health;

    // Start is called before the first frame update
    protected virtual void Start()
    {
            health = GetComponent<HealthManager>();
    }

// Update is called once per frame
void Update()
    {
        
    }
}