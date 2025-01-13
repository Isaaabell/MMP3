using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignMinimapCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Canvas playerCanvas;
    [SerializeField] private Camera minimapCamera;

    void Start()
    {
        // Automatically find the minimap camera if not assigned in the Inspector
        if (minimapCamera == null)
        {
            minimapCamera = GameObject.FindWithTag("MinimapCamera")?.GetComponent<Camera>();
        }

        if (playerCanvas != null && minimapCamera != null)
        {
            playerCanvas.worldCamera = minimapCamera;
        }
        else
        {
            Debug.LogWarning("MinimapCamera or PlayerCanvas not set or found!");
        }
    }
}

