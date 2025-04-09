using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playermovement_Prototype : MonoBehaviour
{
    public float bewegungsGeschwindigkeit = 5f;
    private Rigidbody rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        Vector3 bewegung = new Vector3(horizontal, 0f, vertical) * bewegungsGeschwindigkeit;
        rb.velocity = bewegung;
        
        if (bewegung != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(bewegung);
        }
    }
}
