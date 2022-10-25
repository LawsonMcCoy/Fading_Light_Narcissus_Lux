using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                currentNumberInList += 1;
                currentObjective = objectives[currentNumberInList];
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
