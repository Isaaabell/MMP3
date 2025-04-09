using UnityEngine;

public class KameraVerfolgung : MonoBehaviour
{
    [SerializeField] private Transform player; // Referenz zum Spieler
    [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 2f, -5f); // Abstand der Kamera
    [SerializeField] private float smoothSpeed = 5f; // Glättungsfaktor

    void LateUpdate()
    {
        if (player == null)
        {
            Debug.LogWarning("Kein Player zugewiesen!");
            return;
        }

        // Zielposition berechnen (Spielerposition + Offset)
        Vector3 desiredPosition = player.position + cameraOffset;
        
        // Sanfte Bewegung zur Zielposition
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
        
        // Kamera auf Spieler ausrichten
        transform.LookAt(player);
    }
}