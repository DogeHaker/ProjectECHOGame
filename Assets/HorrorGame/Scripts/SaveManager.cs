using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    // Singleton instance so any script/button can easily call SaveManager.Instance
    public static SaveManager Instance;

    public static List<string> OpenedDoorRegistry = new List<string>();
    public static List<string> SolvedKeypadRegistry = new List<string>();

    public static int currentActiveSlot = 1;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (MainMenuManager.IsLoadingFromSave)
        {
            string targetPath = GetSavePathForSlot(currentActiveSlot);
            if (File.Exists(targetPath))
            {
                Debug.LogWarning("Save profile tracking data detected. Injecting files into scene...");
                LoadGame();
            }
        }
        else
        {
            Debug.LogWarning("Fresh run sequence authorized. Wiping session history registries clean.");
            OpenedDoorRegistry.Clear();
            SolvedKeypadRegistry.Clear();
        }
    }

    private string GetSavePathForSlot(int slotNumber)
    {
        return Path.Combine(Application.persistentDataPath, "facility_save_" + slotNumber + ".json");
    }

    // PUBLIC METHOD FOR UI BUTTONS: Saves directly to chosen slot number (1, 2, or 3)
    public void SaveToSlot(int slotNumber)
    {
        currentActiveSlot = slotNumber;
        SaveGame();
    }

    public void SaveGame()
    {
        GameObject player = GameObject.FindWithTag("Player");
        GameObject enemy = GameObject.FindWithTag("Enemy");
        PlayerInventoryV2 inventory = FindObjectOfType<PlayerInventoryV2>();
        FlashlightPower flashlight = FindObjectOfType<FlashlightPower>();

        if (player == null) return;

        GameSaveData data = new GameSaveData();

        // Record Enemy Position
        if (enemy != null)
        {
            data.eX = enemy.transform.position.x;
            data.eY = enemy.transform.position.y;
            data.eZ = enemy.transform.position.z;
        }

        // 1. Position Vectors
        data.pX = player.transform.position.x;
        data.pY = player.transform.position.y;
        data.pZ = player.transform.position.z;

        // 2. Mechanics State
        if (flashlight != null) data.batteryPercentage = flashlight.currentPower;
        if (inventory != null) data.storedItems = inventory.GetCurrentInventoryList();

        // 3. Environmental Registries
        data.openedDoorIDs = new List<string>(OpenedDoorRegistry);
        data.solvedKeypadIDs = new List<string>(SolvedKeypadRegistry);

        // 4. Write to disk
        string targetPath = GetSavePathForSlot(currentActiveSlot);
        string jsonText = JsonUtility.ToJson(data, true);
        File.WriteAllText(targetPath, jsonText);

        Debug.LogWarning("💾 DATA ENCRYPTED TO SLOT " + currentActiveSlot + " AT: " + targetPath);

        NotificationUI notifier = FindObjectOfType<NotificationUI>();
        if (notifier != null) notifier.DisplayMessage("FACILITY PROGRESS ENCRYPTED TO SLOT " + currentActiveSlot, 2.5f);
    }

    public void LoadGame()
    {
        string targetPath = GetSavePathForSlot(currentActiveSlot);

        if (!File.Exists(targetPath))
        {
            Debug.LogError("No data tracking file located for Slot: " + currentActiveSlot);
            return;
        }

        string jsonText = File.ReadAllText(targetPath);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(jsonText);

        // 1. Teleport Player
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            player.transform.position = new Vector3(data.pX, data.pY, data.pZ);
            if (cc != null) cc.enabled = true;
        }

        // 2. Reload Flashlight
        FlashlightPower flashlight = FindObjectOfType<FlashlightPower>();
        if (flashlight != null) flashlight.currentPower = data.batteryPercentage;

        // 3. Rebuild Inventory Pocket Slots
        PlayerInventoryV2 inventory = FindObjectOfType<PlayerInventoryV2>();
        if (inventory != null) inventory.ReconstructInventoryFromSave(data.storedItems);

        // 4. Restore Regular Security Doors
        OpenedDoorRegistry = new List<string>(data.openedDoorIDs);
        SecurityDoor[] allDoors = FindObjectsOfType<SecurityDoor>();
        foreach (SecurityDoor door in allDoors)
        {
            if (OpenedDoorRegistry.Contains(door.doorID)) door.ForceOpenOnLoad();
        }

        // 5. Restore Solved Keypad Doors
        SolvedKeypadRegistry = new List<string>(data.solvedKeypadIDs);
        KeypadPuzzle[] allKeypads = FindObjectsOfType<KeypadPuzzle>();
        foreach (KeypadPuzzle keypad in allKeypads)
        {
            if (SolvedKeypadRegistry.Contains(keypad.puzzleUniqueID))
            {
                keypad.ForceSolveOnLoad();
            }
        }

        // Restore Enemy Position
        GameObject enemyObject = GameObject.FindWithTag("Enemy");
        if (enemyObject != null)
        {
            UnityEngine.AI.NavMeshAgent agent = enemyObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null)
            {
                agent.Warp(new Vector3(data.eX, data.eY, data.eZ));
            }
            else
            {
                enemyObject.transform.position = new Vector3(data.eX, data.eY, data.eZ);
            }
        }

        Debug.LogWarning("📂 PROFILE INJECTED FROM SLOT: " + currentActiveSlot);
    }
}