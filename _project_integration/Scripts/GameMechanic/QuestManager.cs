using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    // void Awake() => Instance = this;
    void Awake()
    {
        if (Instance == null && QuestManager.Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); Load(); }
        else Destroy(gameObject);
    }

    Quest activeQuest;
    int count;
    bool objectiveDone = false;

    public void StartQuest(Quest q)
    {
        activeQuest = q;
        count = 0;
        objectiveDone = false;

        UIQuest.Instance.Show(q.questTitle, q.questDescription);
    }

    public void CollectItem(string itemName, int collected)
    {
        if (activeQuest == null) return;
        if (itemName != activeQuest.targetItemName) return;

		count += collected;
        //count++;
        UIQuest.Instance.UpdateProgress(count, activeQuest.targetItemCount);

        if (count >= activeQuest.targetItemCount)
            OnObjectiveComplete();
    }

    void OnObjectiveComplete()
    {
        objectiveDone = true;
        UIQuest.Instance.MarkObjectiveDone();

        if (activeQuest.completionMode == QuestCompletionMode.AutoComplete)
        {
            CompleteQuest();
        }
        else
        {
            UIQuest.Instance.ShowReturnHint(activeQuest.returnToNPC);
        }
    }

    public void InteractWithNPC(string npcName)
    {
        if (activeQuest == null) return;
        if (!objectiveDone) return;

        if (activeQuest.completionMode == QuestCompletionMode.ReturnToNPC &&
            npcName == activeQuest.returnToNPC)
        {
            CompleteQuest();
        }
    }

    void CompleteQuest()
    {
        UIQuest.Instance.Hide();
        activeQuest = null;

        // Continue story
        FindObjectOfType<StoryManager>().NextStep();
    }
}
