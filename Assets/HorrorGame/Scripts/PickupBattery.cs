using UnityEngine;

public class PickupBattery : MonoBehaviour
{
    public void OnInteract()
    {
        // Find the slot manager on the player capsule
        PlayerInventory inventory = FindObjectOfType<PlayerInventory>();

        if (inventory != null)
        {
            // Push the item string into an open hotbar array slot
            bool success = inventory.AddItemToInventory("Battery");

            if (success)
            {
                Debug.Log("Battery safely loaded into inventory slot.");
                Destroy(gameObject); // Clear it from the ground scene
            }
        }
    }
}
