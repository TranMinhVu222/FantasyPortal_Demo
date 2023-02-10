using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISound : MonoBehaviour
{
    public AudioManager audioManager;
    public void ToggleMusic()
    {
        audioManager.ToggleMusic();
    }

    public void ToggleSFX()
    {
        audioManager.ToggleSFX();
    }
}
