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
            int currentLevel =  PlayerPrefs.GetInt("levelsUnlocked");
            if (PlayerPrefs.GetInt("Present Level") == PlayerPrefs.GetInt("levelsUnlocked"))
            {
                if (PlayerPrefs.GetInt("levelsUnlocked") < LevelManager.totalLevel)
                {
                    PlayerPrefs.SetInt("levelsUnlocked",currentLevel + 1);    
                }
                int countStarList = PlayerPrefs.GetInt("Star List Level Count");
                PlayerPrefs.SetInt("Star List Level Count",countStarList + 1);
            }    
        }
    }
}
