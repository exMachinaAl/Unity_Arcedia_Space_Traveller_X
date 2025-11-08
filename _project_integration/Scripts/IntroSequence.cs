using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using System.Collections;
using System.Collections.Generic;

public class IntroSequence : MonoBehaviour
{
    public Image fade;
    public Image subFadePanel;
    public TextMeshProUGUI textUI;
    public PlayableDirector timeline;
    public Material spaceWarpMat;
    public Color subFadeColor = Color.white;
    public List<string> AdditiveScene;
    [TextArea] public List<string> introTextList;

    public float textSpeed = 2f;
    public float textStay = 2f;
    public float warpMax = 0.9f;
    public float fadeDuration = 2f;
    public string nextScene = "Scene_1";

    void Start()
    {
        fade.color = Color.black;
        StartCoroutine(RunIntro());
    }

    IEnumerator RunIntro()
    {
        // Fade in
        yield return Fade(1, 0, 2f);

        timeline.Play();

        // yield return StartCoroutine(FadeVerse(0, 1));

        // Text sequence
        foreach (var t in introTextList)
        {
            yield return ShowText(t);
        }


        // Warp travel burst
        //yield return Warp(0, warpMax, 1.5f);

        yield return new WaitForSeconds(1.5f);

        //yield return StartCoroutine(FadeVerse(1, 0));
        yield return Fade(0, 1, 2f);

        SceneManager.LoadScene(nextScene);
        foreach (var s in AdditiveScene) 
            SceneManager.LoadScene(s, LoadSceneMode.Additive);
    }

    IEnumerator ShowText(string s)
    {
        textUI.text = s;
        textUI.alpha = 0;

        float t = 0;
        while(t < textSpeed)
        {
            textUI.alpha = t / textSpeed;
            t += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(textStay);

        t = 0;
        while(t < textSpeed)
        {
            textUI.alpha = 1 - (t / textSpeed);
            t += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator Fade(float a, float b, float dur)
    {
        float t = 0;
        while (t < dur)
        {
            fade.color = new Color(0,0,0, Mathf.Lerp(a,b,t/dur));
            t += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator Warp(float a, float b, float dur)
    {
        float t = 0;
        while(t < dur)
        {
            spaceWarpMat.SetFloat("_Intensity", Mathf.Lerp(a,b,t/dur));
            t += Time.deltaTime;
            yield return null;
        }
    }
	IEnumerator FadeVerse(float from, float to)
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, t / fadeDuration);
			subFadePanel.color = new Color(subFadeColor.r, subFadeColor.g, subFadeColor.b, alpha);
            //fadePanel.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
            yield return null;
        }
    }
}
