using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : ActionNode
{
    private GameObject myAI;
    private Melee attackSpell;
    public LayerMask mask;

    public float coolDownInSeconds;
    private float nextAttackTime;

    public void Awake()
    {
        nextAttackTime = Time.time;
        myAI = myTree.getAI();
        attackSpell = myAI.GetComponent<Melee>();
        attackSpell.setLayerMask(mask);
    }
    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if (Time.time > nextAttackTime)
        {
            attackSpell.Cast();
            nextAttackTime = Time.time + coolDownInSeconds;
            return Node.State.SUCCESS;
        }
        else
        {
            return Node.State.RUNNING;
        }
    }
}
