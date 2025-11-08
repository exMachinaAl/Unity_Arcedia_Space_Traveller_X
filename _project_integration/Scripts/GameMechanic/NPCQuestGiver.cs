using UnityEngine;

public class NPCQuestGiver : MonoBehaviour
{
    public string npcName;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            QuestManager.Instance.InteractWithNPC(npcName);
        }
    }
}
