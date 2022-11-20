using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]

public class Post : MonoBehaviour
{

    private GoalPost myGoalPostRef;
    private bool isCurrent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void initializePost(GoalPost goal)
    {
        gameObject.GetComponent<BoxCollider>().isTrigger = true;
        myGoalPostRef = goal;
    }
    public void makeCurrent()
    {
        isCurrent = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isCurrent)
        {
            if (other.CompareTag("Player"))
            {
                //reached the goal post
                gameObject.SetActive(false);
                isCurrent = false;
                Debug.Log(myGoalPostRef);
                myGoalPostRef.reachedAGoal();
            }
        }

    }
}
