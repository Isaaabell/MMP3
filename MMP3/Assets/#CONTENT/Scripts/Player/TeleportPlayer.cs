using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportPlayer : MonoBehaviour
{

    [SerializeField] private List<GameObject> _players = new List<GameObject>();
    [SerializeField] private int _neededPlayers;
    private const int _CITYSCENEINDEX = 1;

    [Header("Timer")]
    [SerializeField] private float _countdownTime = 5f;
    private Coroutine _countdownCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_players.Contains(other.gameObject))
        {
            _players.Add(other.gameObject);
            // CheckAndLoadScene();
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
    }
}
