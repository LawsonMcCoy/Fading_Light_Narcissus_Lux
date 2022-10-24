using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[RequireComponent(typeof(Text))]

public class GoalPost : MonoBehaviour
{
    public GameObject Post;
    public ObjectiveScriptableObject objectiveScript;

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
