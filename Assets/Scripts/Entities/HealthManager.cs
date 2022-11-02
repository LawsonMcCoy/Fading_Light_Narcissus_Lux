using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Code worked on by David Arenas

public class HealthManager : ResourceManager
{
    private bool dead;

    protected override void Awake()
    {
        //initialized base class data members
        base.Awake();

        //The entity is initailized to an alive state
        dead = false;
    }

    //override the subtract function to kill the entity
    //when health drops to zero or below
    public override void Subtract(float damage)
    {
        base.Subtract(damage);

        if(resource <= 0.0)
        {
            dead = true;
            OnDeath();
        }
    }

    //A function handle the processing for when an entity dies
    //for most entitiy this is as simple as destroying the game
    //object. However, special cases may override this for special 
    //death processing
    protected virtual void OnDeath()
    {
        Destroy(gameObject);
    }
}
