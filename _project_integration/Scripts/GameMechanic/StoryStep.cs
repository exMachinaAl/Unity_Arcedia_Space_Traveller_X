using UnityEngine;

[CreateAssetMenu(fileName = "StoryStep", menuName = "Story/Story Step")]
public class StoryStep : ScriptableObject
{
    public string speakerName;
    [TextArea] public string subtitleText;
    public AudioClip voice;
    public Quest quest;  // optional quest
}
