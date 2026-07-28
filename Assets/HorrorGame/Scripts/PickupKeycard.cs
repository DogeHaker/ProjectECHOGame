using UnityEngine;

public class PickupKeycard : MonoBehaviour
{
    public void OnInteract()
    {
        PlayerInventory inventory = FindObjectOfType<PlayerInventory>();

        if (inventory != null)
        {
            // Attempt to slip the keycard string into an open slot
            bool success = inventory.AddItemToInventory("Keycard");

            if (success)
            {
                Debug.Log("Keycard pocketed.");
                Destroy(gameObject); // Vaporize the floor object
            }
        }
    }
}
