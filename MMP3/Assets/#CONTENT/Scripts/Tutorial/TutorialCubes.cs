using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCubes : MonoBehaviour
{
    private Renderer cubeRenderer;
    [HideInInspector]public bool isTriggered = false;
    void Start()
    {
        // Initialisiere den Renderer
        cubeRenderer = GetComponent<Renderer>();
        if(cubeRenderer == null)
        {
            Debug.LogError("CubeRenderer not found");
        }
        cubeRenderer.material.color = Color.red; 
    }
    public void Update()
    {
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            cubeRenderer.material.color = Color.green;
            isTriggered = true;
        }
    }
}
