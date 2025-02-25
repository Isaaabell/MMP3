using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
  public void PlayTutorial()
    {
        SceneManager.LoadSceneAsync("Tutorial");  //richtigen namen der scene eintragen
    }
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("Game");
    }
    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
