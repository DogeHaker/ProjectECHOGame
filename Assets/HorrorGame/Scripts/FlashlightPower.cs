using UnityEngine;
using UnityEngine.UI; // Required to communicate with your UI Slider

public class FlashlightPower : MonoBehaviour
{
    [Header("Battery Settings")]
    public float maxPower = 100f;
    public float currentPower = 100f;
    public float drainRate = 2f; // How much power drops per second
    public int batteryCount = 0;  // How many backup batteries we hold

    [Header("References")]
    public Light flashlightLight; // Drag your player camera's Spotlight here
    public Slider batterySlider;  // Drag your UI BatteryBar here

    void Start()
    {
        currentPower = maxPower;
        UpdateUI();
    }

    void Update()
    {
        // 1. Drain battery ONLY if the flashlight exists, is active in hierarchy, and the light component is turned ON
        if (flashlightLight != null && flashlightLight.gameObject.activeInHierarchy && flashlightLight.enabled)
        {
            if (currentPower > 0)
            {
                currentPower -= drainRate * Time.deltaTime;

                // Dim the light slightly as it approaches empty for extra atmosphere
                if (currentPower < 20f) flashlightLight.intensity = Random.Range(1f, 3f); // Flickers when dying!
            }
            else
            {
                currentPower = 0;
                flashlightLight.enabled = false; // Kill the light entirely
            }
            UpdateUI();
        }

        // 2. Manual Reload Input (Press R to reload)
    //    if (Input.GetKeyDown(KeyCode.R))
    //    {
    //        ReloadFlashlight();
    //    }
        }

    //public void ReloadFlashlight()
    //{
    //    if (batteryCount > 0 && currentPower < maxPower)
    //    {
    //        batteryCount--;
    //        currentPower = maxPower;

    //        // Restore default flashlight brightness stability
    //        flashlightLight.intensity = 5f;

    //        UpdateUI();
    //        Debug.Log("Flashlight reloaded! Remaining batteries: " + batteryCount);
    //    }
    //    else if (batteryCount <= 0)
    //    {
    //        Debug.Log("No batteries left in inventory!");
    //    }
    //}

    // Public method called by your collectible battery scripts
    public void AddBattery()
    {
        batteryCount++;
        Debug.Log("Collected a battery! Total: " + batteryCount);
    }

    void UpdateUI()
    {
        if (batterySlider != null)
        {
            batterySlider.value = currentPower;
        }
    }
}