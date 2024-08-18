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

    public List<Clip> clips = new List<Clip>();
    public List<Clip> musics = new List<Clip>();
    public List<Source> sources = new List<Source>();
    void Start()
    {
        PlayMusic("main");
        PlayMusicWithFade("main2");
    }

    public void PlaySound(string audioClip, float pitch = 1)
    {
        var currentClip = clips.Find(c => c.name == audioClip);
        //Debug.Log(soundEffectSource.isPlaying);
        if (soundEffectSource.isPlaying)
        {
            if (currentClip.priority >= soundEffectSource.priority)
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

    private IEnumerator FadeMusic(Clip newClip, float fadeTime)
    {
        yield return new WaitForSeconds(3);
        float timeToFade = fadeTime;
        float timeElapsed = 0;

        if (musicSource.isPlaying)
        {
            newMusicSource.clip = newClip.audio;
            newMusicSource.Play();
            while (timeElapsed < timeToFade)
            {
                newMusicSource.volume = Mathf.Lerp(0, 1, timeElapsed / timeToFade);
                musicSource.volume = Mathf.Lerp(1, 0, timeElapsed / timeToFade);
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
                musicSource.volume = Mathf.Lerp(0, 1, timeElapsed / timeToFade);
                newMusicSource.volume = Mathf.Lerp(1, 0, timeElapsed / timeToFade);
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

