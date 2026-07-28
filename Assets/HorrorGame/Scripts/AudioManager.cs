using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;     // Route Output -> Music Group
    public AudioSource ambienceSource;  // Route Output -> Music Group
    public AudioSource chaseSource;     // Route Output -> Music Group
    public AudioSource sfxSource;       // Route Output -> SFX Group

    [Header("Music Clips")]
    public AudioClip menuMusic;
    public AudioClip ambientLoop;
    public AudioClip chaseMusic;
    public AudioClip endingMusic;

    [Header("Sound Effect Clips")]
    public AudioClip flashlightClickSFX;
    public AudioClip doorOpenSFX;
    public AudioClip itemPickupSFX;
    public AudioClip buttonClickSFX;
    public AudioClip keypadClickSFX;
    public AudioClip terminalClickSFX;
    public AudioClip lockerDoorSFX;
    public AudioClip jumpscareSFX;
    public AudioClip[] footstepSFX; 

    private void Awake()
    {
        // Singleton pattern to access AudioManager from any script
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Start playing ambient sound if assigned
        if (ambienceSource != null && ambientLoop != null)
        {
            ambienceSource.clip = ambientLoop;
            ambienceSource.loop = true;
            ambienceSource.Play();
        }
    }

    // ==========================================
    //            PLAYBACK HELPERS
    // ==========================================

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayRandomFootstep()
    {
        if (footstepSFX != null && footstepSFX.Length > 0 && sfxSource != null)
        {
            int randomIndex = Random.Range(0, footstepSFX.Length);
            sfxSource.PlayOneShot(footstepSFX[randomIndex], 0.25f); // Slightly quieter
        }
    }

    public void StartChaseMusic()
    {
        if (chaseSource != null && chaseMusic != null && !chaseSource.isPlaying)
        {
            chaseSource.clip = chaseMusic;
            chaseSource.loop = true;
            chaseSource.Play();
        }
    }

    public void StopChaseMusic()
    {
        if (chaseSource != null)
        {
            chaseSource.Stop();
        }
    }

    public void PlayJumpscare()
    {
        StopChaseMusic();
        if (jumpscareSFX != null && sfxSource != null)
        {
            StopChaseMusic();
            sfxSource.PlayOneShot(jumpscareSFX, 1.0f);
        }
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (musicSource != null && clip != null)
        {
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.Play();
        }
    }

    public void PlayMenuMusic()
    {
        // 1. Stop facility ambient hums and chase tracks
        if (ambienceSource != null) ambienceSource.Stop();
        if (endingMusic != null) musicSource.Stop();
        StopChaseMusic();

        // 2. Play the main menu theme on loop
        if (menuMusic != null)
        {
            PlayMusic(menuMusic, true);
        }
    }

    public void PlayButtonClick()
    {
        if (buttonClickSFX != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(buttonClickSFX);
        }
    }

    public void PlayKeypadClick()
    {
        if (keypadClickSFX != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(keypadClickSFX);
        }
    }
}