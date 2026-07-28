using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Needed for Button components!
using System.Collections;
using System.IO;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public static bool IsLoadingFromSave = false;

    [Header("UI Panels")]
    public GameObject mainButtonsPanel;
    public GameObject saveSlotsPanel;
    public GameObject introLorePanel;

    [Header("Main & Slot Buttons (For Graying Out)")]
    public Button mainContinueButton; // Drag your main CONTINUE / LOAD button here
    public Button slot1Button;        // Drag Slot 1 button here
    public Button slot2Button;        // Drag Slot 2 button here
    public Button slot3Button;        // Drag Slot 3 button here

    [Header("Intro Lore Configuration")]
    public TextMeshProUGUI loreTextElement;
    public string[] introLines = new string[]
    {
        "LOG ENTITY: PROJECT ECHO\nLOCATION: SUB-LEVEL 4 SUPERCOMPUTER CORE",
        "Systems went dark 14 hours ago. Automatic containment protocols failed.",
        "They sent me down to pull the master authorization drive...",
        "...but whatever is running the security grid isn't letting me out."
    };

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (saveSlotsPanel != null) saveSlotsPanel.SetActive(false);
        if (introLorePanel != null) introLorePanel.SetActive(false);
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(true);

        // Check save files on disk and gray out empty slots
        UpdateSaveButtonStates();
    }

    void UpdateSaveButtonStates()
    {
        // Check if save files physically exist on disk
        bool slot1Exists = File.Exists(GetSavePathForSlot(1));
        bool slot2Exists = File.Exists(GetSavePathForSlot(2));
        bool slot3Exists = File.Exists(GetSavePathForSlot(3));

        // Gray out individual slot buttons if no save file exists for them
        if (slot1Button != null) slot1Button.interactable = slot1Exists;
        if (slot2Button != null) slot2Button.interactable = slot2Exists;
        if (slot3Button != null) slot3Button.interactable = slot3Exists;

        // Gray out the main CONTINUE button if ZERO save files exist across all slots
        bool anySaveExists = slot1Exists || slot2Exists || slot3Exists;
        if (mainContinueButton != null) mainContinueButton.interactable = anySaveExists;
    }

    private string GetSavePathForSlot(int slotNumber)
    {
        return Path.Combine(Application.persistentDataPath, "facility_save_" + slotNumber + ".json");
    }

    // --- BUTTON TRIGGER METHODS ---

    public void OnClickNewGame()
    {
        IsLoadingFromSave = false;
        SaveManager.currentActiveSlot = 1;
        StartCoroutine(PlayIntroSequence());
    }

    public void OnClickOpenLoadMenu()
    {
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(false);
        if (saveSlotsPanel != null) saveSlotsPanel.SetActive(true);
    }

    public void OnClickCloseLoadMenu()
    {
        if (saveSlotsPanel != null) saveSlotsPanel.SetActive(false);
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(true);
    }

    public void SelectSlotAndLoadGame(int slotNumber)
    {
        IsLoadingFromSave = true;
        SaveManager.currentActiveSlot = slotNumber;

        SceneManager.LoadScene("Main");
    }

    public void OnClickQuitApplication()
    {
        Application.Quit();
    }

    // --- NARRATIVE TRANSITION MACHINE ---
    IEnumerator PlayIntroSequence()
    {
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(false);
        if (introLorePanel != null) introLorePanel.SetActive(true);

        // 1. Play lore text lines
        for (int i = 0; i < introLines.Length; i++)
        {
            loreTextElement.text = introLines[i];
            yield return new WaitForSeconds(3.5f);
        }

        // 2. Simple fade to black at the end
        Image bg = introLorePanel.GetComponent<Image>();
        float timer = 0f;
        while (timer < 1.5f)
        {
            timer += Time.deltaTime;
            if (bg != null) bg.color = new Color(0, 0, 0, timer / 1.5f);
            yield return null;
        }

        // 3. Load game
        SceneManager.LoadScene("Main");
    }
}