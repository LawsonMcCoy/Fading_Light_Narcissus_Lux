using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Spell_Effects/InstantDamage", menuName = "Spell_Effects/InstanctDamage")]

public class InstantDamage : Spell_Effects
{
    public float damage;
    

    public override void Process(GameObject entity)
    {
        
        HealthManager health = entity.GetComponent<HealthManager>();
        health.Subtract(damage);
        
    }
}
