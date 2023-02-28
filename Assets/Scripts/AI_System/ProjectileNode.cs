using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ProjectileNode : ActionNode
{
    private GameObject myAI;
    private Projectile projectileSpell;
    public LayerMask mask;
    public AI_Destinations.Dest target;
    private GameObject enemy;

    public float coolDownInSeconds;
    private float nextAttackTime;

    public void Awake()
    {
        myAI = myTree.getAI();
        projectileSpell = myAI.GetComponent<Projectile>();
        projectileSpell.setLayerMask(mask);
        nextAttackTime = Time.time;
        enemy = AI_Destinations.getGameObjectFromDestination(target);
    }
    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        Vector3 dirFromAIToEnemey = (enemy.transform.position - myAI.transform.position).normalized;

        float dotProd = Vector3.Dot(dirFromAIToEnemey, myAI.transform.forward);

        if (dotProd > 0.95)
        {
            // ObjA is looking mostly towards ObjB
            if (Time.time > nextAttackTime)
            {
                projectileSpell.Cast();
                nextAttackTime = Time.time + coolDownInSeconds;
                return Node.State.SUCCESS;
            }

        }
        else
        {
            var targetRotation = Quaternion.LookRotation(enemy.transform.position - myAI.transform.position);

            // Smoothly rotate towards the target point.
            myAI.transform.rotation = Quaternion.Slerp(myAI.transform.rotation, targetRotation, Time.deltaTime);
        }
        return Node.State.RUNNING;
    }
}
