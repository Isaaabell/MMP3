using UnityEngine;
using UnityEngine.AI;

public class PoliceCarAI : MonoBehaviour
{
    public enum PoliceState
    {
        Patrol,
        Pursuit,
        Returning
    }

    [Header("AI Settings")]
    public PoliceState currentState = PoliceState.Patrol;
    public float detectionRadius = 15f;

    [Header("Patrol Settings")]
    public float minPatrolDistance = 20f;
    public float maxPatrolDistance = 40f;
    public float patrolTimeout = 30f; // Neue Variable für Timeout

    [Header("Movement Settings")]
    public float patrolSpeed = 8f;
    public float pursuitSpeed = 12f;
    public float rotationSpeed = 2f;
    public float waypointThreshold = 2f;

    private NavMeshAgent agent;
    private Transform player;
    private Vector3 currentPatrolDestination;
    private float timeSinceLastSawPlayer;
    private float timeOnCurrentDestination; // Timer für aktuelles Ziel

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        agent.speed = patrolSpeed;
        agent.angularSpeed = 360f;
        agent.acceleration = 8f;

        SetRandomPatrolDestination();
        timeOnCurrentDestination = 0f; // Timer zurücksetzen
    }

    void Update()
    {
        switch (currentState)
        {
            case PoliceState.Patrol:
                PatrolBehavior();
                CheckForPlayer();
                break;

            case PoliceState.Pursuit:
                PursuitBehavior();
                break;

            case PoliceState.Returning:
                ReturnBehavior();
                break;
        }
    }

    void PatrolBehavior()
    {
        // Timeout-Check
        timeOnCurrentDestination += Time.deltaTime;
        if (timeOnCurrentDestination >= patrolTimeout)
        {
            Debug.Log("Patrol timeout - selecting new destination");
            SetRandomPatrolDestination();
            timeOnCurrentDestination = 0f; // Timer zurücksetzen
        }

        // Normale Wegpunkt-Logik
        if (agent.remainingDistance < waypointThreshold)
        {
            SetRandomPatrolDestination();
            timeOnCurrentDestination = 0f; // Timer zurücksetzen
        }

        // Rotation
        if (agent.velocity.magnitude > 0.1f)
        {
            Vector3 moveDirection = agent.velocity.normalized;
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    void SetRandomPatrolDestination()
    {
        int maxTries = 20;
        for (int i = 0; i < maxTries; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * maxPatrolDistance;
            randomDirection += transform.position;
            randomDirection.y = transform.position.y;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, maxPatrolDistance, NavMesh.AllAreas))
            {
                float distance = Vector3.Distance(transform.position, hit.position);
                if (distance >= minPatrolDistance && distance <= maxPatrolDistance)
                {
                    currentPatrolDestination = hit.position;
                    agent.SetDestination(currentPatrolDestination);
                    timeOnCurrentDestination = 0f; // Timer zurücksetzen
                    return;
                }
            }
        }

        Debug.LogWarning("Could not find valid destination in required distance range.");
    }

    // Rest des Skripts bleibt unverändert...
    void PursuitBehavior() { /* ... */ }
    void ReturnBehavior() { /* ... */ }
    void CheckForPlayer() { /* ... */ }
    void OnDrawGizmosSelected() { /* ... */ }
}