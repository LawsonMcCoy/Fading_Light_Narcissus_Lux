//Created by Marc Hagoriles
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{   
    #region Private Fields
    
    //Reference to the DialogueBoxUI and all its objects
    [SerializeField] private GameObject dialogueUI; //For the whole UI
    [SerializeField] private Text speakerName; //For the speaker name being displayed in the UI
    [SerializeField] private Text textField; //For the dialogue text being displayed in the UI

    //Reference to the lines of dialogue (ScriptableObject)
    ///
    /// Ideally, we want to have the objects within the scene to be "Interactable" that holds their own dialogues
    /// But for TESTING PURPOSES right now, there can only be one dialogue set within the inspector.
    ///
    [SerializeField] private Dialogue dialogue; 
    [SerializeField] private float typeSpeed; //Float that sets the speed in which the letters in the dialogue gets typed out.
    private int currSentenceIndex = 0; //Integer index that corresponds to the current sentence(s) in the dialogue.

    #endregion


    #region Singleton
    /// 
    /// DialogueManager should be a Singleton. This means that everything inside this function can
    /// only be called through the Instance in which DialogueManager is set. (There can only be one DialogueManager running)
    /// 
    public static DialogueManager Instance {get; private set;}

    private void OnDestroy()
    {
        //When the DialogueManager is destroyed, reset the Instance to null (No more DialogueManager running).
        Instance = null;
    }
    #endregion

    #region MonoBehaviour

    private void Awake()
    {
        // If the current Instance is NOT null (meaning there is a DialogueManager Instance active) AND it's not this one, then destroy it.
        if (Instance != null && Instance != this)
        {
            // Destroy(this);
        }
        else
        {
            //Set DialogueManager Instance to this.
            Instance = this;
        }

        //make sure the singleton exists accross scenes
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        //TESTING PURPOSES: Clicking on an object such as the capsule in the middle to start the dialogue.
        //Ideally, we want the Narration Manager to handle this, as certain Narration Sequences will have playing dialogue as a sequence.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit) && Input.GetMouseButtonDown(0)) //If raycast hit and left clicked
        {
            Transform clickableObject = hit.transform;
            if (clickableObject.CompareTag("Player")) //If the clickable object is a player (TESTING PURPOSES: the capsule is a player)
            {
                Debug.Log("Clicked on a clickable object!");
                if (!isSpeaking()) //If the dialoguebox is already present (meaning that someone is already speaking)
                {                  //Then don't re-do it.
                    showUI();
                    StartDialogue(dialogue);
                }
            }
        }
        
        //If the UI is showing (meaning isSpeaking is true), then that must mean that the dialogue is currently active.
        //Pressing space (TESTING PURPOSES) advances the dialogue.
        if (Input.GetKeyDown("space") && isSpeaking())
        {
            ContinueDialogue();
        }
    }
    private IEnumerator Type()
    {
        //For each letter in our sentences, 
        foreach (char letter in dialogue.sentences[currSentenceIndex].text.ToCharArray())
        {
            //Update the text field by adding each letter per iteration
            textField.text += letter;
            //And then wait for typeSpeed seconds before displaying the next letter.
            yield return new WaitForSeconds(typeSpeed);
        }
    }
    #endregion

    #region Helper Functions

    //This function checks to see if there is a dialogue currently active
    //(Just checks to see if the UI is on, meaning it is active)
    private bool isSpeaking()
    {
        return dialogueUI.activeSelf;
    }

    //This functions starts a certain dialogue passed as a parameter.
    //For now, dialogue only contains one ScriptableObject set in the inspector.
    //This function can be used to pass different dialogues and start them.
    public void StartDialogue(Dialogue dialogue)
    {
        Debug.Log($"Starting dialogue {dialogue.sentences[0].text}");
        speakerName.text = dialogue.sentences[currSentenceIndex].character.fullName;
        showUI(); //activating the dialogue UI
        Instance.StartCoroutine(Type());
    }

    //This functions returns true if the displayed text is equal to the entire sentence.
    private bool isTypedOut()
    {
        return textField.text == dialogue.sentences[currSentenceIndex].text ? true : false;
    }

    private void ContinueDialogue()
    {
            //If the entire sentence is already typed out, then advance to the next dialogue.
            if (isTypedOut())
            {
                //If our index is still in range with how many sentences we have...
                if (currSentenceIndex < dialogue.sentences.Length - 1)
                {
                    StopAllCoroutines(); //Failsafe
                    //Increment it to go to the next sentence
                    currSentenceIndex++;
                    textField.text = ""; //Update the text field so that it's blank again (since it's a new sentence)
                    speakerName.text = dialogue.sentences[currSentenceIndex].character.fullName; //Update the speaker
                    StartCoroutine(Type()); //Start the Coroutine of typing out the next sentence
                }
                else 
                {
                    hideUI(); //If we no longer have sentences, hide the UI
                    resetUI();
                    NarrationManager.Instance.ReportCompletion(); //report that the dialogue has ended
                }
            }
            else //If not, stop the typing and set the displayed text equal to the entire dialogue (to skip the typing)
            {
                StopAllCoroutines();
                textField.text = dialogue.sentences[currSentenceIndex].text;
            } 
    }

    //Displays the DialogueBox UI
    private void showUI()
    {
        dialogueUI.SetActive(true);
    }

    //Hides the DialogueBox UI
    private void hideUI()
    {
        dialogueUI.SetActive(false);
    }
    #endregion

    //Resets the DialogueBox UI, which means speaker name and the dialogue text will be blank
    //And the index for checking which sentence we are currently at will be set back to 0.
    private void resetUI()
    {
        StopAllCoroutines(); //Stop all ongoing Coroutines (this is a failsafe)
        speakerName.text = "";
        currSentenceIndex = 0;
        textField.text = "";
    }
}