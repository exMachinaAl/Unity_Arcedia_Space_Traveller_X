using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
    [Header("Buttons")]
    public Button buttonStart;
    public Button buttonLoad;
    public Button buttonCredits;
    public Button buttonExit;

    [Header("Scene Names")]
    public string introSceneName = ""; // nama scene animasi fade
    public string loadSceneName = ""; // nama scene animasi fade
    public string creditsSceneName = "";

    void Start()
    {
        // Tambahkan event listener ke tombol-tombol
        buttonStart.onClick.AddListener(OnStartClicked);
        buttonLoad.onClick.AddListener(OnLoadClicked);
        buttonCredits.onClick.AddListener(OnCreditsClicked);
        buttonExit.onClick.AddListener(OnExitClicked);
    }

    void OnStartClicked()
    {
        Debug.Log("Start clicked!");
        // Load scene animasi fade in/out
        if (introSceneName == null || introSceneName == "")
        {
            Debug.LogWarning("Scene names for loading are not set!");
            return;
        }
        SceneManager.LoadScene(introSceneName);
    }
    void OnLoadClicked()
    {
        Debug.Log("Load clicked!");
        if (loadSceneName == null || loadSceneName == "")
        {
            Debug.LogWarning("Scene names for loading are not set!");
            return;
        }
        // Load scene animasi fade in/out
        SceneManager.LoadScene(loadSceneName);
    }

    void OnCreditsClicked()
    {
        Debug.Log("Credits clicked!");
        if (creditsSceneName == null || creditsSceneName == "")
        {
            Debug.LogWarning("Scene names for loading are not set!");
            return;
        }
        SceneManager.LoadScene(creditsSceneName);
    }

    void OnExitClicked()
    {
        Debug.Log("Exit clicked!");
        Application.Quit();
    }
}
