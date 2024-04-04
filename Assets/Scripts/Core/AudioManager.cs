
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source ")]
    [SerializeField] public AudioSource musicSource;
    [SerializeField] public AudioSource SFXSource;

    [Header("Audio Clip ")]
    public AudioClip background;
    public AudioClip death;
    public AudioClip bullet;
    public AudioClip GameOver;
    public AudioClip Jump;
    public AudioClip ShowrdHit;
    public AudioClip Win;
    public AudioClip Run;
    public AudioClip click;

    private bool isMusicPaused = false;
    private float musicVolume = 1.0f;

    private void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
    }

    public void PlaySFX (AudioClip clip)
    {
        SFXSource.PlayOneShot (clip);
    }
    public void PauseBackgroundMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Pause();
            isMusicPaused = true;
        }
    }

    public void ResumeBackgroundMusic()
    {
        if (isMusicPaused)
        {
            musicSource.UnPause();
            isMusicPaused = false;
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
    }
}

