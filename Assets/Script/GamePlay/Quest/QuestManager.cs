using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SpriteGlow;

public class QuestManager : MonoBehaviour
{
    public PlayerController playerController;
    public GameObject starRanking, starRanking2, starRanking3, starRankingBackground, starRankingBackground2, starRankingBackground3, 
        monsterGameOverImg, magicShardGameOverImg, monsterWinImg, magicShardWinImg, usedBooster;
    public int countKillMonster, countCollectMagicShard, killMonsterQuest, collectMagicShardQuest, countStar, level, starLevelListCount, tempMana, tempUp, starQuest;
    public bool completedMonster, completedMagicShard, completedLevel;
    public Text killMonsterQuestText, magicShardQuestText, killedMonsterText, collectedMagicShardText, 
        monsterGameOverText, magicShardGameOverText, monsterWinText, magicShardWinText;
    public List<Quest> questList = new List<Quest>();
    public WatchAdEvent watchAdEvent;

    public void Start()
    {
        playerController.checkWin = false;
        completedLevel = false;
        completedMonster = false;
        completedMagicShard = false;
        if (!PlayerPrefs.HasKey("Level Star " + level))
        {
            PlayerPrefs.SetInt("Level Star " + level,0);
        }

        if (!PlayerPrefs.HasKey("Subtotal level Star " + level))
        {
            PlayerPrefs.SetInt("Subtotal level Star " + level,0);
        }

        tempMana = 10;
        tempUp = 1;
        if (!PlayerPrefs.HasKey("Total Mana") && !PlayerPrefs.HasKey("Star To Upgrade"))
        {
            PlayerPrefs.SetInt("Total Mana", tempMana);
            PlayerPrefs.SetInt("Star To Upgrade", tempUp);
        }
        if (!PlayerPrefs.HasKey("Used booster"))
        {
            PlayerPrefs.SetInt("Used booster",1);
        }
        for (int i = 0; i < questList.Count; i++)
        {
            if (questList[i].questProgress == Quest.QuestProgress.KillMonster)
            {
                killMonsterQuestText.text = "Kill monster:     " + questList[i].goalKillMonster;
                monsterWinImg.SetActive(true);
                monsterGameOverImg.SetActive(true);
            }

            if (questList[i].questProgress == Quest.QuestProgress.CollectingMagicShard)
            {
                magicShardQuestText.text = "Collect magic shard:     " + questList[i].goalMagicShard;
                magicShardWinImg.SetActive(true);
                magicShardGameOverImg.SetActive(true);
            }
        }
        if (PlayerPrefs.GetInt("Used booster") == 2)
        {
            usedBooster.SetActive(true);
        }

        if (PlayerPrefs.GetInt("Used booster") != 2)
        {
            usedBooster.SetActive(false);
        }
    }

    public void Update()
    {
        for (int i = 0; i < questList.Count; i++)
        {
            if (questList[i].questProgress == Quest.QuestProgress.KillMonster)
            {
                killMonsterQuestText.text = "Kill monsters:     " + questList[i].goalKillMonster;
                if (questList[i].goalKillMonster >= countKillMonster)
                {
                    killedMonsterText.text = "x" + (questList[i].goalKillMonster - countKillMonster);
                }
                monsterWinText.text = "Killed monsters:   " + countKillMonster + "/" + questList[i].goalKillMonster;
                monsterGameOverText.text = "Killed monsters:   " + countKillMonster + "/" + questList[i].goalKillMonster;
                killMonsterQuest = questList[i].goalKillMonster;
                if (countKillMonster == killMonsterQuest && !completedMonster)
                {
                    starQuest += 1;
                    completedMonster = true;
                }
            }

            if (questList[i].questProgress == Quest.QuestProgress.CollectingMagicShard)
            {
                magicShardQuestText.text = "Collect Magic shard:     " + questList[i].goalMagicShard;
                if (questList[i].goalMagicShard >= countCollectMagicShard)
                {
                    collectedMagicShardText.text = "x" + (questList[i].goalMagicShard - countCollectMagicShard);
                }
                magicShardWinText.text = "Magic Shard collected:   " + countCollectMagicShard + "/" + questList[i].goalMagicShard;
                magicShardGameOverText.text = "Magic Shard collected:   " + countCollectMagicShard + "/" + questList[i].goalMagicShard;
                collectMagicShardQuest = questList[i].goalMagicShard;
                if (countCollectMagicShard == collectMagicShardQuest && !completedMagicShard)
                {
                    starQuest += 1;
                    completedMagicShard = true;
                }
            }
        }

        if (playerController.checkWin && !completedLevel)
        {
            starQuest += 1;
            completedLevel = true;
        }
        
        if (playerController.checkWin)
        {
            if (countStar >= PlayerPrefs.GetInt("Level Star " + level))
            {
                if (watchAdEvent.checkMultiply2Stars)
                {
                    PlayerPrefs.SetInt("Level Star " + level, countStar);
                    PlayerPrefs.SetInt("Subtotal level Star" + level,countStar * 2);    
                }
                else
                {
                    PlayerPrefs.SetInt("Level Star " + level, countStar);
                    PlayerPrefs.SetInt("Subtotal level Star" + level, countStar);
                }
            }
        }
        GainStar(starQuest);
        ShowStar(countStar);
    }
    
    public void GainStar(int gain)
    {
        if (gain == 1)
        {
            countStar = 1; 
            starRanking.SetActive(true);
        }

        if (gain == 2)
        {
            countStar = 2;
            starRanking2.SetActive(true);
        }

        else if (gain == 3)
        {
            countStar = 3;
            starRanking3.SetActive(true);
        }
    }
    public void ShowStar(int gain)
    {
        if (gain == 1) {starRankingBackground.SetActive(false);}
        if (gain == 2) {starRankingBackground2.SetActive(false);starRankingBackground.SetActive(false);}
        if (gain == 3) {starRankingBackground3.SetActive(false);starRankingBackground2.SetActive(false);starRankingBackground.SetActive(false);}
    }
}
