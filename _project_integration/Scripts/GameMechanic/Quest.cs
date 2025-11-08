using UnityEngine;

public enum QuestCompletionMode
{
    AutoComplete,
    ReturnToNPC
}

[CreateAssetMenu(fileName = "Quest", menuName = "Story/Quest")]
public class Quest : ScriptableObject
{
    public string questTitle;
    public string questDescription;

    [Header("Requirement")]
    public string targetItemName;
    public int targetItemCount;

    [Header("Completion Mode")]
    public QuestCompletionMode completionMode = QuestCompletionMode.AutoComplete;

    [Tooltip("NPC name or ID to return to")]
    public string returnToNPC;
}
