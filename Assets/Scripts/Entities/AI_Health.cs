using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AI_Health : HealthManager
{
    // Start is called before the first frame update
    private CombatEntity entity;
    private Camera cam;
    public Slider bar;
    void Start()
    {
        entity = gameObject.GetComponent<CombatEntity>();
        bar.value = 1;

        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    //overrides add function so that the health bar is updated
    public override void Add(float amountToAdd)
    {
        base.Add(amountToAdd);
        bar.value = resource / maxResource;
    }

    //override subtract function so that the health bar is updated
    public override void Subtract(float damage)
    {
        base.Subtract(damage);
        bar.value = resource / maxResource;
    }
    // Update is called once per frame
    void Update()
    {
        //change rotation of healthbar to face the main cmaera
        bar.transform.rotation = Quaternion.LookRotation(bar.transform.position - cam.transform.position);
    }
    protected override void OnDeath()
    {
        entity.OnDeath();
        base.OnDeath();
    }
}
