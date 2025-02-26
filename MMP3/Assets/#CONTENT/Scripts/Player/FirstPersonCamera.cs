using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;


public class FirstPersonCamera : MonoBehaviour
{
    public Transform Target;
    public float MouseSensitivity = 10f;

    private float verticalRotation;
    private float horizontalRotation;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    void LateUpdate()
    {
        if (Target == null)
        {
            return;
        }

        if (Target.GetComponent<PlayerMovement>().playerIndex == 2 && SceneManager.GetActiveScene().buildIndex == 1)
        {
            var player1 = FindPlayer1();
            if (player1 != null)
            {
                Target = player1.transform;
                Debug.Log("Camera target set to Player 1 for Player 2");
            }
        }

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