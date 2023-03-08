using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//A NarrationSequence for starting an objective
[CreateAssetMenu(fileName = "ObjectiveSequence", menuName = "ObjectiveSequence")]
public class ObjectiveSequence : NarrationSequence
{
    //the Accept function for the visitor pattern
    public override void Accept()
    {
        NarrationManager.Instance.ProcessObjective(this);
    }
    //cancel function for when a player dies before the objective is finished
    public override void Cancel()
    {
        cancel.Invoke();
    }
    private int reachGoal;
    public int enemiesToKill;//better to set this in editor
    public string Description;
    public bool resetCourse;
    public bool disapear;

    /*subscribe to this event to listen for the completion of objective
    event takes an int. Decrease number of enemies to kill until 0
    or takes a 0 when reaching a goal post
     */
    [System.NonSerialized]
    public UnityEvent<int> objectiveCompletion;

    //subscriber to activate the objective
    [System.NonSerialized]
    public UnityEvent activateObjective;

    [System.NonSerialized]
    public UnityEvent cancel;
    private void OnEnable()
    {
        if (resetCourse)
        {
            reachGoal = 1; //1 is used to represesnt incomplete
        }

        //make sure the objective completion callback exists
        if (objectiveCompletion == null)
        {
            objectiveCompletion = new UnityEvent<int>();
        }

        //make sure the activateObjective callback exists
        if (activateObjective == null)
        {
            activateObjective = new UnityEvent();
        }
        if(cancel == null)
        {
            cancel = new UnityEvent();
        }
    }

    //called when enemies are killed
    public void EnemyDeath()
    {
        enemiesToKill -= 1;
        if (enemiesToKill <= 0)
        {
            if(NarrationManager.Instance != null)
            {
                NarrationManager.Instance.ReportCompletion();
            }
        }
    }

    public void reachedGoal()
    {
        reachGoal = 0;
        Debug.Log("objectiveCompletion");
        objectiveCompletion.Invoke(reachGoal);
    }

    public void Activate()
    {
        activateObjective.Invoke();
    }
}
