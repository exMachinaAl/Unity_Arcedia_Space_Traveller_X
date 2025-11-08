using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameBootstraps : MonoBehaviour
{
    public string scene1 = "";
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        StartCoroutine(DoLoad());
    }

    private IEnumerator DoLoad() {
        yield return SceneManager.LoadSceneAsync(scene1, LoadSceneMode.Additive);
        // yield return SceneManager.LoadSceneAsync("World", LoadSceneMode.Additive);

        // if (UniverseManager.I != null) UniverseManager.I.OnScenesLoaded();
    }
}