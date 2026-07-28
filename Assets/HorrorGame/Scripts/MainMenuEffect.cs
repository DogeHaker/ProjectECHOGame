using UnityEngine;

public class MainMenuEffect : MonoBehaviour
{
    [Header("Elevator Rumble Engine")]
    public float shakeIntensity = 0.02f;
    public float shakeSpeed = 15f;

    private Vector3 baselinePosition;

    void Start()
    {
        baselinePosition = transform.localPosition;
    }

    void Update()
    {
        // Create an organic, unsettling mechanical rumble using Perlin Noise math
        float offsetVectorX = (Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) - 0.5f) * shakeIntensity;
        float offsetVectorY = (Mathf.PerlinNoise(0f, Time.time * shakeSpeed) - 0.5f) * shakeIntensity;

        // Apply shake offset directly to base position coordinates
        transform.localPosition = baselinePosition + new Vector3(offsetVectorX, offsetVectorY, 0f);
    }
}
