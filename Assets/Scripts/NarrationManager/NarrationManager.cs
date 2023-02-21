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

    [SerializeField] private float loadSceneDelay; //The amount of time the narration is delayed during
                                                   //a load scene sequence to prevent race conditions

    private bool narrationPaused; //a boolean that says wheter or not the narration is currently paused
    [SerializeField] private int currentNarrationSequenceIndex; //An integer representing the index of the current 
                                               //narration sequence
    [SerializeField] private int savedNarrationSequenceIndex; //An integer representing the index of the narration 
                                             //sequence that was last saved, Note this value will
                                             //made to be persistent later
    [SerializeField] private SceneSpawnContainer spawnPoints;
    private int currentSpawnPoint;
    private Scenes.ScenesList currentScene;   //the current scene

    private ObjectiveSequence currentObjectiveActive; //Member field that holds the Current Objective Sequence active.

    //Property that is able to return the Current Objective Sequence active, or set it.
    public ObjectiveSequence CurrentObjectiveActive
    {
        get
        {
            return currentObjectiveActive;
        }
        set
        {
            currentObjectiveActive = value;
        }
    }

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

    private void start()
    {
        //set listener for player's death
        EventManager.Instance.Subscribe(EventTypes.Events.PLAYER_DEATH, playerDeath);
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
        currentScene = sequence.scene;
        SceneManager.LoadScene((int)sequence.scene);

        //Post load processing, this is done in a coroutine
        //so we can wait full the scene to be fully loaded 
        //before the post load processing. Since it is done
        //in a coroutine, the narration will be paused until
        //the post load processing is completed
        narrationPaused = true;
        StartCoroutine(PostLoadSceneProcessing(sequence));
    }

    public void ProcessDialogue(DialogueSequence dialogueSequence)
    {
        //Have dialogue manager start the dialogue
        Debug.Log(DialogueManager.Instance);
        DialogueManager.Instance.StartDialogue(dialogueSequence);

        //pause the dialogue until it finishes
        narrationPaused = true;
    }

    public void ProcessObjective(ObjectiveSequence sequence)
    {
        //Have the objective system activate the objective
        sequence.activateObjective.Invoke();
        //Set the Current Objective Active property to be the Objective Sequence to be processed.
        //This is for the Quest UI.
        CurrentObjectiveActive = sequence;
        //pause the narration until the objective system reports completion

        narrationPaused = true;
    }

    public void ProcessDelay(DelaySequence sequence)
    {
        //Delay the narration by the apporiate amount of time
        StartCoroutine(NarrationDelay(sequence.delayTime));
    }

    public void ProcessSave(SaveSequence sequence)
    {
        Debug.Log(sequence);

        sequence.saveSuccesful = true; //save was successful(in correct scene)
        currentSpawnPoint = sequence.spawnIndex;
        spawnPoints = GameObject.FindGameObjectWithTag("Respawn").GetComponent<SceneSpawnContainer>();
        EventManager.Instance.Notify(EventTypes.Events.SAVE);

        //set saveindex to current index
        savedNarrationSequenceIndex = currentNarrationSequenceIndex + 1;
    }

    public void playerDeath()
    {
        //cancel narrations past the last save
        while(currentNarrationSequenceIndex > savedNarrationSequenceIndex)
        {
            narration[currentNarrationSequenceIndex].Cancel();
            currentNarrationSequenceIndex--;    //make current index to previous one
                                                //goes on until current = save
        }

        narrationPaused = false;    //make sure narration can continue

        
    }

    private IEnumerator NarrationDelay(float delayTime)
    {
        //pause the narration
        narrationPaused = true;

        yield return new WaitForSeconds(delayTime);

        EventManager.Instance.Notify(EventTypes.Events.LOAD_SCENE);
        Debug.Log("scene loaded");
        //unpause the narration after some amount of time
        narrationPaused = false;
    }

    //A function to perform post load processing on a scene
    //This function will be a seperate coroutine so that it can 
    //wait until the scene is fully loaded 
    private IEnumerator PostLoadSceneProcessing(LoadSceneSequence sequence)
    {
        //wait for scene to be fully loaded
        yield return new WaitForSeconds(loadSceneDelay);

        //if the scene is a game scene, make sure to save the game
        if (sequence.isGameScene)
        {
            ProcessSave(sequence);
        }
        EventManager.Instance.Notify(EventTypes.Events.LOAD_SCENE);
        //Once the post load processing is completed
        //unpause the narration
        narrationPaused = false;
    }
    public Vector3 getSpawn()
    {
        //returns the position of current spawn point
        if (spawnPoints != null)
        {
            return spawnPoints.getSpawn(currentSpawnPoint);
        }
        return Vector3.zero;
    }

    private void OnDestroy()
    {
        //reset singleton instance to null 
        //when object is destroyed
        Instance = null;

        //unsubscribe from listener
        EventManager.Instance.Unsubscribe(EventTypes.Events.PLAYER_DEATH, playerDeath);
    }
}
