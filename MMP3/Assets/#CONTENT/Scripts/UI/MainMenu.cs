using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
public class MainMenu : MonoBehaviour
{

    [SerializeField] private CinemachineVirtualCamera mainMenuCam;
    [SerializeField] private CinemachineVirtualCamera optionsCam;

    [SerializeField] private GameObject _loadingScreen;
    private const int _LEVEL1SCENEINDEX = 1;

    void Awake()
    {
        mainMenuCam.Priority = 10;
        optionsCam.Priority = 0;
    }

    public void PlayBtn()
    {
        // SceneManager.LoadSceneAsync(_LEVEL1SCENEINDEX); //TODO: Corect scene name if needed
        StartCoroutine(LoadLevelWithDelay());
    }

    private IEnumerator LoadLevelWithDelay()
    {
        _loadingScreen.SetActive(true);
        yield return new WaitForSeconds(2f); // Simulate loading delay
        SceneManager.LoadScene(_LEVEL1SCENEINDEX);

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
