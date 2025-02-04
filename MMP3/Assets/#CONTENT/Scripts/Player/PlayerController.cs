using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Alteruna;
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

    [SerializeField] private Synchronizable _synchronizable;
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
    [SerializeField] private GameObject _human;
    [SerializeField] public GameObject _carSeat;
    // [SerializeField] private Image _marker;
    // [SerializeField] private PrometeoCarController _carController;



    void Awake()
    {
        DontDestroyOnLoad(gameObject);

    }

    void Start()
    {
        _avatar = GetComponent<Alteruna.Avatar>();
        PlayerManager.Instance.AddPlayer(_avatar.gameObject);

        // _marker.enabled = false;

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

        Scene currentScene = SceneManager.GetActiveScene();
        bool isSceneOne = currentScene.buildIndex == 1;
        var players = PlayerManager.Instance._players;

        if (players.Count == 0)
            return;

        bool isPlayerOne = players.Count > 0 && players[0].GetComponent<Alteruna.Avatar>().IsMe;
        bool isPlayerTwo = players.Count > 1 && players[1].GetComponent<Alteruna.Avatar>().IsMe;

        if (isSceneOne)
        {
            if (isPlayerOne)
            {
                // Player 1 (ID 0) is the driver
                _car.SetActive(true);
                _human.SetActive(false);
                _playerCamera.transform.SetParent(_car.transform);
                // _marker.enabled = false;
                // _carController.CarMovement(); //External script to move the car
                Debug.Log("III: Player 1 is in the driver seat");
                MoveCar();

                //CAse if therse nly one player DEBUG
                if (players.Count == 1)
                    return;
                
                PlayerManager.Instance.ParentPlayerToCar(players[1], _carSeat);


            }
            else if (isPlayerTwo)
            {
                // Player 2 (ID 1) is the passenger
                _car.SetActive(false);
                _human.SetActive(true);
                Debug.Log("III: Player 2 is in the passenger seat");
                // _marker.enabled = true;

                // Move();

                if (Input.GetKeyDown(KeyCode.M))
                {
                    MiniMap.Instance.isMiniMapActive = !MiniMap.Instance.isMiniMapActive;

                    if (MiniMap.Instance.miniMapCanvas != null)
                        MiniMap.Instance.miniMapCanvas.SetActive(MiniMap.Instance.isMiniMapActive);

                    if (MiniMap.Instance.fullMapCanvas != null)
                        MiniMap.Instance.fullMapCanvas.SetActive(!MiniMap.Instance.isMiniMapActive);
                }
            }
        }
        else
        {
            // In other scenes, both players walk independently
            _car.SetActive(false);
            _human.SetActive(true);
            _playerCamera.transform.SetParent(transform);
            Move();
        }
    }

    private void Move()
    {
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
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

    private void MoveCar()
    {
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
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

    // //Corutine to Wait 5 seconds
    // IEnumerator Wait(int seconds)
    // {
    //     yield return new WaitForSeconds(seconds);
    // }

}