using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManagerTutorial : MonoBehaviour
{
    private bool firstDialogue = false;
    private bool secondDialogue = false;
    [SerializeField] private List<GameObject> movementMarkers;
    [SerializeField] private List<GameObject> levelTwoObjects;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject tutorialCanvas;
    [SerializeField] private GameObject firstDialogueCanvas;
    [SerializeField] private GameObject secondDialogueCanvas;
    private bool allTriggeredLogged = false; //damit nur einmal getriggered wird wenn alle true sind
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

        foreach (var objects in levelTwoObjects)
        {
            Item item = objects.GetComponent<Item>();

            

            if (item == null || !item.tutorialboolSmallItem && !item.tutorialboolBigItem)
            {
                allPickedup = false;
                break; // Sofort abbrechen, wenn einer nicht getriggert ist
            }
        }

        if (allPickedup && !allTriggeredLogged) // Prüfen, ob wir schon geloggt haben
        {
            Debug.Log("Alle dinge gestohlen");//mach was wenn alles aufgesammelt ist
            allTriggeredLogged = true; // Verhindert weiteres Debugging
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
                break; // Sofort abbrechen, wenn einer nicht getriggert ist
            }
        }

        if (allTriggered && !allTriggeredLogged) // Prüfen, ob wir schon geloggt haben
        {
            TutorialStageTwo();
            allTriggeredLogged = true; // Verhindert weiteres Debugging
        }
    }

    private void TutorialStageTwo()
    {
        if (player != null)
        {
            player.transform.position = new Vector3(13f, 0.5f, 0f);
            Debug.Log("Spieler wurde in den zweiten Raum teleportiert!");
        }
        else
        {
            Debug.LogError("Spieler-GameObject ist nicht zugewiesen!");
        }

        // Canvas aktivieren
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
