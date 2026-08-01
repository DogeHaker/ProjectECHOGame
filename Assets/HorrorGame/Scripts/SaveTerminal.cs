using UnityEngine;

public class SaveTerminal : MonoBehaviour
{
    [Header("Terminal State & Persistence")]
    public string terminalUniqueID = "SaveTerminal_SectorA"; // Unique ID for save files
    public bool isUsed = false;

    [Header("UI Reference")]
    public GameObject inGameSavePanel;

    [Header("Visual Feedback (Optional)")]
    public Renderer terminalRenderer;
    public Color depletedColor = Color.gray;
    public Light terminalLight;

    // Called when the player looks at the cube and presses 'E'
    public void OnInteract()
    {
        // BLOCK INTERACTION IF ALREADY USED
        if (isUsed)
        {
            NotificationUI notifier = FindObjectOfType<NotificationUI>();
            if (notifier != null)
            {
                notifier.DisplayMessage("SAVE TERMINAL EXHAUSTED: Internal battery core depleted.", 2.5f);
            }
            return;
        }

        if (inGameSavePanel != null)
        {
            inGameSavePanel.SetActive(true);

            // Free the mouse cursor so player can click a slot button
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Pause player look/movement while menu is open (AI keeps running!)
            MouseMovement mouseLook = FindObjectOfType<MouseMovement>();
            PlayerMovement playerMove = FindObjectOfType<PlayerMovement>();
            if (mouseLook != null) mouseLook.enabled = false;
            if (playerMove != null) playerMove.enabled = false;
        }
    }

    // Call this function when the player actually clicks Slot 1, 2, or 3 to save!
    public void ConsumeTerminal()
    {
        if (isUsed) return;

        isUsed = true;

        // Visual Feedback: Turn grey and shut off light indicator
        if (terminalRenderer != null)
        {
            terminalRenderer.material.color = depletedColor;
        }
        if (terminalLight != null)
        {
            terminalLight.enabled = false;
        }

        // Register ID so reloads keep this terminal exhausted
        if (!string.IsNullOrEmpty(terminalUniqueID) && !SaveManager.UsedSaveTerminals.Contains(terminalUniqueID))
        {
            SaveManager.UsedSaveTerminals.Add(terminalUniqueID);
        }

        NotificationUI notifier = FindObjectOfType<NotificationUI>();
        if (notifier != null)
        {
            notifier.DisplayMessage("SYSTEM STATE SERIALIZED. Terminal power depleted.", 3f);
        }

        CloseTerminalUI();
    }

    // Called when clicking a slot or Cancel button
    public void CloseTerminalUI()
    {
        if (inGameSavePanel != null)
        {
            inGameSavePanel.SetActive(false);

            // Lock cursor back to first-person controls
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Re-enable player movement/camera look
            MouseMovement mouseLook = FindObjectOfType<MouseMovement>();
            PlayerMovement playerMove = FindObjectOfType<PlayerMovement>();
            if (mouseLook != null) mouseLook.enabled = true;
            if (playerMove != null) playerMove.enabled = true;
        }
    }

    // Restores used state cleanly when loading a save file
    public void ForceDepleteOnLoad()
    {
        isUsed = true;
        if (terminalRenderer != null) terminalRenderer.material.color = depletedColor;
        if (terminalLight != null) terminalLight.enabled = false;
    }
}