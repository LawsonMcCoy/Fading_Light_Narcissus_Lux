using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ScriptableObject", menuName = "ScriptableObjects/objective")]

public class ObjectiveScriptableObject : ScriptableObject
{
    private int reachGoal;
    public int enemiesToKill;//better to set this in editor
    public string Description;
    public bool resetCourse;

    /*subscribe to this event to listen for the completion of objective
    event takes an int. Decrease number of enemies to kill until 0
    or takes a 0 when reaching a goal post
     */
    [System.NonSerialized]
    public UnityEvent<int> objectiveCompletion;


    private void OnEnable()
    {
        if (resetCourse)
        {
            reachGoal = 1; //1 is used to represesnt incomplete
        }
        if(objectiveCompletion == null)
        {
            objectiveCompletion = new UnityEvent<int>();
        }
    }

    //called when enemies are killed
    public void decreaseEnemies(int amount)
    {
        enemiesToKill -= amount;
        if(enemiesToKill <= 0)
        {
            objectiveCompletion.Invoke(enemiesToKill);
        }
    }

    //called inside of GoalPost script, inside OnTriggerEnter function
    public void reachedGoal()
    {
        reachGoal = 0;
        objectiveCompletion.Invoke(reachGoal);
    }
}
