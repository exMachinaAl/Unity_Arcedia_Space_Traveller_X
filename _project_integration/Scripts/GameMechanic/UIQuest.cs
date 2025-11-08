using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIQuest : MonoBehaviour
{
    public static UIQuest Instance;

    public CanvasGroup group;
    public RectTransform panel;

    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public TMP_Text progressText;
    public TMP_Text hintText;

    Vector2 hiddenPos = new Vector2(-400f, 0f);
    Vector2 shownPos = new Vector2(40f, 0f);
    
    void Awake()
    {
        Instance = this;
        HideInstant();
    }

    void HideInstant()
    {
        group.alpha = 0;
        panel.anchoredPosition = hiddenPos;
        gameObject.SetActive(false);
    }

    public void Show(string title, string desc)
    {
        gameObject.SetActive(true);

        titleText.text = title;
        descriptionText.text = desc;
        progressText.text = "";
        hintText.text = "";

        StopAllCoroutines();
        StartCoroutine(FadeIn());
    }

    public void UpdateProgress(int current, int target)
    {
        progressText.text = $"{current}/{target}";
    }

    public void MarkObjectiveDone()
    {
        progressText.text = "<color=#00FFFF>✔ Objective Complete</color>";
    }

    public void ShowReturnHint(string npc)
    {
        hintText.text = $"Return to <b>{npc}</b>";
    }

    public void Hide()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    System.Collections.IEnumerator FadeIn()
    {
        float t = 0;
        while(t < 1)
        {
            t += Time.deltaTime * 4;
            group.alpha = Mathf.Lerp(0, 1, t);
            panel.anchoredPosition = Vector2.Lerp(hiddenPos, shownPos, t);
            yield return null;
        }
    }

    System.Collections.IEnumerator FadeOut()
    {
        float t = 0;
        while(t < 1)
        {
            t += Time.deltaTime * 4;
            group.alpha = Mathf.Lerp(1, 0, t);
            panel.anchoredPosition = Vector2.Lerp(shownPos, hiddenPos, t);
            yield return null;
        }

        gameObject.SetActive(false);
    }
}


