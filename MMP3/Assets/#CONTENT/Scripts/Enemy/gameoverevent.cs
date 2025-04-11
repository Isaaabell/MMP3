using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverTrigger : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Radius in dem der Player erkannt wird")]
    public float detectionRadius = 5f;
    
    [Tooltip("Name der GameOver-Szene")]
    public string gameOverSceneName = "GameOver";

    [Tooltip("Mindestdauer im Radius für Game Over (Sekunden)")]
    public float requiredTimeInRadius = 5f;

    private Transform player;
    private float timeInRadius;
    private bool isPlayerInRadius;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player == null)
        {
            Debug.LogError("Kein GameObject mit Tag 'Player' gefunden!");
        }
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool currentlyInRadius = distanceToPlayer <= detectionRadius;

        // Countdown-Logik
        if (currentlyInRadius)
        {
            if (!isPlayerInRadius)
            {
                // Player gerade erst in den Radius gekommen
                isPlayerInRadius = true;
                timeInRadius = 0f;
            }
            else
            {
                // Player bleibt im Radius
                timeInRadius += Time.deltaTime;
                
                if (timeInRadius >= requiredTimeInRadius)
                {
                    TriggerGameOver();
                }
            }
        }
        else
        {
            // Player hat den Radius verlassen
            isPlayerInRadius = false;
            timeInRadius = 0f;
        }
    }

    void TriggerGameOver()
    {
        Debug.Log($"Player war {requiredTimeInRadius} Sekunden im Radius - Game Over!");
        SceneManager.LoadScene(gameOverSceneName);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = isPlayerInRadius ? Color.red : new Color(1, 0, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}