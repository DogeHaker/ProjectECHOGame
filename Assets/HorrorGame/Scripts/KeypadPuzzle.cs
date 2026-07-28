using UnityEngine;

public class KeypadPuzzle : MonoBehaviour
{
    [Header("Puzzle Matrix Variables")]
    public string puzzleUniqueID = "WingA_DoorLock"; // CRITICAL FOR SAVING LATER!
    public string correctFourDigitCode = "1984";    // Change this to whatever you want
    public bool isSolved = false;                    // Save system will track this state

    [Header("Success Target Output Links")]
    public GameObject doorToOpen; // Drag the sliding door geometry object here
    public float doorSlideHeight = 4f;

    public void OnInteract()
    {
        // If the puzzle is already solved, don't open the UI panel again
        if (isSolved)
        {
            NotificationUI notifier = FindObjectOfType<NotificationUI>();
            if (notifier != null) notifier.DisplayMessage("Hydraulic lines already bypassed.", 2f);
            return;
        }

        // Send this specific script reference to the manager to open the screen interface
        if (KeypadUIManager.Instance != null)
        {
            KeypadUIManager.Instance.OpenKeypad(this);
        }
    }

    public void SolvePuzzle()
    {
        if (isSolved) return; // Prevent double-triggering

        isSolved = true;
        Debug.Log("Puzzle " + puzzleUniqueID + " marked as permanently cleared.");

        // Physical output effect: Shift the door frame out of the way
        if (doorToOpen != null)
        {
            doorToOpen.transform.position += new Vector3(0, doorSlideHeight, 0);
        }

        // SAVE SYSTEM: Register this keypad as permanently solved for this playthrough
        if (!string.IsNullOrEmpty(puzzleUniqueID) && !SaveManager.SolvedKeypadRegistry.Contains(puzzleUniqueID))
        {
            SaveManager.SolvedKeypadRegistry.Add(puzzleUniqueID);
        }

        // FUTURE CODE: Play an electronic unlock chime audio effect here
    }

    // SAVE SYSTEM LINK: Restores the solved state cleanly when loading a save file
    public void ForceSolveOnLoad()
    {
        // Only move the door if the scene object hasn't been processed yet
        if (!isSolved)
        {
            isSolved = true;
            if (doorToOpen != null)
            {
                doorToOpen.transform.position += new Vector3(0, doorSlideHeight, 0);
            }
        }
    }
}