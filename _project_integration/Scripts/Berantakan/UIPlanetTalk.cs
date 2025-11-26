using UnityEngine;
using System.Collections;
using TMPro;

public class UIPlanetTalk : MonoBehaviour
{
    public static UIPlanetTalk Instance;
    public TMP_Text chatText;

    void Awake()
    {
        Instance = this;
    }

    public void ShowSubtitle(string message, float duration = 3f)
    {
        StopAllCoroutines();
        StartCoroutine(SubtitleRoutine(message, duration));
    }

    IEnumerator SubtitleRoutine(string msg, float dur)
    {
        // Tampilkan teks dan set alpha penuh
        chatText.text = msg;
        SetTextAlpha(1f);

        // Tunggu beberapa detik
        yield return new WaitForSeconds(dur);

        // Fade out selama 0.5 detik
        float fadeTime = 0.5f;
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, t / fadeTime);
            SetTextAlpha(a);
            yield return null;
        }

        // pastikan benar-benar transparan di akhir
        SetTextAlpha(0f);
    }

    void SetTextAlpha(float alpha)
    {
        Color c = chatText.color;
        c.a = alpha;
        chatText.color = c;
    }
}
