using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDirection : MonoBehaviour
{
    [SerializeField] private int moveUnits;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float waitTime;
    private bool isMoving = false;
    
    // Update is called once per frame
    void Update()
    {
        if (!isMoving)
        {
            StartCoroutine(MoveHorizontal(moveUnits));
            moveUnits *= -1;  // change to opposite direction
            isMoving = true;
        }
    }

    private IEnumerator MoveHorizontal(float units)
    {
        yield return new WaitForSeconds(waitTime);
        float totalTime = 0;
        float originX = transform.position.x;
        while (totalTime < waitTime)
        {
            float x = Mathf.Lerp(0, units, totalTime / moveSpeed);
            transform.position = new Vector3(originX + x, transform.position.y, transform.position.z);
            totalTime += Time.deltaTime;
            yield return null;
        }
        isMoving = false;
    }
}
