using UnityEngine;

public class SecurityDoor : MonoBehaviour
{
    private bool isOpen = false;
    public float openSpeed = 2f;
    private Vector3 targetPosition;

    [Header("Save System Identity")]
    public string doorID; // CRITICAL: Type a unique name for EACH door in your inspector!

    [Header("Lock Parameters")]
    public string requiredItem = "Keycard1"; // Change to Keycard2, Keycard3, or BigRedButton in inspector
    public string doorLockedMessage = "Locked. Level 1 Security Access Required.";
    public string doorSuccessMessage = "ACCESS GRANTED";

    [Header("UI Injection")]
    public NotificationUI uiNotifier; // Drag your NotificationText Canvas object here

    void Start()
    {
        // Slid upward into ceiling frame
        targetPosition = transform.position + (transform.up * 4f);
    }

    public void OnInteract()
    {
        if (isOpen) return;

        PlayerInventoryV2 playerInv = FindObjectOfType<PlayerInventoryV2>();

        if (playerInv != null)
        {
            // Check the centralized inventory array for the target string
            if (playerInv.HasItem(requiredItem))
            {
                isOpen = true;
                if (uiNotifier != null) uiNotifier.DisplayMessage(doorSuccessMessage, 2.5f);

                // SAVE SYSTEM: Register this door as permanently open for this playthrough
                if (!string.IsNullOrEmpty(doorID) && !SaveManager.OpenedDoorRegistry.Contains(doorID))
                {
                    SaveManager.OpenedDoorRegistry.Add(doorID);
                }
            }
            else
            {
                if (uiNotifier != null) uiNotifier.DisplayMessage(doorLockedMessage, 3f);
            }
        }
    }

    void Update()
    {
        if (isOpen)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, openSpeed * Time.deltaTime);
        }
    }

    // SAVE SYSTEM LINK: Called by the SaveManager to restore state instantly without sliding transitions
    public void ForceOpenOnLoad()
    {
        isOpen = true;
        targetPosition = transform.position + (transform.up * 4f);
        transform.position = targetPosition; // Snap directly to final open coordinates
    }
}