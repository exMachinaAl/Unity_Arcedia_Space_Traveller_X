using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneAnimator : MonoBehaviour
{
    public Image fadePanel;
	public Image subFadePanel;
    public TextMeshProUGUI introText;
	public string nextScene = ""; // nama scene animasi fade

    [Header("Settings")]
    public Color fadeColor = Color.black;
	public Color subFadeColor = Color.white;
    public float fadeDuration = 2f;
    public float textDelay = 1f;
    public float textFadeDuration = 2f;

    void Start()
    {
        // Pastikan panel mulai dari 100% opasitas
        fadePanel.color = fadeColor;
		subFadePanel.color = subFadeColor;
        introText.color = new Color(introText.color.r, introText.color.g, introText.color.b, 0);

        StartCoroutine(PlayIntro());
    }

    IEnumerator PlayIntro()
    {
        // 1️⃣ Fade In (dari hitam/putih ke transparan)
        yield return StartCoroutine(Fade(1, 0));

        // 2️⃣ Tunda sebentar sebelum teks muncul
        yield return new WaitForSeconds(textDelay);
		
		yield return StartCoroutine(FadeVerse(0, 1));
		
		yield return new WaitForSeconds(textDelay);

        // 3️⃣ Munculkan teks secara halus
        yield return StartCoroutine(FadeText(0, 1));

        // 4️⃣ Tunggu beberapa detik (misal pemain baca teks)
        yield return new WaitForSeconds(3f);

        // 5️⃣ Fade Out kembali ke hitam/putih
        yield return StartCoroutine(Fade(0, 1));
		yield return StartCoroutine(FadeVerse(1, 0));

        // Bisa load scene berikutnya di sini
		if (nextScene != null && nextScene == "") {
			SceneManager.LoadScene(nextScene);
		} else {
			Debug.Log("scene not found or you not set it");
		}
        
    }

    IEnumerator Fade(float from, float to)
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, t / fadeDuration);
			//subFadePanel.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
            fadePanel.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
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

    IEnumerator FadeText(float from, float to)
    {
        float t = 0;
        while (t < textFadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, t / textFadeDuration);
            introText.color = new Color(introText.color.r, introText.color.g, introText.color.b, alpha);
            yield return null;
        }
    }
}
