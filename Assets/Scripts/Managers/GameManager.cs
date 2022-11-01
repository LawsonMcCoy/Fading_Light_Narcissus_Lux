using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Game Manager is a singleton
    public static GameManager Instance
    {
        get;
        private set;
    }

    private void Awake()
    {
        //set the singleton instance
        Instance = this;

        //set the singleton to not destroy on load
        DontDestroyOnLoad(this);
    }

    public void StartGame()
    {
        NarrationManager.Instance.PlayNarration();

    }

    private void OnDestroy()
    {
        //reset singleton instance to null 
        //when object is destroyed
        Instance = null;
    }
}
