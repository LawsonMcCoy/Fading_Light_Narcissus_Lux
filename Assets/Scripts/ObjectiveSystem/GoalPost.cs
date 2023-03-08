using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

//[RequireComponent(typeof(BoxCollider))]

public class GoalPost : MonoBehaviour
{
    [SerializeField] private GameObject[] Posts;
    [SerializeField] private ObjectiveSequence objective;
    private GameObject currentPost;
    private int currentIndex;
    private GameObject player;
    public int distance;
    private bool active;
    private bool warned;
    private bool enemyMode = false;
    private void Start()
    {
        Debug.Log("Goal Post Start");
        //add listener to the SO
        objective.activateObjective.AddListener(Activate);

        objective.cancel.AddListener(Cancel);

        if(objective.enemiesToKill > 0)
        {
            enemyMode = true;
        }
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
        //checks if holding onto enemyAI or posts
        if (enemyMode)
        {
           
        }
        else
        {


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
                if (currentPostScript == null)
                {
                    Posts[i].AddComponent<Post>();
                }
                Posts[i].GetComponent<Post>().initializePost(gameObject.GetComponent<GoalPost>());
            }
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
        if (!enemyMode)
        {
            currentIndex = 0;
            currentPost = Posts[currentIndex];
            currentPost.GetComponent<Post>().makeCurrent();


            //find player too notify if they get too far away from the objective
            player = GameObject.FindGameObjectWithTag("Player");
            active = true;
            warned = false;
            if (distance == 0)
            {
                distance = 20;
            }
        }
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
            active = false;
            objective.reachedGoal();
        }
    }
    private void finished(int completionNumber)
    {
        //taken from old objectiveManager
        Debug.Log("Finished");
        NarrationManager.Instance.ReportCompletion();
    }

    public void Update()
    {
        if (active)
        {
            if (distance < Vector3.Distance(currentPost.transform.position, player.transform.position))
            {
                if (!warned)
                {
                    EventManager.Instance.Notify(EventTypes.Events.OBJECTIVE_TOO_FAR);
                    warned = true;
                }
            }
            else
            {
                if (warned)
                {
                    EventManager.Instance.Notify(EventTypes.Events.OBJECCTIVE_WITHIN_DISTANCE);
                    warned = false;
                }
            }
        }
    }
    private void OnDestroy()
    {
       // objective.activateObjective.RemoveListener(Activate);
        //objective.cancel.RemoveListener(Cancel);
    }
}
