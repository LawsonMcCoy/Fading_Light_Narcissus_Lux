using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Code worked on by David Arenas

public class HealthManager : MonoBehaviour
{
    [SerializeField]
    private float health;
    public float maxHealth;
    public bool dead;
    // Start is called before the first frame update
    void Start()
    {
        dead = false;
    }

    public void add(float healthPoints)
    {
        health += healthPoints;
        if(health > maxHealth)
        {
            health = maxHealth;
        }
    }

    public void subtract(float damage)
    {
        health -= damage;
        if(health >= 0.0)
        {
            dead = true;
            OnDeath();
        }
    }
    protected virtual void OnDeath()
    {
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
