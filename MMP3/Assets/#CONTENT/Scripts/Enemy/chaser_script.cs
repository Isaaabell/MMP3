using UnityEngine;
using UnityEngine.AI;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] public float speed = 5f; // Geschwindigkeit des folgenden Objekts
    [SerializeField] public float stoppingDistance = 1f; // Abstand zum Spieler
    [SerializeField] public float avoidanceRadius = 2f; //
    private Transform player;
    private NavMeshAgent agent;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        agent = GetComponent<NavMeshAgent>();

        if (player == null)
        {
            Debug.LogError("Kein Objekt mit Tag 'Player' gefunden!");
        }
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent nicht gefunden!");
        }
        else
        {
            agent.speed = speed;
            agent.stoppingDistance = stoppingDistance;
            agent.updateRotation = false; // Deaktiviere die automatische Rotation des Agents
            agent.avoidancePriority = Random.Range(30, 70); // Zufällige Priorität zur Vermeidung von Staus
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            agent.avoidancePriority = Random.Range(30, 70);
        }
    }

    void Update()
    {
        if (player != null && agent != null)
        {
            agent.SetDestination(player.position);
            
            // Vermeidung von Kollisionen mit anderen Chasern
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, avoidanceRadius);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Chaser") && hitCollider.transform != transform)
                {
                    Vector3 awayDirection = transform.position - hitCollider.transform.position;
                    agent.SetDestination(transform.position + awayDirection.normalized * avoidanceRadius);
                }
            }
            
            // Manuelle Rotation für realistische Drehung wie ein Auto
            if (agent.velocity.sqrMagnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }
    }
}
