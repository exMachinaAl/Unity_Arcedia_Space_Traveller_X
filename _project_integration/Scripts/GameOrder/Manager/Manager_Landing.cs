using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Manager_Landing : MonoBehaviour
{
    public static Manager_Landing Instance;
    // public Transform player;
    public string planetSceneName = "Template_UnderWorld";
    public string atmosferPlanetSceneName = "Template_Atmosphere";
    public string spaceSceneName = "Template_SpaceWorld";
    private bool isLanding = false;

    void Awake()
    {
        if (Manager_Landing.Instance != null && Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // public float triggerDistance = 500f; // jarak mulai proses landing
    // public float landingSpeedThreshold = 30f;


    // void Update()
    // {
    //     if (isLanding) return;

    //     // contoh jarak planet (bisa pakai vector jarak antar object)
    //     float distance = Vector3.Distance(player.position, Vector3.zero);

    //     // cek jarak & kecepatan
    //     if (distance < triggerDistance && player.GetComponent<Rigidbody>().velocity.magnitude < landingSpeedThreshold)
    //     {
    //         StartCoroutine(LandingSequence());
    //     }
    // }
    public void EnterPlanetAtmosphere(Game_PlanetFullInformation planet)
    {
        // currentPlanet = planet;
        Game_SaveSystem.Instance.setCurrentPlanetId(planet.planetInfo.planetSeed);
        Debug.Log("Landing mode: ON → Planet " + planet.planetInfo.planetName);
    }

    public void ExitPlanetAtmosphere(Game_PlanetFullInformation planet)
    {
        // if (currentPlanet == planet)
        //     currentPlanet = null;

        Debug.Log("Landing mode: OFF");
    }

    IEnumerator LandingSequence()
    {
        isLanding = true;

        // nyalakan efek atmosfer masuk
        yield return StartCoroutine(PlayAtmosphereEffect());

        // load scene permukaan planet
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(planetSceneName, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
            yield return null;

        // efek fade transition
        yield return StartCoroutine(FadeTransition(1f));

        // unload space
        SceneManager.UnloadSceneAsync(spaceSceneName);

        isLanding = false;
    }

    IEnumerator PlayAtmosphereEffect()
    {
        // efek kamera, partikel, atau lens flare
        // bisa juga ganti skybox ke warna merah/oranye sementara
        yield return new WaitForSeconds(3f);
    }

    IEnumerator FadeTransition(float duration)
    {
        // buat overlay fade UI di layar
        yield return new WaitForSeconds(duration);
    }
}
