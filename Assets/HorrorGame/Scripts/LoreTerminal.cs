using UnityEngine;

public class LoreTerminal : MonoBehaviour
{
    [Header("Terminal Logs Content")]
    [TextArea(5, 10)] // Gives you a spacious text block entry in the inspector
    public string logContentData;

    // Triggered when looked at via the central camera raycast and pressing E
    public void OnInteract()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.terminalClickSFX);
        }
        if (LoreManager.Instance != null)
        {
            // Send the custom narrative log data directly to our display overlay window
            LoreManager.Instance.OpenLoreWindow(logContentData);
        }
    }
}