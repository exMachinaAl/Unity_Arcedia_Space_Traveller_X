using UnityEngine;

public class NPCQuestGiver : MonoBehaviour
{
    public string npcName;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Manager_Quest.Instance.InteractWithNPC(npcName);
            // QuestManager.Instance.InteractWithNPC(npcName);
        }

    }
}
