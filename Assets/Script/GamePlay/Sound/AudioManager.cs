using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    private static AudioManager audioManager;
    public static  AudioManager AudioManger {get => audioManager;}
    
    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;
    private bool mutedSFX = false;
    private bool mutedMusic = false;
    public Text musicText, sfxText;
    public string musicStatus, sfxStatus, nameAudioClipAssetBundle;
    private int count;

    public AudioClip audioNULL;
    
    private void Awake()
    {
        if (audioManager != null)
        {
            Debug.LogError("Only 1 Object allow to exist");
        }
        audioManager = this;
    }

    public void LoadAudioAssetBundle()
    {
        if (AssetBundleManager.audioClipArray != null)
        {
            musicSounds = new Sound[AssetBundleManager.audioClipArray.Length];
            sfxSounds = new Sound[AssetBundleManager.audioClipArray.Length];
            for (int i = 0; i < musicSounds.Length; i++)
            {
                musicSounds[i] = new Sound();
                sfxSounds[i] = new Sound();
                
                nameAudioClipAssetBundle = Path.GetFileNameWithoutExtension(AssetBundleManager.audioClipArray[i]);
                
                musicSounds[i].InputSound(nameAudioClipAssetBundle, AssetBundleManager.audioClipList[i]);
                sfxSounds[i].InputSound(nameAudioClipAssetBundle, AssetBundleManager.audioClipList[i]);
                count += 1;
            }

            if (count == musicSounds.Length)
            {
                PlayMusic("soundbackground_2");
                musicSource.mute = mutedMusic;
                sfxSource.mute = mutedSFX;
            }
            
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
        }
    }

    public void PlayMusic(string name)
    {
        Sound soundArray = Array.Find(musicSounds, x => x.nameSound == name);
        if (soundArray == null)
        {
            musicSource.clip = audioNULL;
        }
        else
        {
            musicSource.clip = soundArray.SoundAssetBundle;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlaySFX(string name)
    {
        try
        {
            Sound soundArray = Array.Find(sfxSounds, x => x.nameSound == name);
            if (soundArray == null)
            {
                
            }
            else
            {
                sfxSource.PlayOneShot(soundArray.SoundAssetBundle);
            }
        }
        catch (Exception e)
        {
            sfxSource.PlayOneShot(audioNULL);
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
