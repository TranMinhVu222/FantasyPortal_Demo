using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuFTUE : MonoBehaviour
{
    public float pointerSpeed;
    public Image pointer, tutorialPanel;
    public Text tutorialText, tapToNext;
    public GameObject systemButton, totalMana, nextUpgrade, upgradeButton, boosterButton, exitButton, level1Panel, systemPanel,
        levelMenu,levelButton,holdingPanel, watchAdsButton, startButton, optionButton, storeButton, exitGameButton,exitLevelPanel, exitLevel1, nextButton;
    public State currentState = State.SystemButton;

    // Start is called before the first frame update
    
    public enum State
    {
        SystemButton,
        TotalMana,
        NextUpgrade,
        UpgradeButton,
        ExitButton,
        StartButton,
        LevelButton,
        BoosterButton,
        Done
    }
    void Start()
    {
        startButton.SetActive(false);
        optionButton.SetActive(false);
        storeButton.SetActive(false);
        exitGameButton.SetActive(false);
        pointer.gameObject.SetActive(false);
        tutorialPanel.gameObject.SetActive(false);
        nextButton.SetActive(false);
        exitLevel1.SetActive(false);
        exitLevelPanel.SetActive(false);
        currentState = State.SystemButton;
    }

    // Update is called once per frame
    void Update()
    {
        if (!holdingPanel.activeSelf)
        {
            pointer.gameObject.SetActive(true);
            tutorialPanel.gameObject.SetActive(true);
        }
        switch (currentState)
        { 
            case State.SystemButton:
                exitButton.SetActive(false);
                watchAdsButton.SetActive(false);
                pointer.transform.position = Vector3.MoveTowards(pointer.transform.position, 
                    systemButton.transform.position, pointerSpeed * Time.deltaTime);
                tutorialText.text = "Click here to know about Mana System";
                if (!holdingPanel.activeSelf)
                {
                    tutorialPanel.gameObject.SetActive(true);
                    if (systemPanel.activeSelf)
                    {
                        tapToNext.gameObject.SetActive(false);
                        tutorialPanel.gameObject.SetActive(false);
                        ChangeState(State.TotalMana);
                    }
                }
                
                break;
            case State.TotalMana:
                pointer.transform.position = Vector3.MoveTowards(pointer.transform.position, 
                    totalMana.transform.position, pointerSpeed * Time.deltaTime);
                if (pointer.transform.position == totalMana.transform.position)
                {
                    tutorialText.text = "This is your mana";
                    tutorialPanel.gameObject.SetActive(true);
                    tapToNext.gameObject.SetActive(true);
                    if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
                    {
                        tapToNext.gameObject.SetActive(false);
                        tutorialPanel.gameObject.SetActive(false);
                        ChangeState(State.NextUpgrade);
                    }
                }
                break;
            case State.NextUpgrade:
                pointer.transform.position = Vector3.MoveTowards(pointer.transform.position, 
                    nextUpgrade.transform.position, pointerSpeed * Time.deltaTime);
                if (pointer.transform.position == nextUpgrade.transform.position)
                {
                    tutorialText.text = "This is the mana you gain when upgrade";
                    tutorialPanel.gameObject.SetActive(true);
                    tapToNext.gameObject.SetActive(true);
                    if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
                    {
                        tapToNext.gameObject.SetActive(false);
                        tutorialPanel.gameObject.SetActive(false);
                        ChangeState(State.UpgradeButton);
                    }
                }
                break;
            case State. UpgradeButton:
                pointer.transform.position = Vector3.MoveTowards(pointer.transform.position, 
                    upgradeButton.transform.position, pointerSpeed * Time.deltaTime);
                if (pointer.transform.position == upgradeButton.transform.position)
                {
                    tutorialText.text = "UpgradeButton";
                    tutorialPanel.gameObject.SetActive(true);
                    tapToNext.gameObject.SetActive(true);
                    if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
                    {
                        tapToNext.gameObject.SetActive(false);
                        tutorialPanel.gameObject.SetActive(false);
                        ChangeState(State.ExitButton);
                    }
                }
                break;
            case State.ExitButton:
                exitButton.SetActive(true);
                pointer.transform.position = Vector3.MoveTowards(pointer.transform.position, 
                    exitButton.transform.position, pointerSpeed * Time.deltaTime);
                if (pointer.transform.position == exitButton.transform.position)
                {
                    tutorialText.text = "Click here";
                    tutorialPanel.gameObject.SetActive(true);
                    if (!systemPanel.activeSelf)
                    {
                        tutorialPanel.gameObject.SetActive(false);
                        ChangeState(State.StartButton);
                    }
                }
                break;
            case State.StartButton:
                startButton.SetActive(true);
                pointer.transform.position = Vector3.MoveTowards(pointer.transform.position, 
                    startButton.transform.position, pointerSpeed * Time.deltaTime);
                if (pointer.transform.position == startButton.transform.position)
                {
                    if (levelMenu.activeSelf)
                    {
                        ChangeState(State.LevelButton);
                    }
                }
                break;
            case State.LevelButton:
                pointer.transform.position = Vector3.MoveTowards(pointer.transform.position, 
                    levelButton.transform.position, pointerSpeed * Time.deltaTime);
                if (level1Panel.activeSelf)
                {
                    ChangeState(State.BoosterButton);
                }
                break;
            case State.BoosterButton:
                pointer.gameObject.SetActive(true);
                pointer.transform.position = Vector3.MoveTowards(pointer.transform.position, 
                    boosterButton.transform.position, pointerSpeed * Time.deltaTime);
                if (pointer.transform.position == boosterButton.transform.position)
                {
                    tutorialText.text = "This is a mana booster, it will help you to increase X2 the amount of mana you get when you pick up Magic Shards in this level";
                    tutorialPanel.gameObject.SetActive(true);
                    if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
                    {
                        optionButton.SetActive(true);
                        storeButton.SetActive(true);
                        exitGameButton.SetActive(true);
                        pointer.gameObject.SetActive(false);
                        tapToNext.gameObject.SetActive(false);
                        tutorialPanel.gameObject.SetActive(false);
                        nextButton.SetActive(true);
                        exitLevel1.SetActive(true);
                        exitLevelPanel.SetActive(true);
                        PlayerPrefs.SetInt("Complete Menu FTUE",1);
                        ChangeState(State.Done);
                    }
                }
                break;
            case State.Done:
                pointer.gameObject.SetActive(false);
                tutorialPanel.gameObject.SetActive(false);
                break;
            default:
                return;   
        }
    }

    private void ChangeState(State state)
    {
        if (state == currentState) return;
        currentState = state;
        switch (state)
        {
            case State.SystemButton:
                break;
            case State.TotalMana:
                
                break;
            case State.NextUpgrade:
                
                break;
            case State. UpgradeButton:
                
                break;
            case State.ExitButton:
                
                break;
            case State.BoosterButton:
                
                break;
            case State.StartButton:
                
                break;
            case State.LevelButton:
                
                break;
            case State.Done:
                
                break;
            default:
                return;   
        }
    }

    public void NextState(State state)
    {
        ChangeState(state);
    }
}
