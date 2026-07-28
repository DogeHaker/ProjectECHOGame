using UnityEngine;

public class ViewBobbing : MonoBehaviour
{
    [Header("Idle Breathing")]
    public float idleSpeed = 1.5f;
    public float idleXAmount = 0.01f;
    public float idleYAmount = 0.02f;

    [Header("Walking Wobble")]
    public float walkSpeed = 10f;
    public float walkXAmount = 0.04f;
    public float walkYAmount = 0.04f;

    private Vector3 originalLocalPosition;
    private float timer = 0f;
    private CharacterController playerController;

    void Start()
    {
        originalLocalPosition = transform.localPosition;

        // Find the player's movement controller box safely
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<CharacterController>();
        }
    }

    void Update()
    {
        // 1. Calculate how fast the player is physically moving on the ground
        float playerSpeed = 0f;
        if (playerController != null)
        {
            // Vector3 velocity flattening Y (jumping doesn't trigger horizontal bobbing)
            Vector3 horizontalVelocity = new Vector3(playerController.velocity.x, 0, playerController.velocity.z);
            playerSpeed = horizontalVelocity.magnitude;
        }

        // 2. Advance the mathematical rhythm clock timer
        timer += Time.deltaTime;

        Vector3 targetPosition = originalLocalPosition;

        // 3. State Machine: Is the player walking or standing still?
        if (playerSpeed > 0.1f)
        {
            // Player is walking -> Dynamic horror camera sway loop
            float waveX = Mathf.Cos(timer * walkSpeed) * walkXAmount;
            float waveY = Mathf.Sin(timer * walkSpeed * 2f) * walkYAmount;

            targetPosition.x += waveX;
            targetPosition.y += waveY;
        }
        else
        {
            // Player is idle -> Slow, heavy breath movement loop
            float waveX = Mathf.Cos(timer * idleSpeed) * idleXAmount;
            float waveY = Mathf.Sin(timer * idleSpeed * 2f) * idleYAmount;

            targetPosition.x += waveX;
            targetPosition.y += waveY;
        }

        // 4. Smoothly shift the hand anchor container to its calculated vector coordinates
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * 8f);
    }
}