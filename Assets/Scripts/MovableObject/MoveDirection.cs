using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDirection : MonoBehaviour
{
    [SerializeField] private List<Transform> allMovingPoints;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float waitTime = 1f;
    [Tooltip("Make the object move back and forth?")]
    [SerializeField] private bool loopPath;

    private bool isMoving = false;
    private bool moveForwardList = true;
    private int currentPointIndex = 0;
    private Transform targetObj;

    private void FixedUpdate()
    {
        if (!isMoving && !allMovingPoints.Count.Equals(0)) // if there's no movement and the list is not empty
        {
            if (moveForwardList)
            {
                MoveNextTargetForward();
            }
            else
            {
                MoveNextTargetBackward();
            }
         
            // if object is not on target, move towards target
            if (this.transform.position != targetObj.position)
            { 
                isMoving = true;
                StartCoroutine(MoveObj(targetObj));
            }
        }
    }

    private void MoveNextTargetForward()
    {
        // Move object along the list forward:
        if (currentPointIndex < allMovingPoints.Count - 1)
        {
            targetObj = allMovingPoints[currentPointIndex + 1];
            currentPointIndex++;
        }
        else 
        {
            // on last element
            if(loopPath)
                moveForwardList = false;
        }
    }

    private void MoveNextTargetBackward()
    {
        // Move object along the list backward:
        if(currentPointIndex > 0)
        {
            targetObj = allMovingPoints[currentPointIndex - 1];
            currentPointIndex--;
        }
        else
        {
            if (loopPath)
                moveForwardList = true;
        }
    }

    private IEnumerator MoveObj(Transform targetPos)
    {
        
        while (this.transform.position != targetPos.position)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, targetPos.position, moveSpeed * Time.deltaTime);
            yield return null;
        }
        
        yield return new WaitForSeconds(waitTime);
        isMoving = false;
    }
}
