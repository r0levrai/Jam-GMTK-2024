using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager>
{
    public AudioSource soundEffectSource;
    public AudioSource musicSource;
    public AudioSource newMusicSource;

    [Serializable]
    public struct Clip
    {
        public string name;
        public AudioClip audio;
        public int priority;

        public Clip(string n, AudioClip a, int p)
        {
            this.name = n;
            this.audio = a;
            this.priority = p;
        }
    }

    public struct Source
    {
        public string name;
        public AudioSource source;
        public Source(string n, AudioSource a)
        {
            this.name = n;
            this.source = a;
        }
    }


	void Awake()
	{
		if (FindObjectsOfType<SoundManager>().Length > 1)
		{
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
	}

	public List<Clip> clips = new List<Clip>();
    public List<Clip> musics = new List<Clip>();
    public List<Source> sources = new List<Source>();

    public float volumeSound = 1;
    public float volumeMusic = 0.2f;

    void Start()
    {
		currentIndex = 4;
        PlayMusic("human");
	}

    public void PlayOneShot(string audioClip)
    {
		var currentClip = clips.Find(c => c.name == audioClip);
        soundEffectSource.PlayOneShot(currentClip.audio);
    }

    public void PlaySound(string audioClip, float pitch = 1)
    {
        var currentClip = clips.Find(c => c.name == audioClip);
        if (soundEffectSource.isPlaying)
        {
            if (currentClip.priority > soundEffectSource.priority)
            {
                soundEffectSource.Stop();
                soundEffectSource.clip = currentClip.audio;
                soundEffectSource.priority = currentClip.priority;
                soundEffectSource.pitch = pitch;
                soundEffectSource.Play();
            }
        }
        else
        {
            soundEffectSource.clip = currentClip.audio;
            soundEffectSource.priority = currentClip.priority;
            soundEffectSource.pitch = pitch;
            soundEffectSource.Play();
        }
    }
    public void StopSound()
    {
		if (soundEffectSource.isPlaying)
			soundEffectSource.Stop();
	}

    public void PlayMusic(string audioClip, int pitch = 1)
    {
        var currentClip = musics.Find(c => c.name == audioClip);
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
        musicSource.clip = currentClip.audio;
        musicSource.pitch = pitch;
        musicSource.Play();
    }

    public void StopMusic()
    {
        currentIndex = -1;
        StopAllCoroutines();
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
        else if (newMusicSource.isPlaying)
        {
            musicSource.clip = newMusicSource.clip;
            newMusicSource.clip = null;
			newMusicSource.Stop();
        }
	}

    private IEnumerator FadeMusic(Clip newClip, float fadeTime)
    {
        //yield return new WaitForSeconds(3);
        float timeToFade = fadeTime;
        float timeElapsed = 0;
		if (musicSource.isPlaying)
		{
			newMusicSource.clip = newClip.audio;
			newMusicSource.timeSamples = musicSource.timeSamples;
            newMusicSource.Play();
            while (timeElapsed < timeToFade)
            {
                newMusicSource.volume = Mathf.Lerp(0, volumeMusic, timeElapsed / timeToFade);
                musicSource.volume = Mathf.Lerp(volumeMusic, 0, timeElapsed / timeToFade);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            musicSource.Stop();
        }
        else
		{
			musicSource.clip = newClip.audio;
            musicSource.timeSamples = newMusicSource.timeSamples;
            musicSource.Play();
            while (timeElapsed < timeToFade)
            {
                musicSource.volume = Mathf.Lerp(0, volumeMusic, timeElapsed / timeToFade);
                newMusicSource.volume = Mathf.Lerp(volumeMusic, 0, timeElapsed / timeToFade);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            newMusicSource.Stop();
        }
    }

    public void PlayMusicWithFade(string audioClip, float timeToFade = .1f)
    {
        StopAllCoroutines();
        var newClip = musics.Find(c => c.name == audioClip);
        StartCoroutine(FadeMusic(newClip, timeToFade));
    }

    private int currentIndex = 4;
    public void PlayMusicByZoomIndex(int index, float timeToFade = .1f)
    {
        if(currentIndex == index) return;
        currentIndex = index;
        switch (index)
        {
            case 0: PlayMusicWithFade("space", timeToFade); break;
            case 1: PlayMusicWithFade("earth", timeToFade); break;
            case 2: PlayMusicWithFade("building", timeToFade); break;
            case 3: PlayMusicWithFade("house", timeToFade); break;
            case 4: PlayMusicWithFade("human", timeToFade); break;
            case 5: PlayMusicWithFade("human", timeToFade); break;
            case 6: PlayMusicWithFade("hairs", timeToFade); break;
            case 7: PlayMusicWithFade("cells", timeToFade); break;
		}
    }

    public void ChangeVolumeMusic(float volume)
    {
        musicSource.volume = volume;
        newMusicSource.volume = volume;
    }
    public void ChangeVolumeSound(float volume)
        => soundEffectSource.volume = volume;
}

