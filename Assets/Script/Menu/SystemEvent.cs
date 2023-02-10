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
    public ReadJSON readJson;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("The number of upgrades"))
        {
            PlayerPrefs.SetInt("The number of upgrades",0);
        }
    }

    private void Update()
    {
        upgradeCount = PlayerPrefs.GetInt("The number of upgrades");
        starsToUpgrade = PlayerPrefs.GetInt("Star To Upgrade",1);
        upgradeText.text = " Upgrade x " + readJson.upgradeList.upgrade[upgradeCount].star;
        manaIncreased = readJson.upgradeList.upgrade[upgradeCount].mana;
        manaIncreasedText.text = " " + manaIncreased;
        totalMana = PlayerPrefs.GetInt("Total Mana",10);
        totalManaText.text = "" + PlayerPrefs.GetInt("Total Mana",10);
        starOwns = PlayerPrefs.GetInt("Total Stars");
        starOwnsText.text = " x " + PlayerPrefs.GetInt("Total Stars");
    }

    // Update is called once per frame
    
    public void Upgrade()
    {
        if (starOwns >= readJson.upgradeList.upgrade[upgradeCount].star && starOwns > 0)
        {
            starOwns -= readJson.upgradeList.upgrade[upgradeCount].star;
            totalUpgrade += readJson.upgradeList.upgrade[upgradeCount].star;
            PlayerPrefs.SetInt("Total Upgrade", totalUpgrade);
            totalMana += readJson.upgradeList.upgrade[upgradeCount].mana;
            PlayerPrefs.SetInt("Total Mana",totalMana);
            if (upgradeCount < readJson.upgradeList.upgrade.Length - 1)
            {
                upgradeCount += 1;
            }
            else
            {
                upgradeCount = readJson.upgradeList.upgrade.Length - 1;
            }
            PlayerPrefs.SetInt("The number of upgrades", upgradeCount);
        }
    }
}
