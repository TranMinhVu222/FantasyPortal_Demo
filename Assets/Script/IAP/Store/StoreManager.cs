using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
    private int starPurchase;
    private void Awake()
    {
        if (!PlayerPrefs.HasKey("Star Purchase"))
        {
            PlayerPrefs.SetInt("Star Purchase",0);
        }
        starPurchase = PlayerPrefs.GetInt("Star Purchase");
    }

    public void On2StarsPurchaseComplete()
    {
        starPurchase += 2;
        PlayerPrefs.SetInt("Star Purchase",starPurchase);
    }
    public void On20StarsPurchaseComplete()
    {
        starPurchase += 20;
        PlayerPrefs.SetInt("Star Purchase",starPurchase);
    }
    public void On50StarsPurchaseComplete()
    {
        starPurchase += 50;
        PlayerPrefs.SetInt("Star Purchase",starPurchase);
    }
}
