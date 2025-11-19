using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SO_QuestCreator", menuName = "Quest/Quest creator")]
public class SO_QuestCreator : ScriptableObject
{

    [System.Serializable]
    public class OCCollectionQuestItem
    {
        public string idItem;
        public int valueItem;
    }
    
    [System.Serializable]
    public class OCNpcTalk
    {
        public string npcName;
        [TextArea] public string npcT;
        public AudioClip npcV;
    }

    [System.Serializable]
    public class OCRewardQuest
    {
        public int scienceCredit;
    }

    [System.Serializable]
    public class OCStepQuest
    {
        public string subTitleQuest;
        public OCCollectionQuestItem objective;
        public List<OCNpcTalk> npcTalkBefore = new List<OCNpcTalk>();
        public List<OCNpcTalk> npcTalkAfter = new List<OCNpcTalk>();
        public OCRewardQuest rewardPerStep;
        public QuestCompletionMode completionMode;
        public bool isDone;
    }

    public string questTitle;
    public string questDescription;
    public List<OCStepQuest> questStep;
    // public List<OCStepQuest> questStep = new List<OCStepQuest>();
    
}
