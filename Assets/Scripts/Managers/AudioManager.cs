using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioSource bgmAudioSource;
    public AudioSource sfxAudioSource;

    public List<AudioClip> bgmAudioClips = new List<AudioClip>();
    public List<AudioClip> sfxAudioClips = new List<AudioClip>();

    public Dictionary<string, AudioClip> bgmTables = new Dictionary<string, AudioClip>();
    public Dictionary<string, AudioClip> sfxTables = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        else
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }
        }
    }

    private void Start()
    {
        for (int i = 0; i < sfxAudioClips.Count; i++)
        {
            sfxTables.Add(sfxAudioClips[i].name, sfxAudioClips[i]);
        }

        for (int i = 0; i < bgmAudioClips.Count; i++)
        {
            bgmTables.Add(bgmAudioClips[i].name, bgmAudioClips[i]);
        }
    }

    public void PlaySFX(string sfxName)
    {
        AudioClip audioClip = sfxTables[sfxName];
        sfxAudioSource.loop = false;
        sfxAudioSource.PlayOneShot(audioClip);
    }

    public void PlayBGM(string sfxName)
    {
        AudioClip audioClip = bgmTables[sfxName];
        bgmAudioSource.loop = false;
        bgmAudioSource.PlayOneShot(audioClip);
    }


}
