using UnityEngine;
using TMPro;
using System.Collections;

public class EmergencyUI : MonoBehaviour
{
    private TextMeshProUGUI textComponent;
    private bool isFlashing = false;

    [Header("Flash Speed Settings")]
    public float flashInterval = 0.5f; // Time in seconds between blinks

    void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();

        // Hide the text completely when the game first launches
        gameObject.SetActive(false);
    }

    // GameManager will call this when the countdown starts
    public void StartEmergencyDisplay()
    {
        gameObject.SetActive(true);

        if (!isFlashing)
        {
            StartCoroutine(FlashRoutine());
        }
    }

    // Call this if the player wins and hits the Big Red Button to shut it down
    public void StopEmergencyDisplay()
    {
        isFlashing = false;
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    // Public method allowing GameManager to update the numbers while it flashes
    public void UpdateTimeText(string newTimeText)
    {
        if (textComponent != null)
        {
            textComponent.text = newTimeText;
        }
    }

    private IEnumerator FlashRoutine()
    {
        isFlashing = true;

        while (isFlashing)
        {
            // Toggle the text rendering component visibility on/off
            textComponent.enabled = !textComponent.enabled;

            // Wait half a second before looping back to toggle it again
            yield return new WaitForSeconds(flashInterval);
        }

        // Ensure text is visible if the flashing loop breaks
        textComponent.enabled = true;
    }
}