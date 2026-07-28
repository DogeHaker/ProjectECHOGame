using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections; // Ensure system collection is included if needed

public class SupercomputerEnding : MonoBehaviour
{
    [Header("Item Requirement")]
    public string requiredItemName = "Master Core Key";

    [Header("UI References")]
    public CanvasGroup fadeCanvasGroup;  // Drag your Black Overlay CanvasGroup here
    public TextMeshProUGUI loreTextUI;   // Drag your Ending TextMeshPro here
    public GameObject mainMenuButton;   // Drag your 'Main Menu' UI Button here

    [Header("Ending Lore Lines")]
    [TextArea(2, 5)]
    public string[] endingLines = new string[]
    {
        "CORE OVERRIDE KEY ACCEPTED...",
        "PURGING PRIMARY NEURAL NETWORK...",
        "E.C.H.O. SYSTEM STATUS: OFF-LINE.",
        "THE FACILITY IS SILENT ONCE AGAIN."
    };

    public float fadeSpeed = 1.5f;
    public float lineDisplayDuration = 3.0f;
    public string mainMenuSceneName = "MainMenu";

    private bool endingStarted = false;

    // Called via PlayerInteraction when player hits 'E' on the panel
    public void OnInteract()
    {
        if (endingStarted) return;

        PlayerInventoryV2 inventory = FindObjectOfType<PlayerInventoryV2>();

        // 1. Check if player has the required keycard/item
        if (inventory != null && inventory.HasItem(requiredItemName))
        {
            StartCoroutine(RunEndingSequence());
        }
        else
        {
            // Show notification if they don't have the key
            NotificationUI notifier = FindObjectOfType<NotificationUI>();
            if (notifier != null)
            {
                notifier.DisplayMessage("SECURITY OVERRIDE REQUIRED: Missing " + requiredItemName, 2.5f);
            }
        }
    }

    private IEnumerator RunEndingSequence()
    {
        endingStarted = true;

        // 2. Freeze Player Movement & Camera
        PlayerMovement movement = FindObjectOfType<PlayerMovement>();
        MouseMovement mouseLook = FindObjectOfType<MouseMovement>();
        if (movement != null) movement.enabled = false;
        if (mouseLook != null) mouseLook.enabled = false;

        // Unlock cursor for UI button clicking
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 3. Fade Screen to Black
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.gameObject.SetActive(true);
            while (fadeCanvasGroup.alpha < 1f)
            {
                fadeCanvasGroup.alpha += Time.deltaTime * fadeSpeed;
                yield return null;
            }
            fadeCanvasGroup.alpha = 1f;
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(AudioManager.Instance.endingMusic, false);
        }

        // 4. Display Lore Lines sequentially
        if (loreTextUI != null)
        {
            loreTextUI.gameObject.SetActive(true);

            foreach (string line in endingLines)
            {
                loreTextUI.text = line;
                yield return new WaitForSeconds(lineDisplayDuration);
            }
        }

        // 5. Reveal the "Return to Main Menu" Button
        if (mainMenuButton != null)
        {
            mainMenuButton.SetActive(true);
        }
    }

    // Called when player clicks the UI button at the end
    public void ReturnToMainMenu()
    {
        // 1. Switch audio tracks back to Main Menu theme
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick(); // Optional UI click SFX
            AudioManager.Instance.PlayMenuMusic();
        }

        // 2. Load Main Menu Scene
        SceneManager.LoadScene(mainMenuSceneName);
    }
}