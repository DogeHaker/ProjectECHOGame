using UnityEngine;
using UnityEngine.AI; // CRITICAL: This gives us access to Unity's AI Pathfinding!

[RequireComponent(typeof(NavMeshAgent))] // Automatically adds the component if missing
public class EnemyAI : MonoBehaviour
{
    public enum AIState { Patrolling, Chasing, Jumpscaring }
    [Header("AI State")]
    public AIState currentState = AIState.Patrolling;

    [Header("Movement Speeds")]
    public float patrolSpeed = 2.0f;
    public float chaseSpeed = 3.2f;

    [Header("Patrol Grid")]
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;
    public float waypointThreshold = 1.0f; // Increased slightly for smooth NavMesh node registration

    [Header("Detection Metrics")]
    public float detectionRadius = 8f;
    public float jumpscareRadius = 1.5f;

    [Header("Jumpscare Setup")]
    public Transform playerCamera;
    public Transform enemyFaceTarget;
    public PlayerMovement playerMovement;
    public MouseMovement mouseLookScript;

    private Transform playerTransform;
    private PlayerInventoryV2 playerInventory;
    private NavMeshAgent agent; // Stores our new pathfinding brain link

    void Start()
    {
        // Cache our pathfinding component
        agent = GetComponent<NavMeshAgent>();

        PlayerInventoryV2 playerScript = FindObjectOfType<PlayerInventoryV2>();
        if (playerScript != null)
        {
            playerTransform = playerScript.transform;
            playerInventory = playerScript;
        }

        // Initialize baseline speeds
        if (agent != null) agent.speed = patrolSpeed;
    }

    void Update()
    {
        if (playerTransform == null || agent == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        switch (currentState)
        {
            case AIState.Patrolling:
                Patrol();

                if (distanceToPlayer <= detectionRadius && !IsPlayerHidden())
                {
                    // Line of Sight (LOS) Raycast check
                    Vector3 eyeOrigin = transform.position + Vector3.up * 1f;
                    Vector3 playerTarget = playerTransform.position + Vector3.up * 1f;
                    Vector3 directionToPlayer = (playerTarget - eyeOrigin).normalized;

                    RaycastHit hit;
                    if (Physics.Raycast(eyeOrigin, directionToPlayer, out hit, detectionRadius))
                    {
                        if (hit.transform == playerTransform)
                        {
                            currentState = AIState.Chasing;
                            agent.speed = chaseSpeed; // Boost pathfinding speed!
                            Debug.LogWarning("ANDROID SENSORS TRIGGERED: Target acquired.");
                        }
                    }
                }
                break;

            case AIState.Chasing:
                Chase();

                if (IsPlayerHidden())
                {
                    currentState = AIState.Patrolling;
                    agent.speed = patrolSpeed; // Return to standard sweep speed
                    Debug.Log("ANDROID LOST TARGET: Returning to systemic sweep loops.");
                }
                else if (distanceToPlayer <= jumpscareRadius)
                {
                    currentState = AIState.Jumpscaring;
                    TriggerJumpscareSequence();
                }
                break;

            case AIState.Jumpscaring:
                // Stop the agent dead in its tracks during the scare frame
                break;
        }
    }

    void Patrol()
    {
        if (waypoints.Length == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];

        // Instead of manual positioning math, we just tell the pathfinding brain where to walk!
        agent.SetDestination(targetWaypoint.position);

        // Check if the agent has reached the waypoint target node destination
        if (!agent.pathPending && agent.remainingDistance < waypointThreshold)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    void Chase()
    {
        // Constantly recalculate the shortest route along the blue mesh floor to catch the player
        agent.SetDestination(playerTransform.position);
    }

    bool IsPlayerHidden()
    {
        if (playerInventory != null)
        {
            return playerInventory.isHidden;
        }
        return false;
    }

    void TriggerJumpscareSequence()
    {
        Debug.LogWarning("💥 JUMPSCARE INITIALIZED.");

        // THE FIX: Completely kill the Android's momentum and turn off its physics brain
        if (agent != null)
        {
            agent.velocity = Vector3.zero; // Instantly drops speed to absolute 0
            agent.isStopped = true;        // Stops the path navigation
            agent.enabled = false;         // Disables the component so it cannot slide or drift
        }

        // 1. Permanently freeze the player's inputs so they can't run away or look away
        if (playerMovement != null) playerMovement.enabled = false;
        if (mouseLookScript != null) mouseLookScript.enabled = false;

        // 2. Force the Player's camera to instantly snap and look directly at the enemy's face target
        if (playerCamera != null && enemyFaceTarget != null)
        {
            playerCamera.LookAt(enemyFaceTarget.position);
        }

        // Triggers the black death screen after 2.5 seconds
        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.TriggerGameOverSequence(2.5f);
        }

        // 3. Display the technical system failure notification
        NotificationUI notifier = FindObjectOfType<NotificationUI>();
        if (notifier != null)
        {
            notifier.DisplayMessage("SYSTEM FAILURE: SECURITY UNIT INTERCEPTED AGENT.\n[Press Escape to Quit]", 999f);
        }
    }
}