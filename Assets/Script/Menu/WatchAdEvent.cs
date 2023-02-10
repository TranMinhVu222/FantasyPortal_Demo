using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchAdEvent : MonoBehaviour
{
    public int starReward;
    public int boosterReward = 1;
    private int boosterTotal;
    public bool checkMultiply2Stars ;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("Star by Ads"))
        {
            PlayerPrefs.SetInt("Star by Ads",0);
        }
        checkMultiply2Stars = false;
    }

    public void PressWatchAdButton()
    {
        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            boosterTotal = PlayerPrefs.GetInt("Booster total");
            boosterTotal += boosterReward;
            PlayerPrefs.SetInt("Booster total", boosterTotal);    
        }
    }

    public void PressWatchAdStarButton()
    {
        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            PlayerPrefs.SetInt("Star by Ads", PlayerPrefs.GetInt("Star by Ads") + starReward);
        }
    }

    public void PressWatchAdsDoubleStars()
    {
        if (PlayerPrefs.GetInt("Completed FTUE") == 1)
        {
            checkMultiply2Stars = true;    
        }
    }
}