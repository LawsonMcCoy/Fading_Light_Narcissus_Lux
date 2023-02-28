using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Projectile))]

public class MageEntity : CombatEntity
{

    [HideInInspector]public NavMeshAgent agent;
    public BehaviorTree tree;
    // Start is called before the first frame update
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        tree.setAI(this.gameObject);
        tree = tree.Clone();
    }

    // Update is called once per frame
    void Update()
    {
        // agent.SetDestination(Aika.transform.position);
        tree.Update();
    }
}
