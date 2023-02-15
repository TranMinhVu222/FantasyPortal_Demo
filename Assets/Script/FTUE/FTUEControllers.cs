using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.XR;
using System;
using System.Text;
using Unity.Mathematics;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class FTUEControllers : MonoBehaviour
{
    public float speedFill, pointerSpeed, height, timeSE, speedShow;
    protected float time;
    private bool moveRightButton, moveLeftButton, monsterQuest, magicQuest, upButton, nextMonsterTuto, checkNextGrab, checkTouch, 
        canMove, checks, checkss, destroyTarget;
    public Vector2 startVector, endVector;
    private int countNext, countGrab, countDestroy;
    public GameObject upPanel, movePanel, manaBar, monsterStatus, magicShardStatus, pauseButton,winPanel, noButton,target1,target2,target3, target4, 
        rightButton, leftButton, magicShards, grabButton, victoryZone, menuButton, unTrajectory,jumpPlant,exitMenu,nextButton,replayButton,watchAdsButton, enterPortal, entryPortal;
    public Image welcomeImg, blurImg, pointer, tutorialPanel;
    public Text TapToNext;
    // public Trajectory trajectory;
    public MagicShard magicShard;
    public PickUpItem pickUpItem;
    public ObstacleController monster;
    public PlayerController playerController;
    public Energy energy;
    
    public State currentState = State.Welcome;

    public IsTouchUI isTouchUI;
    public Text tutorialText;
    // Start is called before the first frame update
    private void Awake()
    {
        if (PlayerPrefs.GetInt("Completed FTUE") == 1)
        {
            SceneManager.UnloadSceneAsync(0);
        }

        if (!PlayerPrefs.HasKey("Complete Menu FTUE"))
        {
            PlayerPrefs.SetInt("Complete Menu FTUE",0);
        }
            
    }

    public enum State
    {
        Welcome,
        Button,
        CastSpell,
        ManaBar,
        MagicShard,
        Requirement,
        GrabObject,
        Monster,
        Win,
        ClickMenu
    }
    void Start()
    {
        TapToNext.gameObject.SetActive(false);
        currentState = State.Welcome;
        upPanel.SetActive(false);
        movePanel.SetActive(false);
        manaBar.SetActive(false);
        monsterStatus.SetActive(false);
        magicShardStatus.SetActive(false);
        pauseButton.SetActive(false);
        victoryZone.SetActive(false);
        moveRightButton = false;
        moveLeftButton = false;
        upButton = false;
        unTrajectory.SetActive(true);
        monsterQuest = false;
        magicQuest = false;
        monsterQuest = true;
        checkNextGrab = false;
        canMove = false;
        nextMonsterTuto = false;
        target1.SetActive(false);
        target2.SetActive(false);
        countDestroy = 1;
        if (!PlayerPrefs.HasKey("Completed FTUE"))
        {
            PlayerPrefs.SetInt("Completed FTUE",0);    
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            checkTouch = true;
            if (!EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId) && !unTrajectory.activeSelf && !canMove) 
            {
                isTouchUI.checkTouch = false;
            }
            else
            {
                isTouchUI.checkTouch = true;
                
            }
        }

        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Canceled && !unTrajectory.activeSelf && !canMove)
        {
            isTouchUI.checkTouch = false;
        }
        if (!EventSystem.current.IsPointerOverGameObject() && !unTrajectory.activeSelf && !checkTouch && !canMove)
        {
            isTouchUI.checkTouch = false;
        }
        
        switch (currentState)
        { 
            case State.Welcome:
                blurImg.fillAmount += speedFill * Time.deltaTime;
                if (blurImg.fillAmount == 1f)
                {
                    welcomeImg.fillAmount += speedFill * Time.deltaTime;
                    if (welcomeImg.fillAmount == 1f)
                    {
                        TapToNext.gameObject.SetActive(true);
                        if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
                        {
                            moveRightButton = true;
                            TapToNext.gameObject.SetActive(false);
                            ChangeState(State.Button);
                        }
                    }    
                }
                break;
            case State.Button:
                welcomeImg.fillAmount -= speedFill * Time.deltaTime;
                unTrajectory.SetActive(true);
                if (welcomeImg.fillAmount == 0)
                {
                    blurImg.fillAmount -= speedFill * Time.deltaTime;
                    if (blurImg.fillAmount == 0)
                    {
                        blurImg.gameObject.SetActive(false);
                        welcomeImg.gameObject.SetActive(false);
                        movePanel.SetActive(true);
                        upPanel.SetActive(true);
                        pointer.gameObject.SetActive(true);
                        if (moveRightButton)
                        {
                            tutorialText.text = "Use these buttons to move left and right";
                            pointer.transform.position = Vector3.MoveTowards(pointer.transform.position, 
                                rightButton.transform.position, pointerSpeed * Time.deltaTime);
                            if (pointer.transform.position == rightButton.transform.position)
                            {
                                tutorialPanel.gameObject.SetActive(true);
                                moveLeftButton = true;
                            }
                        }
                        if (moveLeftButton)
                        {
                            moveRightButton = false;
                            pointer.transform.position = Vector3.MoveTowards(pointer.transform.position, 
                                leftButton.transform.position, pointerSpeed * Time.deltaTime);
                            if (pointer.transform.position == leftButton.transform.position)
                            {
                                TapToNext.gameObject.SetActive(true);
                                if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
                                {
                                    TapToNext.gameObject.SetActive(false);
                                    upButton = true;
                                }
                            }
                        }

                        if (upButton)
                        {
                            tutorialText.text = "Use this button to jump up";
                            moveLeftButton = false;
                            pointer.transform.position = Vector3.MoveTowards(pointer.transform.position, 
                                upPanel.transform.position, pointerSpeed * Time.deltaTime);
                            if (pointer.transform.position == upPanel.transform.position)
                            {
                                TapToNext.gameObject.SetActive(true);
                                if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
                                {
                                    pointer.gameObject.SetActive(false);
                                    TapToNext.gameObject.SetActive(false);
                                    target1.SetActive(true);
                                    target2.SetActive(true);
                                    tutorialPanel.gameObject.SetActive(true);
                                    ChangeState(State.CastSpell);
                                }
                            }
                        }
                    }
                }
                break;
            case State.CastSpell:
                tutorialText.text = "Swipe the screen to shoot the energy to open the portal at the arrow position";
                pointer.gameObject.SetActive(true);
                time += Time.deltaTime;
                time %= timeSE;
                pointer.gameObject.transform.position = Parabola(startVector, endVector * 10f, height, time / timeSE);
                if (Input.touchCount > 1 ||  Input.GetMouseButton(0) || Input.GetMouseButtonDown(0))
                {
                    unTrajectory.SetActive(false);
                    tutorialPanel.gameObject.SetActive(false);
                    pointer.gameObject.SetActive(false);
                }

                if (!target1.activeSelf && !target2.activeSelf)
                {
                    TapToNext.gameObject.SetActive(true);
                    pointer.gameObject.SetActive(false);
                    unTrajectory.SetActive(true);
                    if (Input.touchCount > 2 || Input.GetMouseButtonDown(0))
                    {
                        TapToNext.gameObject.SetActive(false);
                        NextState(State.ManaBar);
                    }
                }
                break;
            case State.ManaBar:
                movePanel.SetActive(false);
                upPanel.SetActive(false);
                manaBar.SetActive(true);
                pointer.gameObject.SetActive(true);
                pointerSpeed = 30f;
                pointer.transform.position = Vector3.MoveTowards(pointer.transform.position, 
                    manaBar.transform.position, (pointerSpeed+30f) * Time.deltaTime);
                tutorialText.text = "This mana bar will decrease when you cast a spell. When your mana runs out, you can not cast spells";
                tutorialPanel.gameObject.SetActive(true);
                if (pointer.transform.position == manaBar.transform.position)
                {
                    TapToNext.gameObject.SetActive(true);
                    unTrajectory.SetActive(true);
                    if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
                    {
                        TapToNext.gameObject.SetActive(false);
                        pointer.gameObject.SetActive(false);
                        NextState(State.MagicShard);
                    }
                }
                break;
            case State.MagicShard:
                pointer.gameObject.SetActive(true);
                tutorialText.text = "Collect this Magic Shard to restore Mana";
                pointer.transform.position = Vector3.MoveTowards(pointer.transform.position, 
                    magicShards.transform.position, (pointerSpeed+20f) * Time.deltaTime);
                if (pointer.transform.position == magicShards.transform.position)
                {
                    movePanel.SetActive(true);
                    upPanel.SetActive(true);
                    tutorialText.text = "Go to the portal to teleport";
                    if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
                    {
                        unTrajectory.gameObject.SetActive(false);
                        tutorialPanel.gameObject.SetActive(false);
                    }
                    if (magicShard.completeMagicShard)
                    {
                        tutorialPanel.gameObject.SetActive(false);
                        pointer.gameObject.SetActive(false);
                        TapToNext.gameObject.SetActive(true);
                        unTrajectory.SetActive(true);
                        if (Input.touchCount > 1 || Input.GetMouseButtonDown(0))
                        {
                            TapToNext.gameObject.SetActive(false);
                            NextState(State.Requirement);
                        }    
                    }
                }
                break;
            case State.Requirement:
                magicShardStatus.SetActive(true);
                monsterStatus.SetActive(true);
                pointer.gameObject.SetActive(true);
                tutorialText.text = "This is the requirements that you need to finish";
                tutorialPanel.gameObject.SetActive(true);
                if (monsterQuest)
                {
                    pointer.transform.position = Vector3.MoveTowards(pointer.transform.position, 
                        monsterStatus.transform.position, pointerSpeed * 5f * Time.deltaTime);
                    if (pointer.transform.position == monsterStatus.transform.position)
                    {
                        TapToNext.gameObject.SetActive(true);
                        unTrajectory.SetActive(true);
                        if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
                        {
                            TapToNext.gameObject.SetActive(false);
                            magicQuest = true;
                        }
                    }
                }
                if (magicQuest)
                {
                    monsterQuest = false;
                    pointer.transform.position = Vector3.MoveTowards(pointer.transform.position, 
                        magicShardStatus.transform.position, pointerSpeed * Time.deltaTime);
                    if (pointer.transform.position == magicShardStatus.transform.position)
                    {
                        TapToNext.gameObject.SetActive(true);
                        unTrajectory.SetActive(true);
                        if (Input.touchCount > 1 || Input.GetMouseButtonDown(0))
                        {
                            tutorialPanel.gameObject.SetActive(false);
                            pointer.gameObject.SetActive(false);
                            TapToNext.gameObject.SetActive(false);
                            magicQuest = false;
                            NextState(State.GrabObject);
                        }
                    }
                }
                break;
            case State.GrabObject:
                pointer.gameObject.SetActive(true);
                if (countGrab != 3)
                {
                    pointer.transform.position = Vector3.MoveTowards(pointer.transform.position, 
                        jumpPlant.transform.position, pointerSpeed * 2f * Time.deltaTime);    
                }
                if (pointer.transform.position == jumpPlant.transform.position && !checkNextGrab)
                {
                    countGrab = 1;
                }

                switch (countGrab)
                {
                    case 1:
                        checkNextGrab = true;
                        tutorialText.text = "This is the object that can be controlled. Move near the Jump Plant to continue";
                        tutorialPanel.gameObject.SetActive(true);
                        TapToNext.gameObject.SetActive(true);
                        canMove = true;
                        if (Input.touchCount > 0 || Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
                        {
                            countGrab = 2;
                        }
                        break;
                    case 2:
                        TapToNext.gameObject.SetActive(false);
                        tutorialPanel.gameObject.SetActive(false);
                        unTrajectory.gameObject.SetActive(false);
                        if (playerController.checkColliderObj)
                        {
                            tutorialPanel.gameObject.SetActive(true);
                            countGrab = 3;
                        }
                        break;
                    case 3:
                        pointer.transform.position = Vector3.MoveTowards(pointer.transform.position, 
                            grabButton.transform.position, pointerSpeed * 2f * Time.deltaTime);
                        tutorialText.text = "This button will appear when you get close to Object. Re-click this button to uncontrolled object";
                        if (pickUpItem.completeGrab)
                        {
                            canMove = false;
                            ChangeState(State.Monster);
                        }
                        break;
                }
                break;
            case State.Monster:
                destroyTarget = false;
                pointer.gameObject.SetActive(true);
                pointer.transform.position = Vector3.MoveTowards(pointer.transform.position, 
                    monster.transform.position, pointerSpeed * 2f * Time.deltaTime);
                if (pointer.transform.position == monster.transform.position && !nextMonsterTuto)
                {
                    countNext = 1;
                }
                switch (countNext)
                {
                    case 1:
                        energy.completeCastSpell = 0;
                        nextMonsterTuto = true;
                        unTrajectory.SetActive(true);
                        tutorialText.text = "This is monsters, don't let them hit you";
                        TapToNext.gameObject.SetActive(true);
                        tutorialPanel.gameObject.SetActive(true);
                        if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
                        {
                            countNext = 2;
                        }
                        break;
                    case 2:
                        tutorialText.text = "The top of the head is their weak point, use the portal and Jumping Plant to destroy them.";
                        if (Input.touchCount > 1 || Input.GetMouseButtonDown(0))
                        {
                            target3.SetActive(true);
                            target4.SetActive(true);
                            enterPortal.SetActive(false);
                            entryPortal.SetActive(false);
                            countNext = 3;
                        }
                        break;
                    case 3:
                        tutorialText.text = "Create a portal above the monster's head. Shoot at two arrows to continue";
                        unTrajectory.SetActive(false);
                        if (Input.touchCount > 2 || Input.GetMouseButtonDown(0))
                        {
                            TapToNext.gameObject.SetActive(false);
                            tutorialPanel.gameObject.SetActive(false);
                        }
                        Debug.Log(energy.completeCastSpell);
                        if (enterPortal.activeSelf && entryPortal.activeSelf)
                        {
                            if (!target3.activeSelf && !target4.activeSelf)
                            {
                                Debug.Log("VAR2");
                                tutorialPanel.gameObject.SetActive(true);
                                countNext = 4;
                            }
                            
                            else if (target3.activeSelf || target4.activeSelf)
                            {
                                Debug.Log("VAR3");
                                energy.completeCastSpell = 0;
                                target3.SetActive(true);
                                target4.SetActive(true);
                                enterPortal.SetActive(false);
                                entryPortal.SetActive(false);
                                tutorialPanel.gameObject.SetActive(true);
                            }
                        }
                        break;
                    case 4:
                        tutorialText.text = "Control this Jumping Plant and try to throw this object through portal when the monster comes under the gate";
                        canMove = true;
                        if (Input.touchCount > 3 || Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
                        {
                            tutorialPanel.gameObject.SetActive(false);
                            TapToNext.gameObject.SetActive(false);
                        }
                        if (monster.completeMonster)
                        {
                            pointer.gameObject.SetActive(false);
                            NextState(State.Win); 
                            victoryZone.SetActive(true);
                        }
                        break;
                }
                break;
            case State.Win:
                pointer.gameObject.SetActive(true);
                nextButton.SetActive(false);
                replayButton.SetActive(false);
                exitMenu.SetActive(false);
                watchAdsButton.SetActive(false);
                noButton.SetActive(false);
                tutorialText.text = "You're almost done, touch this tree to complete the level !";
                tutorialPanel.gameObject.SetActive(true);
                pointer.transform.position = Vector3.MoveTowards(pointer.transform.position, 
                    victoryZone.transform.position, pointerSpeed * Time.deltaTime);
                if (pointer.transform.position == victoryZone.transform.position)
                {
                    if (Input.touchCount > 0 || Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
                    {
                        tutorialPanel.gameObject.SetActive(false);
                        TapToNext.gameObject.SetActive(false);
                    }
                    if (playerController.completeWin)
                    {
                        winPanel.SetActive(true);
                        upPanel.SetActive(false);
                        movePanel.SetActive(false);
                        grabButton.SetActive(false);
                        tutorialPanel.gameObject.SetActive(false);
                        pointer.gameObject.SetActive(false);
                        NextState(State.ClickMenu);
                    }
                }
                break;
            case State.ClickMenu:
                pointer.gameObject.SetActive(true);
                isTouchUI.checkTouch = true;
                pointer.transform.position = Vector3.MoveTowards(pointer.transform.position, 
                    menuButton.transform.position, (pointerSpeed +20f)  * Time.deltaTime);
                PlayerPrefs.SetInt("Complete Menu FTUE",0);
                if (pointer.transform.position == menuButton.transform.position)
                {
                    // Time.timeScale = 0;
                }
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
            case State.Welcome:
                break;
            case State.Button:
                break;
            case State.CastSpell:
                break;
            case State.ManaBar:
                break;
            case State.MagicShard:
                break;
            case State.Requirement:
                break;
            case State.GrabObject:
                break;
            case State.Monster:
                break;
            case State.Win:
                break;
            case State.ClickMenu:
                break;
            default:
                return;
        }
    }

    public void NextState(State state)
    {
        ChangeState(state);
    }
    
    public static Vector2 Parabola(Vector2 start, Vector2 end, float height, float t)
    {
        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        var mid = Vector2.Lerp(start, end, t);

        return new Vector2(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t));
    }
}