using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//code worked on by david arenas
public class ObjectiveManager : MonoBehaviour
{
    // public ObjectiveScriptableObject[] objectives;
    // private ObjectiveScriptableObject currentObjective;
    private int currentNumberInList;

    //Objective Manager is a singleton
    public static ObjectiveManager Instance
    {
        get;
        private set;
    }

    private void Awake()
    {
        //set the singleton instance
        Instance = this;

        //set the singleton to not destroy on load
        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        // currentNumberInList = 0;
        // currentObjective = objectives[currentNumberInList];
        // if(currentObjective != null)
        // {
        //     currentObjective.objectiveCompletion.AddListener(finished);
        // }
    }
    private void finished(int completionNumer)//0 for complete
    {
        Debug.Log("Finished");
        NarrationManager.Instance.ReportCompletion();
        // if(completionNumer == 0)
        // {
        //     if(objectives.Length - 1 > currentNumberInList)
        //     {
        //         //remove old listener
        //         // currentObjective.objectiveCompletion.RemoveListener(finished);
        //         Debug.Log("mission accomplished");
        //         currentNumberInList += 1;
        //         // currentObjective = objectives[currentNumberInList];
        //         if (currentObjective != null)
        //         {
        //             //create new listener
        //             // currentObjective.objectiveCompletion.AddListener(finished);
        //         }
        //     }
        //     else
        //     {
        //         Debug.Log("No more objectives");
        //         NarrationManager.Instance.ReportCompletion();
        //     }
        // }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate(ObjectiveScriptableObject objective)
    {
        //set the objective complete listener
        objective.objectiveCompletion.AddListener(finished);

        //activate the objective
        Debug.Log(objective);
        Debug.Log(objective.activateObjective);
        objective.activateObjective.Invoke();
    }

    private void OnDestroy()
    {
        //set the instance to null when object is destoryed
        Instance = null;
    }
}
