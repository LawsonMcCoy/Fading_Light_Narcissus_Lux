//Script created by Marc Hagoriles
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    #region Helper Functions
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene"); //Loads up GameScene.
    }

    public void QuitGame()
    {
        Application.Quit(); //This should terminate the game.
    }
    #endregion
}
