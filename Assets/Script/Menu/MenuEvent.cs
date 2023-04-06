using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class MenuEvent : MonoBehaviour
{
    
    private double progress, totalBytes, bytes;
    public Text downloadingText, progressPercent;
    public Image progressBar;
    public GameObject warningPanel;
    private int starOwns;
    public Text starOwnsText;
    public int starTool, tempMana, tempUp, starAds,boosterTotal;
    public GameObject loadingPanel,holdingPanel, FTUE, startButton, optionButton, storeButton, exitGameButton, watchAds;
    public bool effect, checkDone;
    public CanvasGroup uiElement;
    private void Awake()
    {
        loadingPanel.SetActive(true);
        if (!PlayerPrefs.HasKey("Present Level"))
        {
            PlayerPrefs.SetInt("Present Level", 0);
        }
    }

    public void LoadLevel(int index)
    {
        PlayerPrefs.SetInt("Present Level",index);
        AssetBundleManager.Instance.LoadScene(index);
        if (PlayerPrefs.GetInt("Used booster") == 2)
        {
            boosterTotal = PlayerPrefs.GetInt("Booster total");
            PlayerPrefs.SetInt("Booster total",boosterTotal - 1);
        }
    }

    private void Start()
    {
        //-----Start DownLoad--------
        checkDone = false;
        starTool = 0;
        for (int i = 1; i <= PlayerPrefs.GetInt("Star List Level Count"); i++)
        {
            starOwns += PlayerPrefs.GetInt("Subtotal level Star" + i);
        }
        tempMana = 10;
        tempUp = 1;
        if (!PlayerPrefs.HasKey("Total Mana") && !PlayerPrefs.HasKey("Star To Upgrade"))
        {
            PlayerPrefs.SetInt("Total Mana", tempMana);
            PlayerPrefs.SetInt("Star To Upgrade", tempUp);
        }
        if (PlayerPrefs.GetInt("Complete Menu FTUE") == 1)
        {
            FTUE.SetActive(false);
            startButton.SetActive(true);
            optionButton.SetActive(true);
            storeButton.SetActive(true);
            watchAds.SetActive(true);
            exitGameButton.SetActive(true);
        }
        effect = false;
        holdingPanel.SetActive(true);
    }

    private void Update()
    {
        if (loadingPanel.activeInHierarchy)
        {
            if (!PlayerPrefs.HasKey("Completed FTUE") || PlayerPrefs.GetInt("Completed FTUE") == 0)
            {
                effect = false;
            }
            else
            {
                effect = true;   
            }
        }

        if (effect)
        {
            StartCoroutine(FadeCanvasGroup(uiElement, uiElement.alpha, 0));
        }

        if (uiElement.alpha == 0)
        {
            holdingPanel.SetActive(false);
        }
        if (starTool == 0)
        {
            PlayerPrefs.SetInt("Total Stars",starOwns - PlayerPrefs.GetInt("Total Upgrade") + PlayerPrefs.GetInt("Star by Ads") + PlayerPrefs.GetInt("Star Purchase"));
            starOwnsText.text = " x" + PlayerPrefs.GetInt("Total Stars");
        }
      
        //TOOL: Up star
        else
        {
            PlayerPrefs.SetInt("Total Stars", starTool - PlayerPrefs.GetInt("Total Upgrade") + PlayerPrefs.GetInt("Star by Ads") + PlayerPrefs.GetInt("Star Purchase"));
            starOwnsText.text = " x" + PlayerPrefs.GetInt("Total Stars");
        }
        //--------------------------
    }
    IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float start, float end, float lerpTime = 1f)
    {
        float timeStartedLerp = Time.time;
        float timeSinceStarted = Time.time - timeStartedLerp;
        float percentageComplete = timeSinceStarted / lerpTime;
        while (true)
        {
            timeSinceStarted = Time.time - timeStartedLerp;
            percentageComplete = timeSinceStarted / lerpTime;

            float currentValue = Mathf.Lerp(start, end, percentageComplete);
            
            canvasGroup.alpha = currentValue;

            if (percentageComplete >= 1)
            {
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        progressBar.fillAmount = 1f;
        progressPercent.text = 100 + "%";
        checkDone = true ;
        effect = false;
    }
}