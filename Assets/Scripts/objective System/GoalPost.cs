using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider))]

public class GoalPost : MonoBehaviour
{
    public GameObject Post;
    public ObjectiveScriptableObject objectiveScript;

    void Start()
    {
        GetComponent<BoxCollider>().isTrigger = true;
        Post.SetActive(true);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //reached the goal post
            Post.SetActive(false);
            objectiveScript.reachedGoal();
        }
    }

}
