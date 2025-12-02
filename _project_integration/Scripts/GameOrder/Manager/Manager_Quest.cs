using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class OCQuestCategory {
    public List<SO_StoryQuest> storyQuest = new List<SO_StoryQuest>();
    public List<SO_QuestCreator> mainQuest = new List<SO_QuestCreator>();
    public List<SO_QuestCreator> sideQuest = new List<SO_QuestCreator>();
}


public class Manager_Quest : MonoBehaviour
{
    public static Manager_Quest Instance;

    public OCQuestCategory quests = new OCQuestCategory();
    [SerializeField] private AudioSource audioNpcTalk;

    void Awake()
    {
        if (Instance == null && Manager_Quest.Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    Quest activeQuest;
    int count;
    int mainStepQst;
    int numStepQst;
    bool objectiveDone = false;
    SO_QuestCreator currentQst;
    SO_QuestCreator.OCStepQuest currentStepQst;

    //sysytem testing
    public GameObject planePrefab;

    void Start()
    {
        mainStepQst = 0;
        numStepQst = 0;
    }

    void Update()
    {
        // CheckInventoryItemQuest();
    }

    public void StartStoryQuest()
    {
        // currentQst = quests.mainQuest[mainStepQst];
        // currentStepQst = currentQst.questStep[numStepQst];
        // activeQuest = q;
        // count = 0;
        objectiveDone = false;

        try
        {
            currentQst = quests.mainQuest[mainStepQst];
        }
        catch (System.ArgumentOutOfRangeException aex)
        {
            Debug.LogWarning($"error questManager = {aex?.Message}");
            // if (currentQst == null)
            {
                // crtitical test mode for fast track
                GameObject plane = Instantiate(planePrefab, new Vector3(0, 10, 0), Quaternion.identity);
                var rootsObj = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
                foreach (var obj in rootsObj)
                {
                    if (obj.name == "Broken_Ship")
                    {
                        Destroy(obj);
                    }
                }
                // GameObject brknShip = plane.transform.parent.Find("Broken_Ship").gameObject;
                // Destroy(brknShip);

                Debug.Log($"Quest completeted");
                return;
            }
        }


        try
        {
            currentStepQst = currentQst.questStep[numStepQst];
        }
        catch (System.ArgumentOutOfRangeException aex)
        {
            Debug.LogWarning($"error questManager = {aex?.Message}");
            // if (currentStepQst == null)
            {
                mainStepQst++;
                numStepQst = 0;
                StartStoryQuest();
                return;
            }
        }

        Manager_UI.Instance.SetCurrentQuest(currentQst.questTitle, currentStepQst.subTitleQuest);
        // Manager_UI.Instance.SetCurrentQuest(currentQst.questTitle, currentQst.questDescription);
        StartCoroutine(YappingPerSec(5f));

        // mainStepQst++;
    }

    private IEnumerator YappingPerSec(float yap)
    {
        // foreach (var questStep in currentQst.questStep)
        {
            // foreach (var befYap in questStep.npcTalkBefore)
            foreach (var befYap in currentStepQst.npcTalkBefore)
            {
                Manager_UI.Instance.ShowYapping(befYap.npcT);
                // Manager_Audio.Instance.PlayAudioClip(audioNpcTalk, befYap.npcV);
                yield return new WaitForSeconds(yap);
            }
            Manager_UI.Instance.HideYapping();
        }
    }
    private IEnumerator AfYappingPerSec(float yap) // kode yapping handling ketika sudah selelsaiin quest
    {
        // foreach (var questStep in currentQst.questStep)
        {
            // foreach (var befYap in questStep.npcTalkBefore)
            foreach (var afYap in currentStepQst.npcTalkAfter)
            {
                Manager_UI.Instance.ShowYapping(afYap.npcT);
                yield return new WaitForSeconds(yap);
            }
            Manager_UI.Instance.HideYapping();
        }
    }

    public void CheckInventoryItemQuest()
    {
        if (currentQst == null) return;
        if (objectiveDone) return;

        Game_PlayerInventory playerInv = Manager_Player.Instance.player.transform.GetComponent<Game_PlayerInventory>();
        int totalItem = 0;

        // for (int i = 0; i < playerInv.items.Count; i++)
        // {
        //     ItemStack stack = playerInv.items[i];
        //     if (stack.item.id == currentStepQst.objective.idItem)
        //     {
        //         totalItem += stack.amount;
        //         Manager_UI.Instance.UpdateProgress(totalItem, currentStepQst.objective.valueItem);
        //     }
        // }
        // if (playerInv.HasItem(currentStepQst.objective.idItem, currentStepQst.objective.valueItem))
        // {
        //     Manager_UI.Instance.UpdateProgress(totalItem, currentStepQst.objective.valueItem);
        // }

        StartCoroutine(UIUpdateProgressMiniQuest(playerInv, totalItem, (totalItem) =>
        {
            if (totalItem >= currentStepQst.objective.valueItem)
            {
                OnObjectiveComplete();
            }
        }));
    }
    IEnumerator UIUpdateProgressMiniQuest(Game_PlayerInventory playerInv, int totalItem, Action<int> onComplete)
    {
        for (int i = 0; i < playerInv.items.Count; i++)
        {
            ItemStack stack = playerInv.items[i];
            if (stack.item.id == currentStepQst.objective.idItem)
            {
                totalItem += stack.amount;
                Manager_UI.Instance.UpdateProgress(totalItem, currentStepQst.objective.valueItem);
            }
            yield return null;
        }
        onComplete(totalItem);
    }
    public void CollectItem(string itemName, int collected) // ubah menjadi (Update) prograss di unity behaviour
    {
        if (currentQst == null) return;
        if (itemName != currentStepQst.objective.idItem) return;

        count += collected;
        //count++;
        Manager_UI.Instance.UpdateProgress(count, currentStepQst.objective.valueItem);

        if (count >= currentStepQst.objective.valueItem)
            OnObjectiveComplete();
    }

    void OnObjectiveComplete()
    {
        objectiveDone = true;
        Manager_UI.Instance.MarkObjectiveDone();

        if (currentStepQst.completionMode == QuestCompletionMode.AutoComplete)
        {
            CompleteQuest();
        }
        else
        {
            Manager_UI.Instance.ShowReturnHint(currentStepQst.npcTalkBefore[0].npcName);
        }
    }

    public void InteractWithNPC(NPCQuestGiver npcSc)
    {
        if (currentQst == null) return;
        if (!objectiveDone) return;

        audioNpcTalk = npcSc.audioSrc;

        if (currentStepQst.completionMode == QuestCompletionMode.ReturnToNPC &&
            npcSc.npcName == currentStepQst.npcTalkBefore[0].npcName)
        {
            //#remove item jiks quest adalah return to NPC
            Game_PlayerInventory playerInv = Manager_Player.Instance.player.transform.GetComponent<Game_PlayerInventory>();
            playerInv.RemoveItem(currentStepQst.objective.idItem, currentStepQst.objective.valueItem);

            StartCoroutine(AfYappingPerSec(5f));
            CompleteQuest();
        }
    }

    void CompleteQuest()
    {
        count = 0;
        numStepQst++;
        StartStoryQuest();
        // Manager_UI.Instance.Hide();
        // currentQst = null;

        // Continue story
        // FindObjectOfType<StoryManager>().NextStep();
    }
}
