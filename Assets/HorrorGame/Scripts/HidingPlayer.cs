using UnityEngine;
using System.Collections; // Required for Coroutines/IEnumerator!

public class HidingPlayer : MonoBehaviour
{
    private bool playerIsInside = false;
    private bool canExitLocker = false; // Guard rail to stop same-frame double triggers

    [Header("Positions")]
    public Transform insidePosition;  // Empty object inside the locker
    public Transform outsidePosition; // Empty object outside the locker door

    private GameObject playerCapsule;
    private PlayerInventoryV2 playerInv;
    private PlayerMovement playerMovement;
    private MouseMovement mouseLookScript;

    void Start()
    {
        playerInv = FindObjectOfType<PlayerInventoryV2>();
        if (playerInv != null)
        {
            playerCapsule = playerInv.gameObject;
            playerMovement = playerCapsule.GetComponent<PlayerMovement>();
            mouseLookScript = playerCapsule.GetComponentInChildren<MouseMovement>();
        }
    }

    void Update()
    {
        // Added 'canExitLocker' check so it ignores the initial entry frame button tap
        if (playerIsInside && canExitLocker && Input.GetKeyDown(KeyCode.E))
        {
            ExitLocker();
        }
    }

    public void OnInteract()
    {
        if (playerCapsule == null || playerInv == null) return;

        if (!playerIsInside)
        {
            EnterLocker();
        }
        else if (canExitLocker) // Only allow interaction exit if guard rail is dropped
        {
            ExitLocker();
        }
    }

    void EnterLocker()
    {
        playerIsInside = true;
        canExitLocker = false; // Lock out the exit key instantly

        // PLAY LOCKER SFX ON ENTRY
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.lockerDoorSFX);
        }

        if (playerInv != null) playerInv.isHidden = true;

        CharacterController controller = playerCapsule.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;

        playerCapsule.transform.position = insidePosition.position;
        playerCapsule.transform.rotation = insidePosition.rotation;

        if (controller != null) controller.enabled = true;

        if (playerMovement != null) playerMovement.enabled = false;
        if (mouseLookScript != null) mouseLookScript.enabled = false;

        Debug.Log("Entered hiding spot.");

        // Start our 1-frame safety countdown
        StartCoroutine(EnableExitNextFrame());
    }

    void ExitLocker()
    {
        playerIsInside = false;
        canExitLocker = false; // Reset state

        // PLAY LOCKER SFX ON EXIT
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.lockerDoorSFX);
        }

        playerInv.isHidden = false;

        CharacterController controller = playerCapsule.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;

        playerCapsule.transform.position = outsidePosition.position;

        if (controller != null) controller.enabled = true;

        if (playerMovement != null) playerMovement.enabled = true;
        if (mouseLookScript != null) mouseLookScript.enabled = true;

        Debug.Log("Exited hiding spot.");
    }

    // This waits exactly 1 frame for the current 'E' keypress signature to expire
    private IEnumerator EnableExitNextFrame()
    {
        yield return null; // Waits for the exact next frame tick
        canExitLocker = true; // Safely open the exit gate inputs
    }
}