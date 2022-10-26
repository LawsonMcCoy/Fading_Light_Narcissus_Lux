using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetect : MonoBehaviour
{
    private bool isCurrent;
    private GoalPost goalPost;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void makeCurrent()
    {
        isCurrent = true;
    }
    public void giveGoalPost(GoalPost goal)
    {
        goalPost = goal;
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
                Debug.Log("Reached a goal");
                goalPost.reachedAGoal();
            }
        }
 
    }

}
