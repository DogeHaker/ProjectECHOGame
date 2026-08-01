using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
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

    [Header("Audio & Spatial Footsteps")]
    public AudioSource enemyAudioSource; // AudioSource with 3D Spatial Blend enabled
    public AudioClip footstepSFX;
    public float patrolStepInterval = 0.8f; // Pace while patrolling
    public float chaseStepInterval = 0.4f;  // Faster pace while chasing
    private float footstepTimer;

    private Transform playerTransform;
    private PlayerInventoryV2 playerInventory;
    private NavMeshAgent agent; // Stores our pathfinding brain link

    void Start()
    {
        // Cache our pathfinding component
        agent = GetComponent<NavMeshAgent>();

        // Auto-fetch AudioSource if not manually assigned in Inspector
        if (enemyAudioSource == null)
        {
            enemyAudioSource = GetComponent<AudioSource>();
        }

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
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.StopChaseMusic();
                }
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
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.StartChaseMusic();
                }
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

        // Tell pathfinding brain where to walk
        agent.SetDestination(targetWaypoint.position);

        // Play 3D footsteps at patrol cadence
        HandleFootsteps(patrolStepInterval);

        // Check if agent reached waypoint target node destination
        if (!agent.pathPending && agent.remainingDistance < waypointThreshold)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    void Chase()
    {
        // Constantly recalculate shortest route along blue mesh floor to catch player
        agent.SetDestination(playerTransform.position);

        // Play 3D footsteps at faster chase cadence
        HandleFootsteps(chaseStepInterval);
    }

    void HandleFootsteps(float stepInterval)
    {
        // Only trigger steps if the enemy is actively moving on the NavMesh
        if (agent != null && agent.velocity.sqrMagnitude > 0.1f)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                if (enemyAudioSource != null && footstepSFX != null)
                {
                    enemyAudioSource.PlayOneShot(footstepSFX);
                }
                footstepTimer = stepInterval;
            }
        }
        else
        {
            // Ready up timer so step plays immediately when movement starts
            footstepTimer = 0f;
        }
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

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayJumpscare();
        }

        // Kill momentum and disable physics brain to prevent sliding
        if (agent != null)
        {
            agent.velocity = Vector3.zero; // Instantly drops speed to absolute 0
            agent.isStopped = true;        // Stops path navigation
            agent.enabled = false;         // Disables component so it cannot slide or drift
        }

        // 1. Permanently freeze player inputs
        if (playerMovement != null) playerMovement.enabled = false;
        if (mouseLookScript != null) mouseLookScript.enabled = false;

        // 2. Force player camera to snap directly to enemy face target
        if (playerCamera != null && enemyFaceTarget != null)
        {
            playerCamera.LookAt(enemyFaceTarget.position);
        }

        // Trigger black death screen after 2.5 seconds
        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.TriggerGameOverSequence(2.5f);
        }

        // 3. Display technical system failure notification
        NotificationUI notifier = FindObjectOfType<NotificationUI>();
        if (notifier != null)
        {
            notifier.DisplayMessage("SYSTEM FAILURE: SECURITY UNIT INTERCEPTED AGENT.\n[Press Escape to Quit]", 999f);
        }
    }
}