using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlUi : MonoBehaviour
{
    [SerializeField] private string[] controlUiLabels = new string[]
            {"Space Text", "Shift Text", "Right-Mouse Text"};
    [SerializeField] private string[] controlImagesLabels = new string[]
            {"Space Image", "Shift Image", "Right-Mouse Image"};
    [SerializeField] private float steps = 0.7f;
    [SerializeField] private float waitSeconds = 1f;

    // Text UI
    private GameObject controlUiParent;
    private int controlUiLength;
    private Text[] controlUiTexts;
    private string[] stringControls = new string[]
        {"Walk", "Hover", "Fly", "Dash", "Jump", "Sprint", "Attack", "---"};

    // Image UI
    private int controlImagesLength;
    private Image[] controlImages;
    bool isRunning = false;

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

    private enum Images
    {
        SPACEIMAGE,
        SHIFTIMAGE,
        RIGHTCLICK
    }

    private void Awake()
    {
        controlUiParent = this.gameObject;
        controlUiLength = controlUiLabels.Length;
        controlUiTexts = new Text[controlUiLength];

        for (int i = 0; i < controlUiLength; i++)
        {
            controlUiTexts[i] = controlUiParent.transform.Find(controlUiLabels[i]).gameObject.GetComponent<Text>();
        }

        controlImagesLength = controlImagesLabels.Length;
        controlImages = new Image[controlImagesLength];
        for (int i = 0; i < controlImagesLength; i++)
        {
            controlImages[i] = controlUiParent.transform.Find(controlImagesLabels[i]).gameObject.GetComponent<Image>();
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

    public void IndicateModeChange()
    {
        // prevent player from permanently changing the image sizes by spamming transitions 
        if (!isRunning)
        {
            isRunning = true;
            // When a transition occurs, inform there is a change by having all key images "pulse"
            foreach (Image image in controlImages)
            {
                Vector2 rectSize = image.rectTransform.sizeDelta;
                StartCoroutine(Pulse(image, rectSize));
            }
        }
    }

    private IEnumerator Pulse(Image image, Vector2 imageRectSize)
    {
        float bound = 0;
        Vector2 rectSize = image.rectTransform.sizeDelta;
        // Increase size
        while (bound <= waitSeconds)
        {
            bound += Time.fixedDeltaTime;
            image.rectTransform.sizeDelta = new Vector2(rectSize.x + steps, rectSize.y + steps);
            rectSize = image.rectTransform.sizeDelta;

            yield return new WaitForEndOfFrame();
        }

        // Decrease size to original size
        while (imageRectSize.x < rectSize.x || imageRectSize.y < rectSize.y)
        {
            bound -= Time.fixedDeltaTime;
            image.rectTransform.sizeDelta = new Vector2(rectSize.x - steps, rectSize.y - steps);
            rectSize = image.rectTransform.sizeDelta;
            yield return new WaitForEndOfFrame();
        }
        isRunning = false;
    }
}
