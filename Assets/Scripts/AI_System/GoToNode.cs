using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GoToNode : ActionNode
{
    private GameObject myAI;
    private NavMeshAgent agent;
    private GameObject enemy;
    public AI_Destinations.Dest destination;
    protected override void OnStart()
    {
        myAI = myTree.getAI();
        agent = myAI.GetComponent<NavMeshAgent>();
        EventManager.Instance.Subscribe(EventTypes.Events.LOAD_SCENE, setEnemy);
        //enemy = AI_Actions.Instance.getDest(destination);
    }

    public void setEnemy()
    {
        //enemy = AI_Actions.Instance.getDest(destination);
        if(destination == AI_Destinations.Dest.IKA)
        {
            Debug.Log("set the enemy");
            enemy = GameObject.FindGameObjectWithTag("Player");
        }
    }
    protected override void OnStop()
    {
        //EventManager.Instance.Unsubscribe(EventTypes.Events.LOAD_SCENE, setEnemy);
        started = false;
    }

    protected override State OnUpdate()
    {
        if(enemy != null)
        {
            if (Vector3.Distance(myAI.transform.position, enemy.transform.position) > agent.stoppingDistance)
            {
                agent.SetDestination(enemy.transform.position);

                return Node.State.RUNNING;
            }
            return Node.State.SUCCESS;
        }

        return Node.State.RUNNING;
    }
}
