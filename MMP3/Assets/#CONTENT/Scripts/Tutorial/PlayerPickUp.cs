using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUp : MonoBehaviour
{
    public float pickUpRange = 3f; // Die Reichweite, in der der Spieler den Würfel aufheben kann
    private GameObject currentCube = null; // Der aktuell in Reichweite befindliche Würfel

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.F) && currentCube != null)
        {
            PickUpCurrentCube();
        }
    }

    // Prüft, ob ein Würfel in Reichweite ist
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("PickUpCube")) // Der Würfel muss das Tag "PickUpCube" haben
        {
            currentCube = other.gameObject; // Der Würfel wird in die Variable gespeichert
        }
    }

    // Wenn der Würfel den Trigger verlässt, wird er gelöscht
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PickUpCube"))
        {
            currentCube = null; // Der Würfel ist nicht mehr in Reichweite
        }
    }

    // Funktion, um den Würfel aufzuheben
    private void PickUpCurrentCube()
    {
        // Hole das Skript, um die PickUp-Funktion auszuführen
        PickUpCube pickUpScript = currentCube.GetComponent<PickUpCube>();
        if (pickUpScript != null)
        {
            pickUpScript.PickUp(); // Würfel verschwinden lassen
        }

        currentCube = null; // Der Würfel ist jetzt nicht mehr in Reichweite
    }
}
