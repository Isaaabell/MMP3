using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class TeleportPlayer : MonoBehaviour
{
    [SerializeField] private List<GameObject> _players = new List<GameObject>();
    [SerializeField] private int _neededPlayers;
    private const int _CITYSCENEINDEX = 2;
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

        // Store player data BEFORE scene load
        var playerData = new List<(int index, string controlScheme, InputDevice[] devices)>();

        foreach (var player in _players)
        {
            var input = player.GetComponent<PlayerInput>();
            playerData.Add((input.playerIndex, input.currentControlScheme, input.devices.ToArray()));
        }

        // Destroy old players
        foreach (var player in _players)
        {
            Destroy(player);
        }

        _players.Clear();

        SceneManager.sceneLoaded += OnSceneLoaded;

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            for (int i = 0; i < playerData.Count; i++)
            {
                var data = playerData[i];

                GameObject prefab = data.index == 0 ? _playerPrefab1 : _playerPrefab2;

                var newPlayer = PlayerInput.Instantiate(
                    prefab,
                    data.index,
                    controlScheme: data.controlScheme,
                    pairWithDevices: data.devices);

                newPlayer.transform.position = new Vector3(0, 1, 0);
            }

            SceneManager.sceneLoaded -= OnSceneLoaded; // Clean up the event subscription
        }

        SceneManager.LoadSceneAsync(_CITYSCENEINDEX);
    }

}
