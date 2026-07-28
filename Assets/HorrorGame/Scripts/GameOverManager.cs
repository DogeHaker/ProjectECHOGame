using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [Header("UI Panels")]
    public GameObject gameOverPanel;
    public GameObject mainGameOverButtons; // Container holding Load & Quit buttons
    public GameObject loadSlotsPanel;       // Container holding Slot 1, 2, 3 buttons

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (loadSlotsPanel != null) loadSlotsPanel.SetActive(false);
    }

    // Call this method from your EnemyAI script when the jumpscare finishes!
    public void TriggerGameOverSequence(float delayBeforeScreen = 2.0f)
    {
        StartCoroutine(GameOverRoutine(delayBeforeScreen));
    }

    IEnumerator GameOverRoutine(float delay)
    {
        // Wait for jumpscare animation/audio to play out
        yield return new WaitForSeconds(delay);

        // Turn on black Game Over overlay
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (mainGameOverButtons != null) mainGameOverButtons.SetActive(true);
        if (loadSlotsPanel != null) loadSlotsPanel.SetActive(false);

        // Free mouse cursor so player can click load buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Disable player controls completely
        MouseMovement mouseLook = FindObjectOfType<MouseMovement>();
        PlayerMovement playerMove = FindObjectOfType<PlayerMovement>();
        if (mouseLook != null) mouseLook.enabled = false;
        if (playerMove != null) playerMove.enabled = false;
    }

    // --- BUTTON TRIGGERS ---

    public void OnClickRestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void OnClickOpenLoadSlots()
    {
        if (mainGameOverButtons != null) mainGameOverButtons.SetActive(false);
        if (loadSlotsPanel != null) loadSlotsPanel.SetActive(true);
    }

    public void OnClickCloseLoadSlots()
    {
        if (loadSlotsPanel != null) loadSlotsPanel.SetActive(false);
        if (mainGameOverButtons != null) mainGameOverButtons.SetActive(true);
    }

    public void LoadFromSlot(int slotNumber)
    {
        MainMenuManager.IsLoadingFromSave = true; // Flag for SaveManager
        SaveManager.currentActiveSlot = slotNumber;

        // Reload the scene cleanly
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnClickReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}