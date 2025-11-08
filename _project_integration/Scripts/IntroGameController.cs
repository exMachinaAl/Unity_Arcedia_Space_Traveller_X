using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class IntroGameController : MonoBehaviour
{
    [Header("References")]
    public PlayableDirector earthTimeline;
    public Image fadePanel;
    public TextMeshProUGUI introText;
    public Volume postVolume;

    DepthOfField dof;
    bool hasDOF = false;

    [Header("Config")] 
    public float fadeDuration = 2f;
    public float textDelay = 1.5f;
    public float textFadeDuration = 2f;
    public string nextScene = "Scene_1";

    void Start()
    {
        // Safety bind DOF
        if (postVolume != null && postVolume.profile.TryGet(out dof))
        {
            hasDOF = true;
            dof.active = true;
            dof.gaussianStart.Override(1f);
            dof.gaussianEnd.Override(0f);
        }
        else
        {
            Debug.LogWarning("⚠ Depth Of Field tidak ditemukan — intro tetap jalan tanpa blur");
        }

        Color c = fadePanel.color;
        c.a = 1;
        fadePanel.color = c;

        Color t = introText.color;
        t.a = 0;
        introText.color = t;

        StartCoroutine(RunSequence());
    }

    IEnumerator RunSequence()
    {
        // Fade in → reveal Earth
        yield return Fade(1, 0, fadeDuration);

        // Wait timeline runs
        earthTimeline.Play();
        yield return new WaitForSeconds((float)earthTimeline.duration - 2f);

        // Blur only if DOF exists
        if (hasDOF) yield return Blur(0, 3f, 2f);

        // Text fade after delay
        yield return new WaitForSeconds(textDelay);
        yield return FadeText(0, 1, textFadeDuration);

        // Delay before next scene
        yield return new WaitForSeconds(2f);

        Debug.Log("Loading next scene: " + nextScene);
        SceneManager.LoadScene(nextScene);
    }

    IEnumerator Fade(float start, float end, float dur)
    {
        float t = 0;
        while (t < dur)
        {
            float a = Mathf.Lerp(start, end, t / dur);
            fadePanel.color = new Color(0, 0, 0, a);
            t += Time.deltaTime;
            yield return null;
        }
        fadePanel.color = new Color(0, 0, 0, end);
    }

    IEnumerator FadeText(float start, float end, float dur)
    {
        float t = 0;
        while (t < dur)
        {
            float a = Mathf.Lerp(start, end, t / dur);
            introText.color = new Color(introText.color.r, introText.color.g, introText.color.b, a);
            t += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator Blur(float start, float end, float dur)
    {
        float t = 0;
        while (t < dur)
        {
            float v = Mathf.Lerp(start, end, t / dur);
            dof.gaussianEnd.value = v;
            t += Time.deltaTime;
            yield return null;
        }
        dof.gaussianEnd.value = end;
    }
}
