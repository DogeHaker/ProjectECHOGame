using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio Mixer Target")]
    public AudioMixer mainAudioMixer;

    [Header("UI Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider sensitivitySlider; // NEW: Drag your Sensitivity Slider here!

    [Header("Panels Navigation")]
    public GameObject settingsPanel;
    public GameObject mainButtonsPanel;

    void Start()
    {
        // Load saved values or set default baselines
        float savedMusic = PlayerPrefs.GetFloat("SavedMusicVolume", -15f);
        float savedSFX = PlayerPrefs.GetFloat("SavedSFXVolume", -10f);
        float savedSens = PlayerPrefs.GetFloat("MouseSensitivity", 100f); // Default 100 speed

        if (musicSlider != null) musicSlider.value = savedMusic;
        if (sfxSlider != null) sfxSlider.value = savedSFX;
        if (sensitivitySlider != null) sensitivitySlider.value = savedSens;

        SetMusicVolume(savedMusic);
        SetSFXVolume(savedSFX);
        SetSensitivity(savedSens);
    }

    public void SetSensitivity(float value)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", value);
    }

    public void SetMusicVolume(float volume)
    {
        if (volume <= -39f) volume = -80f;
        if (mainAudioMixer != null) mainAudioMixer.SetFloat("MusicVolume", volume);
        PlayerPrefs.SetFloat("SavedMusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        if (volume <= -39f) volume = -80f;
        if (mainAudioMixer != null) mainAudioMixer.SetFloat("SFXVolume", volume);
        PlayerPrefs.SetFloat("SavedSFXVolume", volume);
    }

    public void OpenSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(false);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(true);
    }
}