using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questText;

    private void Update()
    {
        //If there is no Objective active (meaning, the current Narration Sequence is a Dialogue/Delay/LoadScene Sequence...)
        //Then have the text display blank. If there is an Objective active, then display the Description of that Objective.
        if (NarrationManager.Instance.CurrentObjectiveActive != null)
        {
            updateQuestText();
        }
        else
        {
            resetQuestText();
        }
    }

    //The purpose of this function is to update the Quest Text on the Quest UI.
    private void updateQuestText()
    {
        questText.text = NarrationManager.Instance.CurrentObjectiveActive.Description.ToString();
    }

    //The purpose of this function is to reset the Quest UI to blank.
    private void resetQuestText()
    {
        questText.text = "";
    }
}
