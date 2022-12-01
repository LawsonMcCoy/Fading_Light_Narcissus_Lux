using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlUi : MonoBehaviour
{
    [SerializeField] private string[] controlUiLabels = new string[]
            {"Space Text", "Shift Text", "Right-Mouse Text"};
    //[SerializeField] private string controlUiParentName;

    private GameObject controlUiParent;
    private int controlUiElements;
    protected Text[] controlUiTexts;

    private string[] stringControls = new string[]
        {"Walk", "Hover", "Fly", "Dash", "Jump", "Sprint", "Attack", "---"};

    //an enum for the string version of each character mode 
    //they are used as int values that matches the index of stringControls
    //basically copying Lawson's tactics
    private enum Controls
    {
        WALKMODE,
        HOVERMODE,
        FLYMODE,
        DASHMODE,
        JUMPMODE,
        SPRINTMODE,
        ATTACKMODE,
        NOMODE
    }

    private void Awake()
    {
        controlUiParent = this.gameObject;//GameObject.Find(controlUiParentName);
        controlUiElements = controlUiLabels.Length;
        controlUiTexts = new Text[controlUiElements];

        for (int i = 0; i < controlUiElements; i++)
        {
            controlUiTexts[i] = controlUiParent.transform.Find(controlUiLabels[i]).gameObject.GetComponent<Text>();
        }
    }

    public void TransitionWalkUI()
    {
        controlUiTexts[0].text = stringControls[(int)Controls.JUMPMODE];    //Space
        controlUiTexts[1].text = stringControls[(int)Controls.SPRINTMODE];  //Shift
        controlUiTexts[2].text = stringControls[(int)Controls.DASHMODE];    //Right-click
    }

    public void TransitionMidairUI()
    {
        controlUiTexts[0].text = stringControls[(int)Controls.HOVERMODE];   //Space
        controlUiTexts[1].text = stringControls[(int)Controls.FLYMODE];     //Shift
        controlUiTexts[2].text = stringControls[(int)Controls.DASHMODE];    //Right-click
    }

    public void TransitionHoverUI()
    {
        controlUiTexts[0].text = stringControls[(int)Controls.WALKMODE];    //Space
        controlUiTexts[1].text = stringControls[(int)Controls.FLYMODE];     //Shift
        controlUiTexts[2].text = stringControls[(int)Controls.DASHMODE];    //Right-click
    }

    public void TransitionFlyUI()
    {
        controlUiTexts[0].text = stringControls[(int)Controls.WALKMODE];    //Space
        controlUiTexts[1].text = stringControls[(int)Controls.NOMODE];      //Shift
        controlUiTexts[2].text = stringControls[(int)Controls.DASHMODE];    //right-click
    }
}
