using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SaveSequence", menuName = "SaveSequence")]

public class SaveSequence : NarrationSequence
{
    //the Accept function for the visitor pattern
  /*  private void Start()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        Player =obj.GetComponent<Player>();
    }
  */
    public override void Accept()
    {
        NarrationManager.Instance.ProcessSave(this);
        if (Player == null)
        {
            Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }
        Player.UpdateSpawn(newCoordinates);
    }

    //data

    public Scenes.ScenesList saveScene; //set this in editor
    public bool saveSuccesful; //bool to know if save was valid
                               //made false if the save was not in correct scene in narrationManager
    public Vector3 newCoordinates;
    private Player Player;
}
