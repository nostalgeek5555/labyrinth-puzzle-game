using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SettingManager : MonoBehaviour
{
    public TextMeshProUGUI volumeText;
    public float[] volumes;
    public AudioSource[] audioSources;
    public GameTime[] gameTimes;

    [System.Serializable]
    public struct GameTime
    {
        public int time;
        public GameObject hover;
    }

    int volume
    {
        get
        {
            return PlayerPrefs.GetInt("VOLUME", 9);
        }
        set
        {
            PlayerPrefs.SetInt("VOLUME", value);
        }
    }
    int gameTime
    {
        get
        {
            return PlayerPrefs.GetInt("GAME_TIME", 1);
        }
        set
        {
            PlayerPrefs.SetInt("GAME_TIME", value);
        }
    }

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        SetVolume();
        SetGameTime();
    }

    void SetVolume()
    {
        foreach (AudioSource a in audioSources)
        {
            float v = volumes[volume] / 100f;
            a.volume = v;
        }
        volumeText.text = volumes[volume].ToString() + "%";
    }

    void SetGameTime()
    {
        foreach (GameTime g in gameTimes)
            g.hover.SetActive(false);
        gameTimes[gameTime].hover.SetActive(true);
        GameManager.GameTime = gameTimes[gameTime].time;
    }

    public void SetGameTime(int index)
    {
        gameTime = index;
        SetGameTime();
    }

    public void SetVolumeGame(int index)
    {
        volume += index;
        volume = volume >= volumes.Length ? volumes.Length - 1 : volume < 0 ? 0 : volume;
        SetVolume();
    }
}
