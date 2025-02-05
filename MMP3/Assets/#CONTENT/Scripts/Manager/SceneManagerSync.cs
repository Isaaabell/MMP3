using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Alteruna;
using System;
using UnityEngine.UI;



public class SceneManagerSync : AttributesSync
{
    private const int _STREETSCENEINDEX = 1;
    private const int _HOUSESCENEINDEX = 2;

    [Header("Canvas Overlay")]
    public Image sceneSwitchOverlay;
    public float fadeDuration = 1.0f;
    private CanvasGroup canvasGroup;

    void Start()
    {
        if (sceneSwitchOverlay != null)
        {
            canvasGroup = sceneSwitchOverlay.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
        }
    }
    void Update()
    {
    }

    [SynchronizableMethod]
    private void SyncLoadStreetScene()
    {
        StartCoroutine(SceneTransition(_STREETSCENEINDEX));
    }

    [SynchronizableMethod]
    private void SyncLoadHouseScene()
    {
        StartCoroutine(SceneTransition(_HOUSESCENEINDEX));
        // SceneManager.LoadSceneAsync(_HOUSESCENEINDEX);
    }

    private IEnumerator SceneTransition(int sceneIndex)
    {
        yield return StartCoroutine(FadeCanvas(1f));
        yield return new WaitForSeconds(1f);
        SceneManager.LoadSceneAsync(sceneIndex);
    }

    private IEnumerator FadeCanvas(float targetAlpha)
    {
        if (canvasGroup == null) yield break;

        float startAlpha = canvasGroup.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }


}
