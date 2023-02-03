using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDirection : MonoBehaviour
{
    [SerializeField] private List<Transform> allMovingPoints;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float waitTime = 1f;
    [Tooltip("Make the object move forward and backward?")]
    [SerializeField] private bool loopPath;
    [Tooltip("Make the object move in a cycle (not go backwards)? You cannot have loopPath as true if you want a cycle movement!")]
    [SerializeField] private bool loopCycle;

    private bool isMoving;
    private bool moveForwardList;
    private bool stopLoop;
    private int currentPointIndex;
    private Transform targetObj;
    
    private void Awake()
    {
        if (loopPath && loopCycle) // if both were true, only set loopPath as true
            loopCycle = false;
        
        currentPointIndex = 0;
        isMoving = false;
        moveForwardList = true;
        stopLoop = false;
    }

    private void FixedUpdate()
    {
        if (!isMoving && !allMovingPoints.Count.Equals(0)) // if there's no movement and the list is not empty
        {
            if (moveForwardList)  // move to the next forward element?
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
        if (loopCycle || loopPath)
        {
            // if either loops are triggered but stopLoop is set true during runtime
            stopLoop = false;
        }

        if (currentPointIndex == allMovingPoints.Count - 1) // if we are in the last index
        {
            if (loopPath)   // if loopPath is true, began to move backward
            {
                moveForwardList = false;
            }
            else if (!loopCycle)  // if loopPath is false and loopCycle is false
            {
                stopLoop = true; 
                targetObj = allMovingPoints[currentPointIndex];
            }
        }

        if (!stopLoop && moveForwardList)
        {
            currentPointIndex = currentPointIndex % allMovingPoints.Count;
            targetObj = allMovingPoints[currentPointIndex];
            currentPointIndex++;
        }
    }

    private void MoveNextTargetBackward()
    {
        // Move object along the list backward:
        if (loopPath && (currentPointIndex == 0)) // if loopPath is true and we are in the first index, move forward!
        {
            moveForwardList = true;
        }
        else if (loopCycle || !loopPath)
        {
            // if loopCycle was set true/loopPath was set false when moving backwards, reset the process:
            currentPointIndex = 0;
            targetObj = allMovingPoints[currentPointIndex];
            moveForwardList = true;
        }
        else
        {
            currentPointIndex = currentPointIndex % allMovingPoints.Count;
            targetObj = allMovingPoints[currentPointIndex];
            currentPointIndex--;
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
