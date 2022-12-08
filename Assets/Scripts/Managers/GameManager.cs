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

        //Events subscription
        Debug.Log(EventManager.Instance);
        EventManager.Instance.Subscribe(EventTypes.Events.DIALOGUE_START, PauseGame);
        EventManager.Instance.Subscribe(EventTypes.Events.DIALOGUE_END, UnpauseGame);
    }

    public void StartGame()
    {
        NarrationManager.Instance.PlayNarration();

    }

    //A funciton to pause the game time
    public void PauseGame()
    {
        Time.timeScale = 0; //pause game temp
    }

    //A function to resume normal game time
    public void UnpauseGame()
    {
        Time.timeScale = 1;
    }

    private void OnDestroy()
    {
        //reset singleton instance to null 
        //when object is destroyed
        Instance = null;

        //Events unsubscription
        EventManager.Instance.Unsubscribe(EventTypes.Events.DIALOGUE_START, PauseGame);
        EventManager.Instance.Unsubscribe(EventTypes.Events.DIALOGUE_END, UnpauseGame);
    }
}
