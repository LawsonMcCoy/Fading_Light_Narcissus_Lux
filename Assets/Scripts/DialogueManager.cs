//Created by Marc Hagoriles
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{   
    #region Private Fields
    [SerializeField] private string[] dialogueSentences;
    [SerializeField] private Text textField;
    [SerializeField] private float typeSpeed;
    private int textIndex;
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
            Destroy(this);
        }
        else
        {
            //Set DialogueManager Instance to this.
            Instance = this;
        }
    }
    private void Start() 
    {
        //TESTING PURPOSES: As soon as the game starts, the dialogue should pop up.
        StartCoroutine(Type());
    }

    private void Update()
    {
        //TESTING PURPOSES: The way we continue our dialogue is by clicking left mouse button FOR NOW.
        if (Input.GetMouseButtonDown(0))
        {
            ContinueDialogue();
        }
    }
    private IEnumerator Type()
    {
        //For each letter in our sentences, 
        foreach (char letter in dialogueSentences[textIndex].ToCharArray())
        {
            //Update the text field by adding each letter per iteration
            textField.text += letter;
            //And then wait for typeSpeed seconds before displaying the next letter.
            yield return new WaitForSeconds(typeSpeed);
        }
    }
    #endregion

    #region Helper Functions
    private void ContinueDialogue()
    {
            //If our index is still in range with how many sentences we have...
            if (textIndex < dialogueSentences.Length - 1)
            {
                //Increment it to go to the next sentence
                textIndex++;
                textField.text = ""; //Update the text field so that it's blank again (since it's a new sentence)
                StartCoroutine(Type()); //Start the Coroutine of typing out the next sentence
            }
            else { textField.text = ""; } //TESTING PURPOSES: For now, the text field goes blank.
    }
    #endregion
}