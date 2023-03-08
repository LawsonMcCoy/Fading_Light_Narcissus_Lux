using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GoToNode : ActionNode
{
    private GameObject myAI;
    private NavMeshAgent agent;
    private GameObject enemy;
    private bool isSetUp;
    public AI_Destinations.Dest destination;
    

    public void Awake()
    {
        /*  myAI = myTree.getAI();
          agent = myAI.GetComponent<NavMeshAgent>();
          enemy = AI_Destinations.getGameObjectFromDestination(destination);
        */
        isSetUp = false;
    }
    protected override void OnStart()
    {
        if (!isSetUp)
        {
            myAI = myTree.getAI();
            agent = myAI.GetComponent<NavMeshAgent>();
            enemy = AI_Destinations.getGameObjectFromDestination(destination);
            Debug.Log(myAI.name);
            Debug.Log(myTree.name);
            isSetUp = true;
        }
        //EventManager.Instance.Subscribe(EventTypes.Events.LOAD_SCENE, setEnemy);
        //enemy = AI_Actions.Instance.getDest(destination);
    }


    protected override void OnStop()
    {
        //EventManager.Instance.Unsubscribe(EventTypes.Events.LOAD_SCENE, setEnemy);
        //started = false;
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
