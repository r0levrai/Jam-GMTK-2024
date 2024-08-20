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
    void Start()
    {
        PlayMusic("main");
        PlayMusicWithFade("main2");
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
        if (musicSource.isPlaying)
        {
			StopAllCoroutines();
			musicSource.Stop();
        }
	}

    private IEnumerator FadeMusic(Clip newClip, float fadeTime)
    {
        yield return new WaitForSeconds(3);
        float timeToFade = fadeTime;
        float timeElapsed = 0;

        float maxVolume = newMusicSource.volume;

		if (musicSource.isPlaying)
        {
            newMusicSource.clip = newClip.audio;
            newMusicSource.Play();
            while (timeElapsed < timeToFade)
            {
                newMusicSource.volume = Mathf.Lerp(0, maxVolume, timeElapsed / timeToFade);
                musicSource.volume = Mathf.Lerp(maxVolume, 0, timeElapsed / timeToFade);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            musicSource.Stop();
        }
        else
        {
            musicSource.clip = newClip.audio;
            musicSource.Play();
            while (timeElapsed < timeToFade)
            {
                musicSource.volume = Mathf.Lerp(0, maxVolume, timeElapsed / timeToFade);
                newMusicSource.volume = Mathf.Lerp(maxVolume, 0, timeElapsed / timeToFade);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            newMusicSource.Stop();
        }
    }

    public void PlayMusicWithFade(string audioClip, float timeToFade = 3)
    {
        StopAllCoroutines();
        var newClip = musics.Find(c => c.name == audioClip);
        StartCoroutine(FadeMusic(newClip, timeToFade));
    }

    public void PlayMusicByZoomIndex(int index, float timeToFade = 3)
    {
        switch (index)
        {
            case 0: PlayMusicWithFade("main", timeToFade); break;
            case 1: PlayMusicWithFade("main2", timeToFade); break;
            case 2: PlayMusicWithFade("main", timeToFade); break;
		}
    }

}

