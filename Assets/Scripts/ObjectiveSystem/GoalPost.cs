using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[RequireComponent(typeof(BoxCollider))]

public class GoalPost : MonoBehaviour
{
    [SerializeField] private GameObject[] Posts;
    [SerializeField] private ObjectiveSequence objective;
    private GameObject currentPost;
    private int currentIndex;

    private void Start()
    {
        Debug.Log("Goal Post Start");
        //add listener to the SO
        objective.activateObjective.AddListener(Activate);

        objective.cancel.AddListener(Cancel);
    }

    private void Cancel()
    {
        if (objective.disapear)
        {
            for (int i = 0; i < Posts.Length; i++)
            {
                Posts[i].SetActive(false);
            }
        }   
    }

    private void resetPosts()
    {
        Debug.Log("reset is called.");
        if (objective.resetCourse)
        {
            for (int i = 0; i < Posts.Length; i++)
            {
                Posts[i].SetActive(true);
            }
        }
        for (int i = 0; i < Posts.Length; i++)
        {
            Post currentPostScript = Posts[i].GetComponent<Post>();
            if(currentPostScript == null)
            {
                Posts[i].AddComponent<Post>();
            }
            Posts[i].GetComponent<Post>().initializePost(gameObject.GetComponent<GoalPost>());
        }

    }

    public void Activate()
    {
        //the activate func from old objectivemanager script
        //set the objective complete listener
        objective.objectiveCompletion.AddListener(finished);

        //activate the objective
        //Debug.Log(objective);
        //Debug.Log(objective.activateObjective);
        //objective.activateObjective.Invoke(); not needed
        //end of old objectivemanager script
        Debug.Log("Activating goal post");
        //make the posts appear and give Post class to posts
        resetPosts();     

        //set up objective
        currentIndex = 0;
        currentPost = Posts[currentIndex];
        currentPost.GetComponent<Post>().makeCurrent();
    }

    public void reachedAGoal()
    {
        //reached the goal post
        if (Posts.Length - 1 > currentIndex)
        {
            if (objective.disapear)
            {
                currentPost.SetActive(false);
            }
            currentIndex += 1;
            currentPost = Posts[currentIndex];

            if (currentPost != null)
            {
                //create new listener
                currentPost.GetComponent<Post>().makeCurrent();
            }
        }
        else
        {
            if (objective.disapear)
            {
                currentPost.SetActive(false);
            }
            Debug.Log("No more checkPoints");
            objective.reachedGoal();
        }
    }
    private void finished(int completionNumber)
    {
        //taken from old objectiveManager
        Debug.Log("Finished");
        NarrationManager.Instance.ReportCompletion();
    }
    private void OnDestroy()
    {
       // objective.activateObjective.RemoveListener(Activate);
        //objective.cancel.RemoveListener(Cancel);
    }
}
