using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Melee))]
[RequireComponent(typeof(AI_Health))]

public class MeleeEntity : CombatEntity
{
    [HideInInspector] public NavMeshAgent agent;
    public BehaviorTree tree;
    public ObjectiveSequence objective;
    private bool activated;
    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.GetComponent<HealthManager>().GetType() == typeof(HealthManager))
        {
            Destroy(gameObject.GetComponent<HealthManager>());
        }

        agent = this.GetComponent<NavMeshAgent>();
        tree.setAI(this.gameObject);
        tree = tree.Clone();
        if (objective != null)
        {
            activated = false;
            objective.activateObjective.AddListener(activateAI);
        }
        else
        {
            activated = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // agent.SetDestination(Aika.transform.position);
        if (activated)
        {
            tree.Update();
        }
    }
    public override void OnDeath()
    {
        if(objective != null)
        {
            objective.EnemyDeath();
        }
    }
    public void activateAI()
    {
        activated = true;
    }
}
