using UnityEngine;

public class InspectSign : MonoBehaviour
{
    [Header("Sign Information")]
    public string roomTitle = "SECTOR 04: CORE PROCESSING";
    public float displayDuration = 2.0f;

    private float cooldownTimer = 0f;

    void Update()
    {
        // Countdown timer between message displays
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    // Call this from your player's interaction script when looking at the sign
    public void OnHover()
    {
        if (cooldownTimer <= 0)
        {
            // Set cooldown slightly longer than duration so it doesn't re-trigger immediately
            cooldownTimer = displayDuration + 1.0f;

            NotificationUI notifier = FindObjectOfType<NotificationUI>();
            if (notifier != null)
            {
                notifier.DisplayMessage(roomTitle, displayDuration);
            }
        }
    }
}