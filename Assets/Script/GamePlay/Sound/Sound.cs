using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Sound
{
    private string nameState;
    private AudioClip clip;
    public string nameSound { get => nameState; set => nameState = value;}
    public AudioClip SoundAssetBundle { get => clip; set => clip = value;}

    public void InputSound(string name, AudioClip sound)
    {
        nameState = name;
        clip = sound;
    }
}
