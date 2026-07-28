using UnityEngine;

public class SaveTerminal : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject inGameSavePanel;

    // Called when the player looks at the cube and presses 'E'
    public void OnInteract()
    {
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
}