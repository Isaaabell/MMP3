using UnityEngine;
using UnityEngine.Splines;

public class PoliceCarAI : MonoBehaviour
{
    public enum PoliceState
    {
        Patrol,
        Pursuit
    }

    [Header("State Settings")]
    public PoliceState currentState = PoliceState.Patrol;
    public float detectionRadius = 10f;
    public float timeToReturnToPatrol = 5f;

    [Header("Patrol Settings")]
    public SplineContainer patrolSpline;
    public float patrolSpeed = 5f;
    public float rotationSpeed = 90f;
    private float splineProgress = 0f;

    [Header("Pursuit Settings")]
    public float pursuitSpeed = 8f;

    private Transform player;
    private float distanceToPlayer;
    private float timePlayerOutOfRange = 0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        // Sicherstellen, dass ein Spline zugewiesen ist
        if (patrolSpline == null)
        {
            Debug.LogError("No patrol spline assigned!");
            enabled = false;
        }
    }

    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case PoliceState.Patrol:
                PatrolState();
                CheckForPlayer();
                break;

            case PoliceState.Pursuit:
                PursuitState();
                CheckIfPlayerLost();
                break;
        }
    }

    void PatrolState()
    {
        if (patrolSpline == null) return;

        // Spline-Fortschritt aktualisieren
        splineProgress += patrolSpeed * Time.deltaTime / patrolSpline.Spline.GetLength();
        
        // Loop beim Ende des Splines
        if (splineProgress > 1f)
        {
            splineProgress = 0f;
        }

        // Position und Rotation vom Spline lesen
        Vector3 position = patrolSpline.EvaluatePosition(splineProgress);
        Vector3 tangent = patrolSpline.EvaluateTangent(splineProgress);
        Vector3 upVector = patrolSpline.EvaluateUpVector(splineProgress);

        // Auto positionieren und ausrichten
        transform.position = position;
        
        // Nur rotieren wenn Bewegung vorhanden
        if (tangent != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(tangent, upVector);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void PursuitState()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * pursuitSpeed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void CheckForPlayer()
    {
        if (distanceToPlayer <= detectionRadius)
        {
            currentState = PoliceState.Pursuit;
            timePlayerOutOfRange = 0f;
        }
    }

    void CheckIfPlayerLost()
    {
        if (distanceToPlayer > detectionRadius)
        {
            timePlayerOutOfRange += Time.deltaTime;
            if (timePlayerOutOfRange >= timeToReturnToPatrol)
            {
                currentState = PoliceState.Patrol;
            }
        }
        else
        {
            timePlayerOutOfRange = 0f;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw detection radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}