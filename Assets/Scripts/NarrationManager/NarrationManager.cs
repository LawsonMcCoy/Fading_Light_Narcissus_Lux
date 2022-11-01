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

    private bool narrationPaused; //a boolean that says wheter or not the narration is currently paused
    private int currentNarrationSequenceIndex; //An integer representing the index of the current 
                                               //narration sequence
    private int savedNarrationSequenceIndex; //An integer representing the index of the narration 
                                             //sequence that was last saved, Note this value will
                                             //made to be persistent later

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

        //initialize data members
        narrationPaused = true; //The narration start paused and remain so until play game is pressed
        currentNarrationSequenceIndex = 0; //start at the begining of the narration
        savedNarrationSequenceIndex = 0; //The first save is the being of the narration
    }

    //The primary function that will play the narration of the game
    //In short this function plays the game
    public void PlayNarration()
    {
        //later add code to set the current index to the start point
        //probably wherever the save index is

        //unpause the narration
        narrationPaused = false;
    }

    //Update loop for narration manager
    //will attempt to play the narration until it is either paused
    //or finishes. If the narration is currently paused it will do
    //nothing
    private void LateUpdate()
    {
        while (!narrationPaused && currentNarrationSequenceIndex != narration.Count)
        {
            //error checking
            if (currentNarrationSequenceIndex < 0 || currentNarrationSequenceIndex > narration.Count)
            {
                Debug.LogError("Narration is out of bounds");
            }

            //process each sequence by visiting them
            narration[currentNarrationSequenceIndex].Accept();

            //increment the current index so the next iteration will 
            //visit the next sequence
            currentNarrationSequenceIndex++;
        }

        //uncomment this code when objectives are put into the game
        //objective sequences will pause the narration giving the player a 
        //to actually play the game
        // //check if game is completed
        // if (currentNarrationSequenceIndex == narration.Count)
        // {
        //     Debug.Log("Game is completed");

        //     //The game is completed load main menu, reset current index, and
        //     //pause the narration until the player chooses to replay the game
        //     SceneManager.LoadScene((int)Scenes.ScenesList.MAIN_MENU);
        //     currentNarrationSequenceIndex = 0;
        //     narrationPaused = true;
        // }
    }

    //A function to unpause the narration
    public void ReportCompletion()
    {
        Debug.Log("Completion Reported");
        narrationPaused = false;
    }

    //*********************************
    //Visitor pattern process functions
    //*********************************

    public void ProcessLoadScene(LoadSceneSequence sequence)
    {
        //load the scene for the narration sequence
        SceneManager.LoadScene((int)sequence.scene);
    }

    public void ProcessDialogue(DialogueSequence sequence)
    {
        //Have dialogue manager start the dialogue
        Debug.Log(DialogueManager.Instance);
        DialogueManager.Instance.StartDialogue(sequence.dialogueToStart);

        //pause the dialogue until it finishes
        narrationPaused = true;
    }

    public void ProcessObjective(ObjectiveSequence sequence)
    {
        //Have the objective system activate the objective
        Debug.Log(sequence.objective);
        ObjectiveManager.Instance.Activate(sequence.objective);

        //pause the narration until the objective system reports completion
        narrationPaused = true;
    }

    public void ProcessDelay(DelaySequence sequence)
    {
        //pause the narration
        narrationPaused = true;

        //begin timer to unpause
        StartCoroutine(NarrationDelay(sequence.delayTime));
    }

    private IEnumerator NarrationDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        //unpause the narration after some amount of time
        narrationPaused = false;
    }

    private void OnDestroy()
    {
        //reset singleton instance to null 
        //when object is destroyed
        Instance = null;
    }
}
