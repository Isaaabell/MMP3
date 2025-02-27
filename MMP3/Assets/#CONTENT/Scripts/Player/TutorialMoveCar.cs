using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMoveCar : MonoBehaviour
{
    // Start is called before the first frame update
    public float moveSpeed = 3.0f; // Bewegungsgeschwindigkeit

    void Start()
    {

    }

    public void MoveCar()
    {
        // Bewegung des Autos
        float moveDirectionX = Input.GetAxis("Horizontal");
        float moveDirectionZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveDirectionX + transform.forward * moveDirectionZ;

        // Bewegung anwenden
        transform.Translate((move * moveSpeed ) * Time.deltaTime);
    }
}
