using UnityEngine;

public class MusicManager : MonoSingleton<MusicManager>
{
    private static MusicManager musicManagerInstance;

    public AudioClip MenuMusic;
    public AudioClip TrapMusic;

    public AudioClip HuntMusic;

    public AudioSource audioSource;

    void Awake()
    {
        DontDestroyOnLoad(this);

        if(musicManagerInstance == null)
        {
            musicManagerInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMenuMusic()
    {
        audioSource.clip = MenuMusic;
        audioSource.Play();
    }

    public void PlayTrapMusic()
    {
        audioSource.clip = TrapMusic;
        audioSource.Play();
    }

    public void PlayHuntMusic()
    {
        audioSource.clip = HuntMusic;
        audioSource.Play();
    }

}
