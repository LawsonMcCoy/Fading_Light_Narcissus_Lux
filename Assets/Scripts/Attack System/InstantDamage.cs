using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantDamage : Spell_Effects
{
    public float damage;

    public override void Process(GameObject entity)
    {
        HealthManager health = entity.GetComponent<HealthManager>();
        health.Subtract(damage);
    }
}
