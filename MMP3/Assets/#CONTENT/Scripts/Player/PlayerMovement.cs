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

    [Networked] private int playerIndex { get; set; }


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

        if (Input.GetKeyDown(KeyCode.F))
        {
            TrySwitchScene();
        }
    }

    public override void FixedUpdateNetwork()
    {
        // FixedUpdateNetwork is only executed on the StateAuthority
        if (playerIndex == 1)
        {
            return;
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
        playerIndex = Runner.SessionInfo.PlayerCount - 1; // Assign player index

        Debug.Log("Player index: " + playerIndex);

        if (playerIndex == 0)
        {
            gameObject.transform.position = new Vector3(0, 0, 0);
        }
        else
        {
            gameObject.transform.position = new Vector3(0, 1, 0);
        }

    }

    void TrySwitchScene()
    {
        Runner.LoadScene(SceneRef.FromIndex(1));
        Debug.Log("Switching scene");
    }
}