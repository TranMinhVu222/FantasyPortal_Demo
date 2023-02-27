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
            if (PlayerPrefs.GetInt("Present Level") + 1 == PlayerPrefs.GetInt("levelsUnlocked"))
            {
                if (PlayerPrefs.GetInt("levelsUnlocked") < ReadJSON.scene.Length)
                {
                    PlayerPrefs.SetInt("levelsUnlocked",currentLevel + 1);    
                }
                int countStarList = PlayerPrefs.GetInt("Star List Level Count");
                PlayerPrefs.SetInt("Star List Level Count",countStarList + 1);
            }    
        }
    }
}
