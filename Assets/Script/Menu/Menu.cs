using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject starRanking, starRanking2, starRanking3;
    public Text killMonsterQuestText, magicShardQuestText;
    public int level;
    public List<Quest> questList = new List<Quest>();

    public void Start()
    {
        for (int i = 0; i < questList.Count; i++)
        {
            if (questList[i].questProgress == Quest.QuestProgress.KillMonster)
            {
                killMonsterQuestText.text = "Kill monster:     " + questList[i].goalKillMonster;
            }

            if (questList[i].questProgress == Quest.QuestProgress.CollectingMagicShard)
            {
                magicShardQuestText.text = "Collect magic shard:     " + questList[i].goalMagicShard;
            }
        }
        ShowStar(PlayerPrefs.GetInt("Level Star " + level));
    }
    
    public void ShowStar(int gain)
    {
        if (gain == 1) {starRanking.SetActive(false);}
        if (gain == 2) {starRanking2.SetActive(false);starRanking.SetActive(false);}
        if (gain == 3) {starRanking3.SetActive(false);starRanking2.SetActive(false);starRanking.SetActive(false);}
    }
}
