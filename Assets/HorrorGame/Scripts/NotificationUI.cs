using UnityEngine;
using TMPro;
using System.Collections;

public class NotificationUI : MonoBehaviour
{
    private TextMeshProUGUI textComponent;
    private Coroutine currentFadeRoutine;

    void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();

        // Hide our text the exact frame the game boots up
        gameObject.SetActive(false);
    }

    // Call this public function from ANY script to flash text on screen
    public void DisplayMessage(string message, float duration = 2.5f)
    {
        // If an old message is still counting down on screen, cut it short
        if (currentFadeRoutine != null)
        {
            StopCoroutine(currentFadeRoutine);
        }

        // Apply our new string text and wake up the game object
        textComponent.text = message;
        gameObject.SetActive(true);

        // Start the automated countdown timer
        currentFadeRoutine = StartCoroutine(HideMessageAfterDelay(duration));
    }

    private IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}