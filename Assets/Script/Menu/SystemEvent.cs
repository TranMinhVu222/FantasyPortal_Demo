using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemEvent : MonoBehaviour
{
    // Start is called before the first frame update
    private int starOwns,totalMana,starsToUpgrade, totalUpgrade;
    public int manaIncreased, upgradeCount;
    public Text starOwnsText, totalManaText, upgradeText, manaIncreasedText;
    public GameObject watchAds;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("The number of upgrades"))
        {
            PlayerPrefs.SetInt("The number of upgrades",0);
        }
        if (PlayerPrefs.GetInt("Complete Menu FTUE") == 1)
        {
            watchAds.SetActive(true);
        }
            
    }

    private void Update()
    {
        upgradeCount = PlayerPrefs.GetInt("The number of upgrades");
        starsToUpgrade = PlayerPrefs.GetInt("Star To Upgrade",1);
        upgradeText.text = " Upgrade x " + DownloadDataGame.upgradeList.upgrade[upgradeCount].star;
        manaIncreased = DownloadDataGame.upgradeList.upgrade[upgradeCount].mana;
        manaIncreasedText.text = " " + manaIncreased;
        totalMana = PlayerPrefs.GetInt("Total Mana",10);
        totalManaText.text = "" + PlayerPrefs.GetInt("Total Mana",10);
        starOwns = PlayerPrefs.GetInt("Total Stars");
        starOwnsText.text = " x " + PlayerPrefs.GetInt("Total Stars");
    }

    // Update is called once per frame
    
    public void Upgrade()
    {
        if (starOwns >= DownloadDataGame.upgradeList.upgrade[upgradeCount].star && starOwns > 0)
        {
            starOwns -= DownloadDataGame.upgradeList.upgrade[upgradeCount].star;
            totalUpgrade += DownloadDataGame.upgradeList.upgrade[upgradeCount].star;
            PlayerPrefs.SetInt("Total Upgrade", totalUpgrade);
            totalMana += DownloadDataGame.upgradeList.upgrade[upgradeCount].mana;
            PlayerPrefs.SetInt("Total Mana",totalMana);
            if (upgradeCount < DownloadDataGame.upgradeList.upgrade.Length - 1)
            {
                upgradeCount += 1;
            }
            else
            {
                upgradeCount = DownloadDataGame.upgradeList.upgrade.Length - 1;
            }
            PlayerPrefs.SetInt("The number of upgrades", upgradeCount);
        }
    }
}
