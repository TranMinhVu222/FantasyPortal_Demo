using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

[System.Serializable]
public class Quest
{
    public enum QuestProgress {KillMonster,CollectingMagicShard, NoQuest}
    public QuestProgress questProgress;
    public int goalMagicShard, goalKillMonster, starReward;
}
