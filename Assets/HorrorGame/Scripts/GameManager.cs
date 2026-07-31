using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public PlayerMovement playerMovement;
    public MouseMovement mouseLookScript;

    [Header("Emergency System")]
    public float timeRemaining = 120f;
    private bool isCountdownActive = false;

    [Header("UI Connections")]
    public EmergencyUI emergencyScript; // Big scary center timer text

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (emergencyScript != null) emergencyScript.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isCountdownActive)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerUI();
            }
            else
            {
                TriggerBadEnding();
            }
        }
    }

    public void TriggerEmergencySequence()
    {
        if (isCountdownActive) return; // Don't double trigger

        isCountdownActive = true;
        if (emergencyScript != null) emergencyScript.StartEmergencyDisplay();

        NotificationUI notifier = FindObjectOfType<NotificationUI>();
        if (notifier != null)
        {
            notifier.DisplayMessage("WARNING: OVERLOAD INTERRUPT PROTOCOL INITIATED. ESCAPE TO THE CORE INSTANTLY.", 5f);
        }
    }

    void UpdateTimerUI()
    {
        if (emergencyScript != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60F);
            int seconds = Mathf.FloorToInt(timeRemaining % 60F);
            string formalTime = string.Format("CRITICAL SYSTEM PURGE IN: {0:0}:{1:00}", minutes, seconds);
            emergencyScript.UpdateTimeText(formalTime);
        }
    }

    void TriggerBadEnding()
    {
        isCountdownActive = false;
        if (emergencyScript != null) emergencyScript.StopEmergencyDisplay();
        if (playerMovement != null) playerMovement.enabled = false;
        if (mouseLookScript != null) mouseLookScript.enabled = false;

        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.TriggerGameOverSequence();
        }

        NotificationUI notifier = FindObjectOfType<NotificationUI>();
        if (notifier != null)
        {
            notifier.DisplayMessage("SYSTEM FAILURE: SECURITY UNIT INTERCEPTED AGENT.\n[Press Escape to Quit]", 999f);
        }
    }
}