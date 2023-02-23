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
        upgradeText.text = " Upgrade x " + GameManager.Ins.upgradeList.upgrade[upgradeCount].star;
        manaIncreased = GameManager.Ins.upgradeList.upgrade[upgradeCount].mana;
        manaIncreasedText.text = " " + manaIncreased;
        totalMana = PlayerPrefs.GetInt("Total Mana",10);
        totalManaText.text = "" + PlayerPrefs.GetInt("Total Mana",10);
        starOwns = PlayerPrefs.GetInt("Total Stars");
        starOwnsText.text = " x " + PlayerPrefs.GetInt("Total Stars");
    }

    // Update is called once per frame
    
    public void Upgrade()
    {
        if (starOwns >= GameManager.Ins.upgradeList.upgrade[upgradeCount].star && starOwns > 0)
        {
            starOwns -= GameManager.Ins.upgradeList.upgrade[upgradeCount].star;
            totalUpgrade += GameManager.Ins.upgradeList.upgrade[upgradeCount].star;
            PlayerPrefs.SetInt("Total Upgrade", totalUpgrade);
            totalMana += GameManager.Ins.upgradeList.upgrade[upgradeCount].mana;
            PlayerPrefs.SetInt("Total Mana",totalMana);
            if (upgradeCount < GameManager.Ins.upgradeList.upgrade.Length - 1)
            {
                upgradeCount += 1;
            }
            else
            {
                upgradeCount = GameManager.Ins.upgradeList.upgrade.Length - 1;
            }
            PlayerPrefs.SetInt("The number of upgrades", upgradeCount);
        }
    }
}
