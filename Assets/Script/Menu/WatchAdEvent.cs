using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchAdEvent : MonoBehaviour
{
    private int starReward = 1;
    public int boosterReward = 1;
    private int boosterTotal;
    public bool checkMultiply2Stars ;
    private static WatchAdEvent instance;
    public static WatchAdEvent Instance
    {
        get => instance;
    }

    private void Awake()
    {
        if (instance != null)
        {
            
        }

        instance = this;
    }

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
        Debug.Log("them booster");
        boosterTotal = PlayerPrefs.GetInt("Booster total");
        boosterTotal += boosterReward;
        PlayerPrefs.SetInt("Booster total", boosterTotal);
    }

    public void PressWatchAdStarButton()
    {
        Debug.Log("thuong sao");
        int temp = PlayerPrefs.GetInt("Star by Ads") + starReward;
        Debug.Log(temp +" " + starReward);
        PlayerPrefs.SetInt("Star by Ads", temp);
    }

    public void PressWatchAdsDoubleStars()
    {
        Debug.Log("nhan doi sao");
        checkMultiply2Stars = true;
    }
}