using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 3.0f;
    public float gravity = -9.81f;
    private float footstepTimer = 0f;
    public float stepInterval = 0.5f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private CharacterController controller;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        // Check if player is pressing WASD keys
        if (moveX != 0f || moveZ != 0f)
        {
            footstepTimer += Time.deltaTime;
            if (footstepTimer >= stepInterval)
            {
                footstepTimer = 0f;
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayRandomFootstep();
                }
            }
        }
        // 1. Reset downward velocity when grounded so gravity doesn't build up infinitely
        if (isGrounded() && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 2. Horizontal WASD Movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // 3. Apply Continuous Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    bool isGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}