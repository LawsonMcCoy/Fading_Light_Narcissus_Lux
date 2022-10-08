using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//The Narration Manager will be responsible for progressing the 
//story of the game. It will load scenes, trigger enemy spawns, begin and end
//narration event, handle when dialogue should be display, and set up objectives.
//Of course it doesn't do most of this stuff itself, but rather tells the apporiate
//systems when to do everything. 
public class NarrationManager : MonoBehaviour
{
    //The narration of the game, broken down into sequences
    [SerializeField] private List<NarrationSequence> narration; 

    //Narration Manager is a singleton
    public static NarrationManager Instance
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

    //The primary function that will play the narration of the game
    //In short this function plays the game
    public void PlayNarration()
    {
        foreach (NarrationSequence sequence in narration)
        {
            //process each sequence by visiting them
            sequence.Accept();
        }
    }

    //Visitor pattern process functions
    public void ProcessLoadScene(LoadSceneSequence sequence)
    {
        //load the scene for the narration sequence
        SceneManager.LoadScene((int)sequence.scene);
    }

    private void OnDestroy()
    {
        //reset singleton instance to null 
        //when object is destroyed
        Instance = null;
    }
}
