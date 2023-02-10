using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;
    private bool mutedSFX = false;
    private bool mutedMusic = false;
    public Text musicText, sfxText;
    public string musicStatus, sfxStatus;
    private void Start()
    {
       PlayMusic("SoundBackground");
       if (!PlayerPrefs.HasKey("Muted Music"))
       {
           PlayerPrefs.SetInt("Muted Music",0);
           LoadMusic();
           musicText.text = "On";
           PlayerPrefs.SetString("Music Status","On");
       }
       if (!PlayerPrefs.HasKey("Muted SFX"))
       {
           PlayerPrefs.SetInt("Muted SFX",0);
           LoadSFX();
           sfxText.text = "On";
           PlayerPrefs.SetString("SFX Status", "On");
       }
       else
       {
           LoadMusic();
           LoadSFX();
       }
       musicText.text = PlayerPrefs.GetString("Music Status");
       sfxText.text = PlayerPrefs.GetString("SFX Status");
       musicSource.mute = mutedMusic;
       sfxSource.mute = mutedSFX;
    }

    public void PlayMusic(string name)
    {
        Sound soundArray = Array.Find(musicSounds, x => x.nameState == name);
        if (soundArray == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            musicSource.clip = soundArray.clip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlaySFX(string name)
    {
        Sound soundArray = Array.Find(sfxSounds, x => x.nameState == name);
        if (soundArray == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            sfxSource.PlayOneShot(soundArray.clip);
        }
    }

    public void ToggleMusic()
    {
        if (mutedMusic == false)
        {
            mutedMusic = true;
            musicSource.mute = true;
            musicText.text = "Off";
            musicStatus = "Off";
        }
        else
        {
            mutedMusic = false;
            musicSource.mute = false;
            musicText.text = "On";
            musicStatus = "On";
        }
        SaveMusic();
    }

    public void ToggleSFX()
    {
        if (mutedSFX == false)
        {
            mutedSFX = true;
            sfxSource.mute = true;
            sfxText.text = "Off";
            sfxStatus = "Off";
        }
        else
        {
            mutedSFX = false;
            sfxSource.mute = false;
            sfxText.text = "On";
            sfxStatus = "On";
        }
        SaveSFX();
    }
    private void LoadMusic()
    {
        mutedMusic = PlayerPrefs.GetInt("Muted Music") == 1;
    }

    private void SaveMusic()
    {
        PlayerPrefs.SetInt("Muted Music", mutedMusic ? 1 : 0);
        PlayerPrefs.SetString("Music Status",musicStatus);
    }
    private void LoadSFX()
    {
        mutedSFX = PlayerPrefs.GetInt("Muted SFX") == 1;
    }

    private void SaveSFX()
    {
        PlayerPrefs.SetInt("Muted SFX", mutedSFX ? 1 : 0);
        PlayerPrefs.SetString("SFX Status",sfxStatus);
    }
}
