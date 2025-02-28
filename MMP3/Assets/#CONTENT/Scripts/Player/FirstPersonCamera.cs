using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FirstPersonCamera : MonoBehaviour
{
    public Transform Target;
    public float MouseSensitivity = 10f;

    private float verticalRotation;
    private float horizontalRotation;
    private Transform originalTarget; // Track the original camera target

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        // Start a coroutine to wait for the Target to be assigned
        StartCoroutine(WaitForTarget());
    }

    private IEnumerator WaitForTarget()
    {
        // Wait until the Target is assigned
        while (Target == null)
        {
            yield return null; // Wait for the next frame
        }

        // Store the original target once it's assigned
        originalTarget = Target;
        Debug.Log("Original camera target set to: " + originalTarget.name);
    }

    void LateUpdate()
    {
        if (Target == null)
        {
            return;
        }

        // Check if the scene has changed
        if (SceneManager.GetActiveScene().buildIndex != 1)
        {
            // Reset the camera target to the original target
            if (Target != originalTarget)
            {
                Target = originalTarget;
                Debug.Log("Camera target reset to original target: " + originalTarget.name);
            }
        }
        else if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            // Handle camera target for Player 2 in scene 1
            if (Target.GetComponent<PlayerMovement>().playerIndex == 2)
            {
                var player1 = FindPlayer1();
                if (player1 != null)
                {
                    Target = player1.transform;
                    Debug.Log("Camera target set to Player 1 for Player 2");
                }
            }
        }

        // Update camera position and rotation
        transform.position = Target.position;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        verticalRotation -= mouseY * MouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -70f, 70f);

        horizontalRotation += mouseX * MouseSensitivity;

        transform.rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0);
    }

    private PlayerMovement FindPlayer1()
    {
        foreach (var player in FindObjectsOfType<PlayerMovement>())
        {
            if (player.playerIndex == 1)
            {
                return player;
            }
        }
        return null;
    }
}