using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music")]
    public AudioClip backgroundMusic;
    [Range(0f, 1f)] public float musicVolume = 0.5f;

    [Header("Sound Effects")]
    public AudioClip starCollectSound;
    public AudioClip diamondCollectSound;
    public AudioClip hitSound;
    public AudioClip killSound;
    public AudioClip bounceSound;

    [Range(0f, 1f)] public float sfxVolume = 1f;

    AudioSource musicSource;
    AudioSource sfxSource;

    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Create music source
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.playOnAwake = false;

        // Create sfx source
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.volume = sfxVolume;
        sfxSource.playOnAwake = false;
    }

    void Start()
    {
        PlayMusic();
    }

    // ── Music ──

    public void PlayMusic()
    {
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        if (musicSource != null)
            musicSource.volume = volume;
    }

    // ── Sound Effects ──

    public void PlayStar()
    {
        PlaySFX(starCollectSound);
    }

    public void PlayDiamond()
    {
        PlaySFX(diamondCollectSound);
    }

    public void PlayHit()
    {
        PlaySFX(hitSound);
    }

    public void PlayKill()
    {
        StopMusic();
        PlaySFX(killSound);
    }

    public void PlayBounce()
    {
        PlaySFX(bounceSound);
    }

    void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }
}