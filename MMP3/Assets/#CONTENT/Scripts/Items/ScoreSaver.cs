using System;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public bool loadScoreOnSceneChange = true;
    
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
        
        // Load score on startup
        LoadScore();
    }
    
    private void OnEnable()
    {
        // Subscribe to scene loading event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDisable()
    {
        // Unsubscribe from scene loading event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (loadScoreOnSceneChange)
        {
            // Add a small delay to ensure everything is set up
            Invoke("LoadScore", 0.1f);
        }
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
    
    public void NewGame()
{
    // Reset score in MoneyManager
    if (moneyManager != null)
    {
        moneyManager.SetTotalMoney(0);
    }
    
    // Create a fresh score data
    ScoreData newGameData = new ScoreData
    {
        score = 0,
        timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
    };
    
    // Save to JSON file to overwrite any existing data
    FileHandler.SaveToJSON(newGameData, jsonFilename);
    lastSavedScore = 0;
    
    Debug.Log("New game started: Score reset to 0");
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

        Debug.Log($"Score saved: {currentScore} at {Application.persistentDataPath}/{jsonFilename}");
    }
    
    // Load score from JSON
    public void LoadScore()
    {
        ScoreData data = FileHandler.ReadFromJSON<ScoreData>(jsonFilename);
        
        if (data != null && moneyManager != null)
        {
            // Set the loaded score in the money manager
            moneyManager.SetTotalMoney(data.score);
            lastSavedScore = data.score;
            
            Debug.Log($"Score loaded: {data.score} from {data.timestamp}");
        }
        else
        {
            Debug.Log("No saved score file found or money manager not available");
        }
    }
    
    // Public method to force a save
    public void ForceSave()
    {
        SaveScore();
    }
    
    // Public method to force a load
    public void ForceLoad()
    {
        LoadScore();
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