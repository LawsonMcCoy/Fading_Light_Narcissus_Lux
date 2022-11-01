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
    [SerializeField] public Rigidbody rigidbody;
    [SerializeField] public Collider collider;

    private HealthManager health;

    // Start is called before the first frame update
    void Start()
    {
            health = GetComponent<HealthManager>();
    }

// Update is called once per frame
void Update()
    {
        
    }
}