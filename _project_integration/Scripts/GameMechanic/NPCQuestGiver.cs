using UnityEngine;

public class NPCQuestGiver : MonoBehaviour
{
    public string npcName;
    public AudioSource audioSrc;

    void Start()
    { 
        audioSrc = GetComponentInChildren<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Manager_Quest.Instance.InteractWithNPC(this);
            // QuestManager.Instance.InteractWithNPC(npcName);
        }

    }
}
