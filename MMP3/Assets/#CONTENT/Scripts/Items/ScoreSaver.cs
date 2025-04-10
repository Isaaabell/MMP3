using System;
using UnityEngine;

[Serializable]
public class ScoreData
{
    public int score;
    public string timestamp;
}

public class ScoreSaver : MonoBehaviour
{
    [Header("Save Settings")]
    public string jsonFilename = "player_score.json";
    public bool autoSaveOnChange = true;
    public float autoSaveInterval = 15f; // Backup save every 15 seconds
    
    private MoneyManager moneyManager;
    private int lastSavedScore = -1;
    private float nextAutoSaveTime;
    
    private void Awake()
    {
        moneyManager = GetComponent<MoneyManager>();
        
        if (moneyManager == null)
        {
            moneyManager = FindObjectOfType<MoneyManager>();
            
            if (moneyManager == null)
            {
                Debug.LogError("No MoneyManager found! ScoreSaver requires a MoneyManager to function.");
                enabled = false;
                return;
            }
        }
        
        // Initialize next save time
        nextAutoSaveTime = Time.time + autoSaveInterval;
        
        // Load initial score if available
        LoadScore();
    }
    
    private void Update()
    {
        // Check if we need to auto-save based on interval
        if (Time.time >= nextAutoSaveTime)
        {
            SaveScore();
            nextAutoSaveTime = Time.time + autoSaveInterval;
        }
        
        // If money has changed since last save and auto-save is enabled
        if (autoSaveOnChange && moneyManager.GetTotalMoney() != lastSavedScore)
        {
            SaveScore();
        }
    }
    
    // Save score to JSON
    public void SaveScore()
    {
        // Get current score from money manager
        int currentScore = moneyManager.GetTotalMoney();
        
        // Create data object
        ScoreData data = new ScoreData
        {
            score = currentScore,
            timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };
        
        // Save to JSON file
        FileHandler.SaveToJSON(data, jsonFilename);
        
        // Update last saved score
        lastSavedScore = currentScore;
    }
    
    // Load score from JSON
    private void LoadScore()
    {
        ScoreData data = FileHandler.ReadFromJSON<ScoreData>(jsonFilename);
        
        if (data != null)
        {
            lastSavedScore = data.score;
            
            // Log that we found saved data
            Debug.Log($"Loaded saved score: {lastSavedScore} from {data.timestamp}");
        }
    }
    
    // Public method to force a save
    public void ForceSave()
    {
        SaveScore();
    }
    
    // Save when application quits
    private void OnApplicationQuit()
    {
        SaveScore();
    }
    
    // Save when application pauses (mobile)
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveScore();
        }
    }
}