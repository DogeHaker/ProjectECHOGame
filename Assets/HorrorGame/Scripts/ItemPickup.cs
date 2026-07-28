using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Pickup Identity")]
    public string itemName = "Keycard1"; // Type: Flashlight, Battery, Keycard1, Keycard2, Keycard3, etc.
    public bool triggersEmergencyOnPickup = false; // Check true ONLY for the absolute final puzzle block!

    public void OnInteract()
    {
        PlayerInventoryV2 inventory = FindObjectOfType<PlayerInventoryV2>();

        if (inventory != null)
        {
            bool success = inventory.AddItemToInventory(itemName);

            if (success)
            {
                Debug.Log(itemName + " moved to slot container matrix.");

                // If this specific object was the master breaker, notify the game clock
                if (triggersEmergencyOnPickup)
                {
                    GameManager.Instance.TriggerEmergencySequence();
                }

                Destroy(gameObject); // Safely erase object from world space
            }
        }
    }
}