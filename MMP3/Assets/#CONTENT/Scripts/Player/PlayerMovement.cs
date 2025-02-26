using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : NetworkBehaviour
{
    private Vector3 _velocity;
    private bool _jumpPressed;
    public Camera Camera;

    private CharacterController _controller;

    public float PlayerSpeed = 2f;
    public float JumpForce = 5f;
    public float GravityValue = -9.81f;

    [Networked] private int playerIndex { get; set; } // Tracks player index
    [Networked] private bool isDriver { get; set; } // Tracks if player is the driver

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            _jumpPressed = true;
        }

        if (Input.GetKeyDown(KeyCode.F) && HasStateAuthority) // Ensure only Player 1 initiates scene switch
        {
            TrySwitchScene();
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (playerIndex == 2) 
            {
                Debug.Log("Player 1 is the driver");
                return; // Prevent movement for non-drivers
            }
        }

        if (_controller.isGrounded)
        {
            _velocity = new Vector3(0, -1, 0);
        }

        Quaternion cameraRotationY = Quaternion.Euler(0, Camera.transform.rotation.eulerAngles.y, 0);
        Vector3 move = cameraRotationY * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Runner.DeltaTime * PlayerSpeed;

        _velocity.y += GravityValue * Runner.DeltaTime;
        if (_jumpPressed && _controller.isGrounded)
        {
            _velocity.y += JumpForce;
        }
        _controller.Move(move + _velocity * Runner.DeltaTime);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }

        _jumpPressed = false;
    }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            Camera = Camera.main;
            Camera.GetComponent<FirstPersonCamera>().Target = transform;
        }

        playerIndex = Runner.LocalPlayer.PlayerId; // Assign player index
        Debug.Log("Player index: " + playerIndex);
        foreach (var player in Runner.ActivePlayers)
        {
            Debug.Log("Player in lobby: " + player.PlayerId);
        }
    }

    void TrySwitchScene()
    {
        Runner.LoadScene(SceneRef.FromIndex(1));
        Debug.Log("Switching scene");
        foreach (var player in Runner.ActivePlayers)
        {
            Debug.Log("Player in lobby: " + player.PlayerId);
        }
    }
}
