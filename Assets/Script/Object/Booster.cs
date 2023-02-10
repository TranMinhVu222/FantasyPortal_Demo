using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class Booster : MonoBehaviour
{
    public int booster;
    private int boosterTotal;
    public Text boosterTotalText;
    public Button useBooster, noUseBooster;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("Used booster"))
        {
            PlayerPrefs.SetInt("Used booster", 1);
        }

        if (!PlayerPrefs.HasKey("Booster total"))
        {
            PlayerPrefs.SetInt("Booster total", 0);
        }

        boosterTotalText.text = "" + boosterTotal;
    }

    public void Update()
    {
        boosterTotalText.text = "" + PlayerPrefs.GetInt("Booster total");
        boosterTotal = PlayerPrefs.GetInt("Booster total");
        Debug.Log(PlayerPrefs.GetInt("Used booster"));
    }

    public void UseBooster()
    {
        if (PlayerPrefs.GetInt("Booster total") > 0)
        {
            booster = 2;
            PlayerPrefs.SetInt("Used booster", booster);
            useBooster.gameObject.SetActive(true);
            noUseBooster.gameObject.SetActive(false);
        }
    }

    public void NoUseBooster()
    {
        PlayerPrefs.SetInt("Used booster", 1);
        useBooster.gameObject.SetActive(false);
        noUseBooster.gameObject.SetActive(true);
    }
}
