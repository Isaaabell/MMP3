using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerController : MonoBehaviour
{
    [Header("Base setup")]
    public float walkingSpeed;
    public float initialWalkingSpeed = 7.5f;
    public float slowedDownWalkingSpeed = 1f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    CharacterController _characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    [SerializeField]
    private float cameraYOffset = 0.4f;
    private Camera _playerCamera;

    private Alteruna.Avatar _avatar;
    [SerializeField] private ItemLogicOnPlayer _itemLogicOnPlayer;

    [SerializeField] private GameObject _car;
    [SerializeField] private GameObject _player;



    void Awake()
    {
        DontDestroyOnLoad(gameObject);

    }

    void Start()
    {
        _avatar = GetComponent<Alteruna.Avatar>();
        
        PlayerManager.Instance.AddPlayer(_avatar.gameObject);

        if (!_avatar.IsMe)
            return;

        _characterController = GetComponent<CharacterController>();
        _playerCamera = Camera.main;
        _playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y + cameraYOffset, transform.position.z);
        _playerCamera.transform.SetParent(transform);

        // // Lock cursor
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    void Update()
    {
        if (!_avatar.IsMe)
            return;

        bool isRunning = false;

        Scene currentScene = SceneManager.GetActiveScene();

        //if curent scene is 1 and self is player with ID 1
        if (currentScene.buildIndex == 1 && PlayerManager.Instance._players[0].GetComponent<Alteruna.Avatar>().IsMe)
        {
            _car.SetActive(true);
            _player.SetActive(false);
            _playerCamera.transform.SetParent(_car.transform);
            Debug.Log("III: Player is in the car");
        }
        else
        {
            _car.SetActive(false);
            _player.SetActive(true);

            _playerCamera.transform.SetParent(transform);



            // Press Left Shift to run
            isRunning = Input.GetKey(KeyCode.LeftShift);

            // We are grounded, so recalculate move direction based on axis
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);

            walkingSpeed = _itemLogicOnPlayer._isBigItemCollected ? slowedDownWalkingSpeed : initialWalkingSpeed;

            float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
            float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
            float movementDirectionY = moveDirection.y;
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);

            if (Input.GetButton("Jump") && canMove && _characterController.isGrounded)
            {
                moveDirection.y = jumpSpeed;
            }
            else
            {
                moveDirection.y = movementDirectionY;
            }

            if (!_characterController.isGrounded)
            {
                moveDirection.y -= gravity * Time.deltaTime;
            }

            // Move the controller
            _characterController.Move(moveDirection * Time.deltaTime);

            // Player and Camera rotation
            if (canMove && _playerCamera != null)
            {
                rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
                rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
                _playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
                transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
            }
        }


    }
}