using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject menuUIPanel; // Drag your UI Menu Canvas Panel here

    private bool isMenuOpen = false;
    private MouseMovement mouseLookScript;
    private PlayerMovement playerMovementScript; // Link to freeze player controls while typing/clicking

    void Start()
    {
        // Safely cache your player's physical input scripts
        mouseLookScript = FindObjectOfType<MouseMovement>();
        playerMovementScript = FindObjectOfType<PlayerMovement>();

        if (menuUIPanel != null)
            menuUIPanel.SetActive(false); // Hide menu instantly on game start
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isMenuOpen) CloseMenu();
            else OpenMenu();
        }
    }

    public void CloseMenu()
    {
        if (menuUIPanel != null) menuUIPanel.SetActive(false);
        isMenuOpen = false;

        // 1. Re-lock mouse to center of screen for first-person gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 2. Give control back to the player character
        if (mouseLookScript != null) mouseLookScript.enabled = true;
        if (playerMovementScript != null) playerMovementScript.enabled = true;
    }

    void OpenMenu()
    {
        if (menuUIPanel != null) menuUIPanel.SetActive(true);
        isMenuOpen = true;

        // 1. Free the mouse cursor so the player can physically click menu buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 2. CRITICAL: Turn off player controls so they don't blindly walk or spin 
        // their head around while trying to click buttons.
        if (mouseLookScript != null) mouseLookScript.enabled = false;
        if (playerMovementScript != null) playerMovementScript.enabled = false;

        // NOTE: The Android AI, scripts, alarms, and timers continue running completely unpaused!
        Debug.LogWarning("⚠️ MENU OPENED: The facility systems remain fully active. Watch your back.");
    }

    public void OnClickQuitToMainMenu()
    {
        // Safely wipe session data registries before changing scenes
        SaveManager.OpenedDoorRegistry.Clear();
        SceneManager.LoadScene("MainMenu"); // Ensure this matches your exact scene asset string name
    }
}