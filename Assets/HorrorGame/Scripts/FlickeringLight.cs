using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    private Light targetLight;

    [Header("Intensity Controls")]
    public float minIntensity = 0.2f;  // Brightness during a "dip"
    public float maxIntensity = 1.8f;  // Normal full brightness

    [Header("Flicker Speed")]
    public float minWaitTime = 0.05f;  // Shortest speed between flickering bursts
    public float maxWaitTime = 0.35f;  // Longest speed between flickering bursts

    private float timer;

    void Start()
    {
        targetLight = GetComponent<Light>();
    }

    void Update()
    {
        if (targetLight == null) return;

        // Count down the random timer frame by frame
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            // Pick a completely random intensity between min and max
            targetLight.intensity = Random.Range(minIntensity, maxIntensity);

            // Reset timer to a random delay for unpredictable intervals
            timer = Random.Range(minWaitTime, maxWaitTime);
        }
    }
}