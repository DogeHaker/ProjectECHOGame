using UnityEngine;
using System.Collections.Generic; // CRITICAL: Gives us access to list conversions

public class PlayerInventoryV2 : MonoBehaviour
{
    public bool isHidden = false; // True when inside a locker or under a table

    [Header("Inventory Slots")]
    public string[] slots = new string[3] { "Empty", "Empty", "Empty" };
    public int currentSlotIndex = 0;

    [Header("Item Visuals (On Camera)")]
    public GameObject flashlightHandModel;
    public GameObject keycardHandModel;
    public GameObject batteryHandModel;
    public Light flashlightSpotlight;
    public GameObject batteryBarUI; // Drag your UI Slider Canvas object here

    [Header("Drop Prefabs (Spawning on Floor)")]
    public GameObject flashlightFloorPrefab;
    public GameObject batteryFloorPrefab;
    public GameObject keycard1FloorPrefab;
    public GameObject keycard2FloorPrefab;
    public GameObject keycard3FloorPrefab;
    public Transform dropSpawnPoint; // Empty object forward from camera

    private FlashlightPower flashlightPowerScript;

    void Start()
    {
        flashlightPowerScript = GetComponent<FlashlightPower>();
        UpdateEquippedItem();
    }

    void Update()
    {
        // 1. Hotbar Number Select
        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeSlot(2);

        // 2. Hotbar Scroll Select
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) ChangeSlot((currentSlotIndex + 1) % slots.Length);
        else if (scroll < 0f) ChangeSlot((currentSlotIndex - 1 + slots.Length) % slots.Length);

        // 3. Drop Item Mechanic (G Key)
        if (Input.GetKeyDown(KeyCode.G)) DropCurrentItem();

        // 4. Reload Flashlight Mechanic (R Key)
        if (Input.GetKeyDown(KeyCode.R) && slots[currentSlotIndex] == "Flashlight")
        {
            TryConsumeBatteryFromInventory();
        }
    }

    void ChangeSlot(int newIndex)
    {
        currentSlotIndex = newIndex;
        UpdateEquippedItem();
    }

    public void UpdateEquippedItem()
    {
        string currentItem = slots[currentSlotIndex];

        // Flashlight hand logic
        if (currentItem == "Flashlight")
        {
            flashlightHandModel.SetActive(true);
            if (flashlightPowerScript != null && flashlightPowerScript.currentPower > 0)
                flashlightSpotlight.enabled = true;
            if (batteryBarUI != null) batteryBarUI.SetActive(true);
        }
        else
        {
            flashlightHandModel.SetActive(false);
            flashlightSpotlight.enabled = false;
            if (batteryBarUI != null) batteryBarUI.SetActive(false);
        }
        if (currentItem == "Battery")
        {
            batteryHandModel.SetActive(true);
        }
        else
        {
            batteryHandModel.SetActive(false);
        }
        if (currentItem == "Keycard1" || currentItem == "Keycard2" || currentItem == "Keycard3")
        {
            keycardHandModel.SetActive(true);
        }
        else
        {
            keycardHandModel.SetActive(false);
        }
    }

    public bool AddItemToInventory(string itemName)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == "Empty")
            {
                slots[i] = itemName;
                UpdateEquippedItem();
                return true;
            }
        }
        return false; // Inventory full
    }

    // Helper function for doors to easily check if player has an item
    public bool HasItem(string itemName)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == itemName) return true;
        }
        return false;
    }

    void DropCurrentItem()
    {
        string itemToDrop = slots[currentSlotIndex];
        if (itemToDrop == "Empty") return;

        GameObject prefabToSpawn = null;
        if (itemToDrop == "Flashlight") prefabToSpawn = flashlightFloorPrefab;
        if (itemToDrop == "Battery") prefabToSpawn = batteryFloorPrefab;
        if (itemToDrop == "Keycard1") prefabToSpawn = keycard1FloorPrefab;
        if (itemToDrop == "Keycard2") prefabToSpawn = keycard2FloorPrefab;
        if (itemToDrop == "Keycard3") prefabToSpawn = keycard3FloorPrefab;

        if (prefabToSpawn != null && dropSpawnPoint != null)
        {
            Instantiate(prefabToSpawn, dropSpawnPoint.position, dropSpawnPoint.rotation);
            slots[currentSlotIndex] = "Empty";
            UpdateEquippedItem();
        }
    }

    void TryConsumeBatteryFromInventory()
    {
        if (flashlightPowerScript == null || flashlightPowerScript.currentPower >= flashlightPowerScript.maxPower) return;

        // Look through all slots for a battery string
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == "Battery")
            {
                slots[i] = "Empty"; // Consume it
                flashlightPowerScript.currentPower = flashlightPowerScript.maxPower; // Refill
                flashlightSpotlight.enabled = true;
                UpdateEquippedItem();
                Debug.Log("Consumed a battery from slot " + (i + 1));
                return;
            }
        }
        Debug.Log("No batteries inside any inventory slots!");
    }

    // ==========================================
    //            SAVE SYSTEM LINKS
    // ==========================================

    // Called by SaveManager to pull strings out of matrix slots
    public List<string> GetCurrentInventoryList()
    {
        List<string> list = new List<string>();
        for (int i = 0; i < slots.Length; i++)
        {
            list.Add(slots[i]);
        }
        return list;
    }

    // Called by SaveManager on load file execution frame
    public void ReconstructInventoryFromSave(List<string> savedItems)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (savedItems != null && i < savedItems.Count)
            {
                slots[i] = savedItems[i];
            }
            else
            {
                slots[i] = "Empty";
            }
        }
        currentSlotIndex = 0; // Default selection focus to slot 1 safely
        UpdateEquippedItem(); // Refresh item rendering engines
    }
}