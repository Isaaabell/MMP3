using System.Linq;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : NetworkBehaviour
{
    private Vector3 _velocity;
    private bool _jumpPressed;
    public Camera playerCamera;

    private CharacterController _controller;
    private MeshRenderer _meshRenderer;

    public float PlayerSpeed = 2f;
    public float JumpForce = 5f;
    public float GravityValue = -9.81f;


    [Networked] public int playerIndex { get; set; } // Tracks player index
    [Networked, OnChangedRender(nameof(DeactivePlayer))] private bool isPassenger { get; set; } = true;// Deactivate player if not driver

    private Item _carriedItem = null;


    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _meshRenderer = GetComponent<MeshRenderer>();
        DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            _jumpPressed = true;
        }

        if (Input.GetKeyDown(KeyCode.Q) && HasStateAuthority) // Ensure only Player 1 initiates scene switch
        {
            TrySwitchScene();
        }

        if (Input.GetKeyDown(KeyCode.E)) // Ensure only Player 1 initiates scene switch
        {
            Debug.Log("I still live");
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("F key was pressed");
            if (_carriedItem == null)
            {
                TryPickUpItem();
            }
            else
            {
                DropItem();
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        // Reset isPassenger when not in scene 1
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (playerIndex == 2)
            {
                Debug.Log("Player 1 is the driver");
                isPassenger = false; // Deactivate Player 2 in scene 1
                Debug.Log("Player 2 is the passenger");
                return; // Prevent movement for non-drivers
            }
        }
        else if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            isPassenger = true;
        }

        if (_controller.isGrounded)
        {
            _velocity = new Vector3(0, -1, 0);
        }

        Quaternion cameraRotationY = Quaternion.Euler(0, playerCamera.transform.rotation.eulerAngles.y, 0);
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
            playerCamera = Camera.main;
            playerCamera.GetComponent<FirstPersonCamera>().Target = transform;
        }

        playerIndex = Runner.LocalPlayer.PlayerId; // Assign player index
        Debug.Log("Player index: " + playerIndex);
        Debug.Log("Player spawned");
    }

    void TrySwitchScene()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            Runner.LoadScene(SceneRef.FromIndex(1));
            Debug.Log("Switching scene");
            foreach (var player in Runner.ActivePlayers)
            {
                Debug.Log("Player in lobby: " + player.PlayerId);
            }
        }
        else if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            Runner.LoadScene(SceneRef.FromIndex(2));
        }

    }

    void DeactivePlayer()
    {
        _controller.enabled = isPassenger;
        _meshRenderer.enabled = isPassenger;
    }

    void TryPickUpItem()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 2f))
        {
            Item item = hit.transform.GetComponent<Item>();
            if (item != null && !item.IsCarried)
            {
                item.PickUp2(this);
                _carriedItem = item;
            }
        }
    }

    void DropItem()
    {
        if (_carriedItem != null)
        {
            _carriedItem.Drop2();
            _carriedItem = null;
        }
    }
}
