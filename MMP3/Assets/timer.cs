using System.Collections;
using UnityEngine;
using TMPro;

public class scr : MonoBehaviour
{
    [SerializeField] private float timerStart = 300f; // 5 Minuten in Sekunden
    private TMP_Text timerText; // UI-Text für die Anzeige
    private float currentTime;
    private bool isRunning = true;

    void Start()
    {
        timerText = GetComponent<TMP_Text>(); // Weist automatisch das TMP-Text-Element zu
        currentTime = timerStart;
        StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        while (currentTime > 0 && isRunning)
        {
            UpdateTimerUI();
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
        Debug.Log("Timer abgelaufen!");
        isRunning = false;
        timerText.text = "00:00";
    }
}