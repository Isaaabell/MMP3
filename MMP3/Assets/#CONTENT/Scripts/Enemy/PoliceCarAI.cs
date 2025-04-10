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

    [Header("Patrol Distance Settings")]
    public float minPatrolDistance = 20f;
    public float maxPatrolDistance = 40f;

    [Header("Movement Settings")]
    public float patrolSpeed = 8f;
    public float pursuitSpeed = 12f;
    public float rotationSpeed = 2f;
    public float waypointThreshold = 2f;

    private NavMeshAgent agent;
    private Transform player;
    private Vector3 currentPatrolDestination;
    private float timeSinceLastSawPlayer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        agent.speed = patrolSpeed;
        agent.angularSpeed = 360f;
        agent.acceleration = 8f;

        SetRandomPatrolDestination();
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
        if (agent.remainingDistance < waypointThreshold)
        {
            SetRandomPatrolDestination();
        }

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
                    return;
                }
            }
        }

        Debug.LogWarning("Konnte kein gültiges Ziel im geforderten Distanzbereich finden.");
    }

    void PursuitBehavior()
    {
        agent.SetDestination(player.position);
        timeSinceLastSawPlayer = 0f;

        if (Vector3.Distance(transform.position, player.position) > detectionRadius * 1.2f)
        {
            timeSinceLastSawPlayer += Time.deltaTime;

            if (timeSinceLastSawPlayer > 3f)
            {
                currentState = PoliceState.Returning;
                agent.speed = patrolSpeed;
                SetRandomPatrolDestination();
            }
        }
    }

    void ReturnBehavior()
    {
        if (agent.remainingDistance < waypointThreshold)
        {
            currentState = PoliceState.Patrol;
            agent.speed = patrolSpeed;
        }
    }

    void CheckForPlayer()
    {
        if (Vector3.Distance(transform.position, player.position) <= detectionRadius)
        {
            currentState = PoliceState.Pursuit;
            agent.speed = pursuitSpeed;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (currentState == PoliceState.Patrol)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, currentPatrolDestination);
        }
    }
}
