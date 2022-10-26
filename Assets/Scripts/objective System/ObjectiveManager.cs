using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//code worked on by david arenas
public class ObjectiveManager : MonoBehaviour
{
    public ObjectiveScriptableObject[] objectives;
    private ObjectiveScriptableObject currentObjective;
    private int currentNumberInList;
    // Start is called before the first frame update
    void Start()
    {
        currentNumberInList = 0;
        currentObjective = objectives[currentNumberInList];
        if(currentObjective != null)
        {
            currentObjective.objectiveCompletion.AddListener(finished);
        }
    }
    void finished(int completionNumer)//0 for complete
    {
        if(completionNumer == 0)
        {
            if(objectives.Length > currentNumberInList - 1)
            {
                //remove old listener
                currentObjective.objectiveCompletion.RemoveListener(finished);
                currentNumberInList += 1;
                currentObjective = objectives[currentNumberInList];
                //create new listener
                currentObjective.objectiveCompletion.AddListener(finished);
            }
            else
            {
                Debug.Log("No more objectives");
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
