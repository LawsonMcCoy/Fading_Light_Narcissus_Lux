using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Spell
{
    public GameObject projectile;
    private float velocity = 10f;
    private LayerMask mask;
    //private GameObject enemy;
    //private AI_Destinations.Dest target;
    private float despawn_Distance;

    public override void Cast()
    {

        GameObject p = Instantiate(projectile, transform.position + new Vector3(0, 1, 0),
                                                transform.rotation);

        Vector3 direction_and_speed = p.transform.forward * velocity;
        p.GetComponent<Rigidbody>().AddForce(direction_and_speed, ForceMode.VelocityChange);

        ProjectileLogic logic = p.GetComponent<ProjectileLogic>();
        logic.hit.AddListener(ApplySpellEffects);
        logic.setLayerMask(mask);
        logic.SetDespawnDistanceAndVelocity(despawn_Distance, velocity);
        logic.setTag(gameObject.tag);
        logic.startTimer();

       
    
    }
    //this cast takes in a gameobject and aims the projectile at this gameObject
    public void Cast(GameObject enemy)
    {
        Debug.Log("ai took a shot");
        Quaternion angle = Quaternion.LookRotation(enemy.transform.position - transform.position);

        GameObject p = Instantiate(projectile, transform.position + new Vector3(0, 1, 0),
                                                angle);

        Vector3 direction_and_speed = p.transform.forward * velocity;
        p.GetComponent<Rigidbody>().AddForce(direction_and_speed, ForceMode.VelocityChange);

        ProjectileLogic logic = p.GetComponent<ProjectileLogic>();
        logic.hit.AddListener(ApplySpellEffects);
        logic.setLayerMask(mask);
        logic.SetDespawnDistanceAndVelocity(despawn_Distance, velocity);
        logic.setTag(gameObject.tag);
        logic.startTimer();
    }

    // Start is called before the first frame update
    void Start()
    {
        //enemy = AI_Destinations.getGameObjectFromDestination(target);
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
    public void SetDespawnDistance(float dist)
    {
        despawn_Distance = dist;
    }
    public void setVelocity(float v)
    {
        velocity = v;
    }
}
