using UnityEngine;

public class TutorialEndScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Stelle sicher, dass dein Auto das Tag "Player" hat!
        {
            Debug.Log("Spiel beendet!");

            // Im Editor den Play-Modus stoppen
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            // Im Build das Spiel beenden
            Application.Quit();
            #endif
        }
    }
}
