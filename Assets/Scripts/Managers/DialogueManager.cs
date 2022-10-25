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
    [SerializeField] private RectTransform dialoguePanel; //For the panel
    [SerializeField] private Text speakerName; //For the speaker name being displayed in the UI
    [SerializeField] private Text textField; //For the dialogue text being displayed in the UI

    //Reference to the dialogue within the UI.
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private float typeSpeed;
    private int textIndex = 0;

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

    private void Start()
    {

    }
    private void Awake()
    {
        // If the current Instance is NOT null (meaning there is a DialogueManager Instance active) AND it's not this one, then destroy it.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            //Set DialogueManager Instance to this.
            Instance = this;
        }
    }

    private void Update()
    {
        //TESTING PURPOSES: Clicking on an object such as the capsule in the middle to advance the dialogue.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit) && Input.GetMouseButtonDown(0)) //If raycast hit and left clicked
        {
            Transform clickableObject = hit.transform;
            if (clickableObject.CompareTag("Player"))
            {
                Debug.Log("Clicked on a clickable object!");
                showUI();
                StartDialogue();
            }
        }

        if (Input.GetKeyDown("space") && isSpeaking() == true)
        {
            ContinueDialogue();
        }
    }
    private IEnumerator Type()
    {
        //For each letter in our sentences, 
        foreach (char letter in dialogue.sentences[textIndex].text.ToCharArray())
        {
            //Update the text field by adding each letter per iteration
            textField.text += letter;
            //And then wait for typeSpeed seconds before displaying the next letter.
            yield return new WaitForSeconds(typeSpeed);
        }
    }
    #endregion

    #region Helper Functions

    private bool isSpeaking()
    {
        return dialogueUI.activeSelf ? true : false;
    }

    private void StartDialogue()
    {
        speakerName.text = dialogue.sentences[textIndex].character.fullName;
        StartCoroutine(Type());
    }

    private void ContinueDialogue()
    {
            //If our index is still in range with how many sentences we have...
            if (textIndex < dialogue.sentences.Length - 1)
            {
                //Increment it to go to the next sentence
                textIndex++;
                textField.text = ""; //Update the text field so that it's blank again (since it's a new sentence)
                speakerName.text = dialogue.sentences[textIndex].character.fullName; //Update the speaker
                StartCoroutine(Type()); //Start the Coroutine of typing out the next sentence
            }
            else 
            {
                hideUI(); //If we no longer have sentences, hide the UI
                textField.text = "";
                textIndex = 0; //Reset the index to 0.
                speakerName.text = " ";
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
}