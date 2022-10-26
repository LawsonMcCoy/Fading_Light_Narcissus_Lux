using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
   [SerializeField] private Player player;
   private Vector3 lastPlayerPosition;

   private void Start()
   {
        lastPlayerPosition = player.transform.position;
   }

   private void FixedUpdate()
   {
        Vector3 playerDisplacement = player.transform.position - lastPlayerPosition;

        Debug.DrawLine(this.transform.position, this.transform.position + playerDisplacement.normalized * 50, Color.green);
        this.transform.Translate(playerDisplacement);

        lastPlayerPosition = player.transform.position;
   }
   
}
