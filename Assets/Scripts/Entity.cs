using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//code worked on by david arenas

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(HealthManager))]

public class Entity : MonoBehaviour
{
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