using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartFadingOutAnim : MonoBehaviour
{
    [SerializeField] private Animation anim;
    //[SerializeField] private AnimationClip fade;

    public void OnStartButtonClick()
    {
        anim.Play("FadeOut");
    }

    public void OnFadeComplete()
    {
        GameManager.Instance.StartGame();
    }
}
