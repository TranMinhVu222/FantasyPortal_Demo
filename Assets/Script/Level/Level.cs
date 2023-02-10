using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    public void Pass()
    {
        if (PlayerPrefs.GetInt("Completed FTUE") == 1)
        {
            int currentLevel = SceneManager.GetActiveScene().buildIndex;
            if (currentLevel >= PlayerPrefs.GetInt("levelsUnlocked"))
            {
                PlayerPrefs.SetInt("levelsUnlocked",currentLevel);
                int countStarList = PlayerPrefs.GetInt("Star List Level Count");
                PlayerPrefs.SetInt("Star List Level Count",countStarList + 1);
            }    
        }
    }
}
