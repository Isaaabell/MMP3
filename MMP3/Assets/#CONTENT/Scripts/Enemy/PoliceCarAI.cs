using UnityEngine;
using UnityEngine.Splines;

public class PoliceCarAI : MonoBehaviour
{
    public enum PoliceState
    {
        Patrol,
        Pursuit,
        ReturnToPatrol
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
    public float returnSpeed = 6f;

    private Transform player;
    private float distanceToPlayer;
    private float timePlayerOutOfRange = 0f;
    private Vector3 nearestSplinePoint;
    private float returnStartTime;
    private Vector3 returnStartPosition;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
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

            case PoliceState.ReturnToPatrol:
                ReturnToPatrolState();
                break;
        }
    }

    void PatrolState()
    {
        if (patrolSpline == null) return;

        splineProgress += patrolSpeed * Time.deltaTime / patrolSpline.Spline.GetLength();
        
        if (splineProgress > 1f)
        {
            splineProgress = 0f;
        }

        Vector3 position = patrolSpline.EvaluatePosition(splineProgress);
        Vector3 tangent = patrolSpline.EvaluateTangent(splineProgress);
        Vector3 upVector = patrolSpline.EvaluateUpVector(splineProgress);

        transform.position = position;
        
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

        // Aktualisiere den nächstgelegenen Punkt auf dem Spline während der Verfolgung
        FindNearestPointOnSpline();
    }

    void ReturnToPatrolState()
    {
        if (patrolSpline == null) return;

        // Berechne die Fortschrittsrate basierend auf der Entfernung
        float distanceToTarget = Vector3.Distance(transform.position, nearestSplinePoint);
        float progress = Mathf.Clamp01((Time.time - returnStartTime) * returnSpeed / distanceToTarget);

        // Sanfte Bewegung zum nächstgelegenen Punkt auf dem Spline
        transform.position = Vector3.Lerp(returnStartPosition, nearestSplinePoint, progress);

        // Rotation in Bewegungsrichtung
        Vector3 direction = (nearestSplinePoint - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Wenn wir nahe genug am Spline sind, zurück zum Patrol-Modus
        if (distanceToTarget < 0.5f)
        {
            currentState = PoliceState.Patrol;
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
                PrepareReturnToPatrol();
            }
        }
        else
        {
            timePlayerOutOfRange = 0f;
        }
    }

    void PrepareReturnToPatrol()
    {
        FindNearestPointOnSpline();
        returnStartTime = Time.time;
        returnStartPosition = transform.position;
        currentState = PoliceState.ReturnToPatrol;
    }

    void FindNearestPointOnSpline()
    {
        if (patrolSpline == null) return;

        float closestDistance = float.MaxValue;
        float stepSize = 0.01f;

        // Durchsuche den Spline nach dem nächstgelegenen Punkt
        for (float t = 0; t <= 1f; t += stepSize)
        {
            Vector3 point = patrolSpline.EvaluatePosition(t);
            float distance = Vector3.Distance(transform.position, point);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestSplinePoint = point;
                splineProgress = t;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        
        if (currentState == PoliceState.ReturnToPatrol)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, nearestSplinePoint);
            Gizmos.DrawSphere(nearestSplinePoint, 0.5f);
        }
    }
}