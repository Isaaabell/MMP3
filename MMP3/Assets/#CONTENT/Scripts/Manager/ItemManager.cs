using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Alteruna;
using System;

[Serializable]
public class ItemManager : AttributesSync
{
    public static ItemManager Instance { get; private set; }

    [Header("Money")]
    [SerializeField] private TextMeshProUGUI _itemDisplayTxt;
    [SynchronizableField] public float _totalItemAmount = 0;

    [Header("Timer")]
    [SerializeField] private float timerStart;
    public TMP_Text timerText;
    [SynchronizableField] private float currentTime;
    private bool isRunning = true;


    [Header("Das gehört eig in ein eigenes Script")]
    public CanvasGroup _winScreen;
    public CanvasGroup _loseScreen;
    public TMP_Text _endScoreTxt;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        _itemDisplayTxt.text = "0€";
        currentTime = timerStart;
        StartCoroutine(Countdown());
    }

    public void AddItem(float amount)
    {
        BroadcastRemoteMethod("SyncAddItem", amount);
    }

    public void WinGame()
    {
        BroadcastRemoteMethod("SyncWinGame");
    }

    IEnumerator Countdown()
    {
        while (currentTime > 0 && isRunning)
        {
            UpdateTimerUI();
            BroadcastRemoteMethod("SyncUpdateTimer", currentTime); // Sync the timer
            yield return new WaitForSeconds(1f);
            currentTime--;
        }

        if (currentTime <= 0)
        {
            TimerEnd();
        }
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void TimerEnd()
    {
        Debug.Log("Timer expired!");
        isRunning = false;
        timerText.text = "00:00";
        _loseScreen.alpha = 1;
    }

    [SynchronizableMethod]
    private void SyncAddItem(float amount)
    {
        _totalItemAmount += amount;
        Debug.Log("Item collected! Total value: " + _totalItemAmount);
        _itemDisplayTxt.text = _totalItemAmount.ToString("0.00") + "€";
        _endScoreTxt.text = _totalItemAmount.ToString("0.00") + "€";
    }

    [SynchronizableMethod]
    private void SyncWinGame()
    {
        _winScreen.alpha = 1;
    }

    [SynchronizableMethod]
    private void SyncUpdateTimer(float syncedTime)
    {
        currentTime = syncedTime;
        UpdateTimerUI();
    }
}
