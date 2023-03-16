using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Health : HealthManager
{
    // Start is called before the first frame update
    private CombatEntity entity;
    void Start()
    {
        entity = gameObject.GetComponent<CombatEntity>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected override void OnDeath()
    {
        entity.OnDeath();
        base.OnDeath();
    }
}
