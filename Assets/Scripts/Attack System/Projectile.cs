using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Spell
{
    public GameObject projectile;
    public float velocity = 10f;
    public float coolDown = 5f;
    private LayerMask mask;
    private GameObject enemy;
    public AI_Destinations.Dest target;

    public override void Cast()
    {
       Quaternion angle = Quaternion.LookRotation(enemy.transform.position - transform.position);

        GameObject p = Instantiate(projectile, transform.position + new Vector3(0, 1, 0),
                                                angle);
        ProjectileLogic logic = p.GetComponent<ProjectileLogic>();

        logic.hit.AddListener(ApplySpellEffects);
        logic.setLayerMask(mask);
        Vector3 direction_and_speed = p.transform.forward * velocity;
        p.GetComponent<Rigidbody>().AddForce(direction_and_speed, ForceMode.VelocityChange);
    
    }

    // Start is called before the first frame update
    void Start()
    {
        enemy = AI_Destinations.getGameObjectFromDestination(target);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void ApplySpellEffects(GameObject entity)
    {
        for (int j = 0; j < effects.Count; j++)
        {
            effects[j].Process(entity);
        }
    }

    public void setLayerMask(LayerMask mask)
    {
        this.mask = mask;
    }
}
