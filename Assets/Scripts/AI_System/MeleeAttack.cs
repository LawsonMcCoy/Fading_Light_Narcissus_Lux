using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : ActionNode
{
    private GameObject myAI;
    private Melee attackSpell;
    public LayerMask mask;
    protected override void OnStart()
    {

        myAI = myTree.getAI();
        attackSpell = myAI.GetComponent<Melee>();
        attackSpell.setLayerMask(mask);
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        attackSpell.Cast();
        return Node.State.FAILURE;
    }
}
