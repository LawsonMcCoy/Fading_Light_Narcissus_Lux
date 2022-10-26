using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[RequireComponent(typeof(BoxCollider))]

public class GoalPost : MonoBehaviour
{
    public GameObject[] Posts;
    public ObjectiveScriptableObject objectiveScript;
    private GameObject currentPost;
    private int currentIndex;
    private BoxCollider currentCollider;

    void Start()
    {
        //reset course if necessary
        resetPosts();
        //make sure each checkpoint has collider
        ColliderCheck();
        //make sure each cheackpoint has collisionDetect
        CollisionDetectcheck();
        currentIndex = 0;
        currentPost = Posts[currentIndex];
        currentPost.GetComponent<CollisionDetect>().makeCurrent();

        currentCollider = currentPost.GetComponent<BoxCollider>();
    }

    private void CollisionDetectcheck()
    {
        for (int i = 0; i < Posts.Length; i++)
        {
            GameObject CP = Posts[i];
            if (CP != null)
            {
                CollisionDetect cd = CP.GetComponent<CollisionDetect>();

                if (cd == null)
                {
                    CP.AddComponent<CollisionDetect>();
                    cd = CP.GetComponent<CollisionDetect>();
                    cd.giveGoalPost(gameObject.GetComponent<GoalPost>());
                }
                else
                {
                    cd.giveGoalPost(gameObject.GetComponent<GoalPost>());
                }
            }


        }
    }
    private void resetPosts()
    {
        if (objectiveScript.resetCourse)
        {
            for (int i = 0; i < Posts.Length; i++)
            {
                Posts[i].SetActive(true);
            }
        }
        
    }
    private void ColliderCheck()
    {
        for (int i = 0; i < Posts.Length; i++)
        {
            GameObject CP = Posts[i];
            if (CP != null)
            {
                BoxCollider BC = CP.GetComponent<BoxCollider>();

                if (BC == null)
                {
                    CP.AddComponent<BoxCollider>();
                    BC = CP.GetComponent<BoxCollider>();
                }
                else
                {
                    BC.isTrigger = true;
                }
            }
            
         
        }
       
    }

    public void reachedAGoal()
    {
        //reached the goal post
        if (Posts.Length - 1 > currentIndex)
        {
            currentIndex += 1;
            currentPost = Posts[currentIndex];

            if (currentPost != null)
            {
                //create new listener
                currentPost.GetComponent<CollisionDetect>().makeCurrent();
            }
        }
        else
        {
            Debug.Log("No more checkPoints");
            objectiveScript.reachedGoal();
        }
    }

}
