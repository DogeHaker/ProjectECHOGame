using UnityEngine;
using TMPro;

public class KeypadUIManager : MonoBehaviour
{
    public static KeypadUIManager Instance { get; private set; }

    [Header("UI Document Connections")]
    public GameObject keypadPanelRoot;    // Drag your 'KeypadPanel' here
    public TextMeshProUGUI displayField;  // Drag your 'KeypadDisplay' text here

    [Header("Player Tracking Links")]
    public MouseMovement mouseLookScript; // Drag your camera MouseMovement component here
    public PlayerMovement movementScript;  // Drag your PlayerMovement component here

    private string currentInputString = "";
    private KeypadPuzzle activeKeypadObject; // Tracks which physical wall panel we are using

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void OpenKeypad(KeypadPuzzle physicalKeypad)
    {
        activeKeypadObject = physicalKeypad;
        currentInputString = "";
        displayField.text = "CODE"; 
        keypadPanelRoot.SetActive(true);

        // Freeze player movement and camera controls
        if (mouseLookScript != null) mouseLookScript.enabled = false;
        if (movementScript != null) movementScript.enabled = false;

        // Release cursor mesh boundary constraints
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // LINK THIS TO BUTTONS 0-9 IN THE INSPECTOR (Pass the number as a string!)
    public void PressNumberButton(string numberPressed)
    {
        if (currentInputString.Length >= 4) return; // Max 4 digits

        currentInputString += numberPressed;
        displayField.text = currentInputString;

        // The split second they hit the 4th digit, instantly evaluate success
        if (currentInputString.Length == 4)
        {
            EvaluateCombination();
        }
    }

    // LINK THIS TO THE "C" CLEAR BUTTON
    public void PressClearButton()
    {
        currentInputString = "";
        displayField.text = "----";
    }

    void EvaluateCombination()
    {
        if (activeKeypadObject == null) return;

        if (currentInputString == activeKeypadObject.correctFourDigitCode)
        {
            displayField.text = "GRANTED";
            activeKeypadObject.SolvePuzzle(); // Tell the world object it's open!
            Invoke("CloseKeypad", 1.2f);     // Auto close window after a short delay
        }
        else
        {
            displayField.text = "WRONG";
            currentInputString = ""; // Reset
        }
    }

    // LINK THIS TO THE "X" CLOSE BUTTON
    public void CloseKeypad()
    {
        keypadPanelRoot.SetActive(false);
        activeKeypadObject = null;

        // Re-enable player physics and camera controllers
        if (mouseLookScript != null) mouseLookScript.enabled = true;
        if (movementScript != null) movementScript.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}