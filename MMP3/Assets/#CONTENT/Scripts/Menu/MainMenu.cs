using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{

    [SerializeField] private CinemachineVirtualCamera mainMenuCam;
    [SerializeField] private CinemachineVirtualCamera optionsCam;

    void Awake()
    {
        mainMenuCam.Priority = 10;
        optionsCam.Priority = 0;
    }

    public void TutorialBtn()
    {
        SceneManager.LoadSceneAsync("Tutorial");  //richtigen namen der scene eintragen
    }
    public void PlayBtn()
    {
        SceneManager.LoadSceneAsync("Game");
    }

    public void OptionsBtn()
    {
        mainMenuCam.Priority = 0;
        optionsCam.Priority = 10;
    }
    public void BackBtn()
    {
        mainMenuCam.Priority = 10;
        optionsCam.Priority = 0;
    }
    public void QuitBtn()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
