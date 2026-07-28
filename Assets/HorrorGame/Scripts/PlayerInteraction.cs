using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionDistance = 3.5f;

    [Header("UI Reference")]
    public TextMeshProUGUI interactionPromptText; // Drag your 'InteractionPromptText' here!

    void Update()
    {
        // Shoot a raycast straight out from the center of the first-person camera view
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // 1. Check if the ray hits anything within reach
        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            // A. Check if we are looking at a room title sign (Auto-hover, no 'E' key needed!)
            InspectSign sign = hit.collider.GetComponent<InspectSign>();
            if (sign != null)
            {
                sign.OnHover(); // Triggers the notification popup automatically
                ClearPrompt();  // Ensure no "[E] Interact" text shows up on screen
                return;         // Exit early so it doesn't trigger generic interactables below
            }

            // B. Check if we are looking at a pickup item
            ItemPickup pickup = hit.collider.GetComponent<ItemPickup>();
            if (pickup != null)
            {
                // Dynamic prompt: Updates based on whatever custom name you typed in the inspector!
                if (interactionPromptText != null)
                {
                    interactionPromptText.text = "[E] " + pickup.itemName;
                }

                // If they tap E while looking at the item, trigger its collection logic
                if (Input.GetKeyDown(KeyCode.E))
                {
                    pickup.OnInteract();
                    ClearPrompt(); // Instantly clear prompt since the item object is destroyed
                }
                return; // Exit out of Update early so we don't clear the text below
            }

            // C. Check if we are looking at any other generic interactable object (Doors, Lockers, Terminals)
            if (hit.collider.CompareTag("Interactable"))
            {
                if (interactionPromptText != null)
                {
                    interactionPromptText.text = "[E] Interact";
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    // This uses Unity's messaging system to fire whatever OnInteract() method 
                    // is sitting on the targeted object (HidingPlayer, SecurityDoor, LoreTerminal, etc.)
                    hit.collider.gameObject.SendMessage("OnInteract", SendMessageOptions.DontRequireReceiver);
                }
                return;
            }
        }

        // 2. FALLBACK: If the raycast hits absolutely nothing, or walks away from an item, clear the screen
        ClearPrompt();
    }

    void ClearPrompt()
    {
        if (interactionPromptText != null && interactionPromptText.text != "")
        {
            interactionPromptText.text = "";
        }
    }
}