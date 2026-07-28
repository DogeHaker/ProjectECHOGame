using UnityEngine;

public class PickupFlashlight : MonoBehaviour
{
    public void OnInteract()
    {
        // Find the inventory manager on the player
        PlayerInventory inventory = FindObjectOfType<PlayerInventory>();

        if (inventory != null)
        {
            // Try to add the flashlight to an open slot
            bool success = inventory.AddItemToInventory("Flashlight");

            if (success)
            {
                Debug.Log("Flashlight added to inventory hotbar.");
                Destroy(gameObject); // Clear it from the ground scene
            }
        }
    }
}
