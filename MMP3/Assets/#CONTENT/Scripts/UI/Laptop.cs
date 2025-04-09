using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laptop : MonoBehaviour
{
    [SerializeField] private CanvasGroup screenCanvasGroup;
    [SerializeField] private WinManager winManager;
    public void ToggleCanvasGroup()
    {
        if (screenCanvasGroup == null) return;

        // Switch on screen
        screenCanvasGroup.alpha = 1;
        screenCanvasGroup.interactable = true;
        screenCanvasGroup.blocksRaycasts = true;

        winManager.EnableInput();
    }
}
