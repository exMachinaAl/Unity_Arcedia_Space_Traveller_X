using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public StoryStep[] steps;
    int currentStep = 0;

    void Start()
    {
        PlayStep();
    }

    public void PlayStep()
    {
        var step = steps[currentStep];

        //SubtitleSystem.Instance.Show(step.speakerName, step.subtitleText);
        //AudioPlayer.Play(step.voice);

        if (step.quest != null)
        {
            QuestManager.Instance.StartQuest(step.quest);
        }
    }

    public void NextStep()
    {
        currentStep++;
        if (currentStep < steps.Length)
            PlayStep();
    }
}
