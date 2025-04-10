using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class TeleportPlayer : MonoBehaviour
{
    [SerializeField] private List<GameObject> _players = new List<GameObject>();
    [SerializeField] private int _neededPlayers;
    private const int _CITYSCENEINDEX = 1;
    public GameObject _playerPrefab1;
    public GameObject _playerPrefab2;

    [Header("Timer")]
    [SerializeField] private float _countdownTime = 5f;
    private Coroutine _countdownCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_players.Contains(other.gameObject))
        {
            _players.Add(other.gameObject);
            TryStartCountdown();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && _players.Contains(other.gameObject))
        {
            _players.Remove(other.gameObject);
            CancelCountdown();
        }
    }

    private void TryStartCountdown()
    {
        if (_players.Count == _neededPlayers && _countdownCoroutine == null)
        {
            _countdownCoroutine = StartCoroutine(CountdownToTeleport());
        }
    }

    private void CancelCountdown()
    {
        if (_countdownCoroutine != null)
        {
            StopCoroutine(_countdownCoroutine);
            _countdownCoroutine = null;
            Debug.Log("Player left the trigger, countdown cancelled. :)");
        }
    }

    private IEnumerator CountdownToTeleport()
    {
        float timer = _countdownTime;

        Debug.Log($"Countdown started: {timer} seconds");

        while (timer > 0)
        {
            // If players leave during countdown, abort
            if (_players.Count != _neededPlayers)
            {
                Debug.Log("Player left during countdown. Resetting...");
                _countdownCoroutine = null;
                yield break;
            }

            Debug.Log($"Teleporting in {timer:F1} seconds...");
            yield return new WaitForSeconds(1f);
            timer -= 1f;
        }

        Debug.Log("Teleporting now!");
        SceneManager.LoadSceneAsync(_CITYSCENEINDEX);
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            // Loop through the players and instantiate their respective prefabs
            foreach (var player in _players)
            {
                // Get the PlayerInput component
                var playerInput = player.GetComponent<PlayerInput>();

                // Log the player index for debugging purposes
                Debug.Log($"Player {playerInput.playerIndex} is being teleported!");

                // Determine the correct prefab based on playerIndex
                GameObject playerPrefab = playerInput.playerIndex == 0 ? _playerPrefab1 : _playerPrefab2;

                // Log the prefab chosen for instantiation
                Debug.Log($"Spawning prefab for Player {playerInput.playerIndex}: {(playerInput.playerIndex == 0 ? "Player1" : "Player2")}");

                // Instantiate the appropriate player prefab using PlayerInput
                var newPlayer = PlayerInput.Instantiate(
                    playerPrefab,
                    playerInput.playerIndex,
                    controlScheme: playerInput.currentControlScheme,
                    pairWithDevices: playerInput.devices.ToArray());

                // OPTIONAL: move the new player to the start position (if needed)
                newPlayer.transform.position = new Vector3(0, 1, 0);

                // Destroy the old player model
                Destroy(player);
            }
        };
    }
}
