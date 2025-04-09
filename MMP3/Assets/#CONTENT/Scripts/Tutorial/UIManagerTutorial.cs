using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManagerTutorial : MonoBehaviour
{
    private bool firstDialogue = false;
    // private bool secondDialogue = false;
    [SerializeField] private List<GameObject> movementMarkers;
    [SerializeField] private List<GameObject> levelTwoObjects;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject tutorialCanvas;
    [SerializeField] private GameObject firstDialogueCanvas;
    [SerializeField] private GameObject secondDialogueCanvas;
    private bool allTriggeredLogged = false;
    public bool allItemsStolen = false;

    [SerializeField] private GameObject Level2SpawnPoint;
    // [SerializeField] private GameObject Level3SpawnPoint;
    public void Start()
    {

    }
    public void Update()
    {
        if(firstDialogue)
        {
            CheckList();
            CheckListTwo();
        }
    }

    private void CheckListTwo()
    {
        bool allPickedup = true;
        bool allTriggeredLogged = false;

        foreach (var objects in levelTwoObjects)
        {
            Item1 item = objects.GetComponent<Item1>();
            if (item == null || !item.tutorialboolSmallItem)
            {
                allPickedup = false;
                break; 
            }
        }

        if (allPickedup && !allTriggeredLogged) 
        {
            Debug.Log("Alle dinge gestohlen");
            allItemsStolen = true;//bool to allowe leave house
            allTriggeredLogged = true;
        }
    }

    private void CheckList()
    {
        bool allTriggered = true;

        foreach (var marker in movementMarkers)
        {
            TutorialCubes trigger = marker.GetComponent<TutorialCubes>();

            if (trigger == null || !trigger.isTriggered)
            {
                allTriggered = false;
                break; 
            }
        }

        if (allTriggered && !allTriggeredLogged) 
        {
            TutorialStageTwo();
            allTriggeredLogged = true; 
        }
    }

    private void TutorialStageTwo()
    {
        if (player != null)
        {
            // player.transform.position = new Vector3(13f, 0.5f, 0f);
            player.transform.position = Level2SpawnPoint.transform.position;
            Debug.Log("Spieler wurde in den zweiten Raum teleportiert!");
        }
        else
        {
            Debug.LogError("Spieler-GameObject ist nicht zugewiesen!");
        }

        
        if (tutorialCanvas != null)
        {
            tutorialCanvas.SetActive(true);
            firstDialogueCanvas.SetActive(false);
            secondDialogueCanvas.SetActive(true);
        }
    }
    public void FinishFirstDialogue()
    {
        firstDialogue = true;
    }
}
