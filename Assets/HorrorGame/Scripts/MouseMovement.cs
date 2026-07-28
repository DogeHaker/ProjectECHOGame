using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    [Header("Mouse Settings")]
    public float mouseSensitivity = 100f; // Default fallback sensitivity
    private float xRotation = 0f;
    private float yRotation = 0f;

    [Header("Clamp Settings")]
    public float topClamp = 75f;
    public float bottomClamp = -75f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Dynamically pull active sensitivity set by the Settings menu slider
        float activeSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", mouseSensitivity);

        float mouseX = Input.GetAxis("Mouse X") * activeSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * activeSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, bottomClamp, topClamp);

        yRotation += mouseX;
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}