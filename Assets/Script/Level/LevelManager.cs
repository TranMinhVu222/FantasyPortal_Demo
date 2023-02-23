using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public int levelsUnlocked;

    public Button[] buttons;
    public GameObject[] locks;
    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("Star List Level Count"))
        {
            PlayerPrefs.SetInt("Star List Level Count", 1);
        }
        if (!PlayerPrefs.HasKey("levelsUnlocked"))
        {
            PlayerPrefs.SetInt("levelsUnlocked", 1);
        }
        levelsUnlocked = PlayerPrefs.GetInt("levelsUnlocked");
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }

        for (int i = 0; i < levelsUnlocked; i++)
        {
            buttons[i].interactable = true;
            locks[i].SetActive(false);
        }
    }

    public void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }
}
