using TMPro;
using UnityEngine;

public class LoreManager : MonoBehaviour
{
    public static LoreManager Instance { get; private set; }

    [Header("UI Document Elements")]
    public GameObject lorePanelRoot;      // Drag your 'LorePanel' here
    public TextMeshProUGUI loreDisplay;  // Drag your 'LoreText' here

    [Header("Player Control Links")]
    public MouseMovement mouseLookScript; // Drag your camera MouseLook script component here
    public PlayerMovement playerMovement;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void OpenLoreWindow(string fullStoryText)
    {
        loreDisplay.text = fullStoryText;
        lorePanelRoot.SetActive(true); // Open the display overlay

        // 1. Freeze player looking around so they can use the menu safely
        if (mouseLookScript != null) mouseLookScript.enabled = false;
        if (playerMovement != null) playerMovement.enabled = false;

        // 2. Unlock the mouse cursor so they can physically click the X button
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // THIS IS THE METHOD LINKED TO YOUR CLICKABLE 'X' BUTTON
    public void CloseLoreWindow()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.terminalClickSFX);
        }
        lorePanelRoot.SetActive(false); // Hide the overlay panel

        // 1. Unfreeze player camera looking
        if (mouseLookScript != null) mouseLookScript.enabled = true;
        if (playerMovement != null) playerMovement.enabled = true;
        // 2. Lock the mouse cursor back to the center of the first person screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}