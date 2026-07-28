using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory Slots")]
    public string[] slots = new string[3] { "Empty", "Empty", "Empty" };
    public int currentSlotIndex = 0;

    [Header("Item Visuals (On Camera)")]
    public GameObject flashlightHandModel;
    public Light flashlightSpotlight;
    public GameObject batteryBarUI;
    public GameObject batteryHandModel;
    public GameObject keycardHandModel;

    [Header("Drop Prefabs (Spawning on Floor)")]
    public GameObject flashlightFloorPrefab;
    public GameObject batteryFloorPrefab;
    public GameObject keycardFloorPrefab;
    public Transform dropSpawnPoint; // Create an empty child under camera slightly forward

    private FlashlightPower flashlightPowerScript;

    void Start()
    {
        flashlightPowerScript = GetComponent<FlashlightPower>();
        UpdateEquippedItem();
    }

    void Update()
    {
        // 1. Hotbar Switching
        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeSlot(2);

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) ChangeSlot((currentSlotIndex + 1) % slots.Length);
        else if (scroll < 0f) ChangeSlot((currentSlotIndex - 1 + slots.Length) % slots.Length);

        // 2. Drop Item Input (G Key)
        if (Input.GetKeyDown(KeyCode.G))
        {
            DropCurrentItem();
        }

        // 3. Reload Input Redirect (R Key)
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

        // Hand Flashlight visibility logic
        if (currentItem == "Flashlight")
        {
            flashlightHandModel.SetActive(true);
            if (flashlightPowerScript != null && flashlightPowerScript.currentPower > 0)
                flashlightSpotlight.enabled = true;

            // SHOW the battery bar when holding the flashlight
            if (batteryBarUI != null) batteryBarUI.SetActive(true);
        }
        else
        {
            flashlightHandModel.SetActive(false);
            flashlightSpotlight.enabled = false;

            // HIDE the battery bar when hands are empty or holding other items
            if (batteryBarUI != null) batteryBarUI.SetActive(false);
        }
        // Hand Battery visibility logic
        if (currentItem == "Battery")
        {
            batteryHandModel.SetActive(true);
        }
        else
        {
            batteryHandModel.SetActive(false);
        }
        // Hand Keycard visibility logic
        if (currentItem == "Keycard")
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
        Debug.Log("Inventory Full!");
        return false;
    }

    // New Feature: Search inventory for a battery to reload the flashlight
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

    // New Feature: Drop items back into the world geometry
    void DropCurrentItem()
    {
        string itemToDrop = slots[currentSlotIndex];
        if (itemToDrop == "Empty") return;

        GameObject prefabToSpawn = null;

        // Determine which physical object to instantiate (spawn)
        if (itemToDrop == "Flashlight") prefabToSpawn = flashlightFloorPrefab;
        if (itemToDrop == "Battery") prefabToSpawn = batteryFloorPrefab;
        if (itemToDrop == "Keycard") prefabToSpawn = keycardFloorPrefab;

        if (prefabToSpawn != null && dropSpawnPoint != null)
        {
            // Spawn the item on the floor in front of the player
            Instantiate(prefabToSpawn, dropSpawnPoint.position, dropSpawnPoint.rotation);

            slots[currentSlotIndex] = "Empty"; // Clear the hotbar slot
            UpdateEquippedItem();
            Debug.Log("Dropped: " + itemToDrop);
        }
    }
}