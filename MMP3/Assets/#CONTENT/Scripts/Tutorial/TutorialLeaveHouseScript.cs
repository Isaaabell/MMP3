using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialLeaveHouseScript : MonoBehaviour
{
    private bool playerInTrigger = false;
    [SerializeField] private GameObject player;
    void Start()
    {

    }
    void Update()
    {
        UIManagerTutorial uiManagerTutorial = FindObjectOfType<UIManagerTutorial>();

        if (uiManagerTutorial != null && uiManagerTutorial.allItemsStolen && playerInTrigger)
        {
            if (Input.GetKeyDown(KeyCode.E)) // Prüft, ob "F" gedrückt wurde
            {
                if (player != null)
                {
                    CharacterController controller = player.GetComponent<CharacterController>();
                    if (controller != null)
                    {
                        controller.enabled = false; // Deaktivieren, damit Position gesetzt werden kann
                    }

                    player.transform.position = new Vector3(-300f, 10f, 300f);
                    // player.transform.rotation = Quaternion.Euler(0f, 90f, 0f);

                    if (controller != null)
                    {
                        controller.enabled = true; // Wieder aktivieren
                    }
                    
                    player.GetComponent<TutorialScript>().isCar = true;
                }
                else
                {
                    Debug.LogError("iwas funkt nd");
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
        }
    }
}
