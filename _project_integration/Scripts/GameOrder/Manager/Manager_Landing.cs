using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Manager_Landing : MonoBehaviour
{
    public static Manager_Landing Instance;
    public Transform surfaceRootPosition;       // titik 0,0,0 world surface (flat terrain)
    public Transform transformCurrentPlanet;
    public Transform playerShip;
    public Camera playerShipCam;
    public string surfacePlanetSceneName = "Template_UnderWorld";
    public string atmosferPlanetSceneName = "Template_Atmosphere";
    public string spaceSceneName = "Template_SpaceWorld";
    public float landingDistance = 150f;         // jarak spawn dari surface root
    public float surfaceHeightOffset = 100f;     // agar tidak nancep ke tanah
    private Coroutine enteringPlanet;
    public Game_PlanetFullInformation currentPlanet;
    private bool isInAtmosphere = false;
    private bool isEnteringSurface = false;

    float slowDownSpeedEnterAtmosphere = 5f;
    float approachSpeedInAtmosphere = 20f;

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
    public void EnteringPlanetSurfaceActiveFlags(Transform planetSaatIni)
    {
        Debug.Log("[Landing Manager] flag control enter planet");
        // transformCurrentPlanet = planetSaatIni;
        isEnteringSurface = true;
    }

    public void EnterPlanetAtmosphere(Game_PlanetFullInformation planet)
    {
        playerShip = Manager_Player.Instance.flightCtrl.shipTransform;
        playerShipCam = Manager_Player.Instance.shipCam;
        transformCurrentPlanet = planet.planetTransform;
        currentPlanet = planet;
        isInAtmosphere = true;

        StartCoroutine(CameraHighSpeedEffect());

        Game_SaveSystem.Instance.setCurrentPlanetId(planet.planetInfo.planetSeed);
        Debug.Log("[Landing Manager] masuk planet");
        Debug.Log("Landing mode: ON → Planet " + planet.planetInfo.planetName);

        enteringPlanet = StartCoroutine(EnteringPlanet());
    }

    public void ExitPlanetAtmosphere(Game_PlanetFullInformation planet)
    {
        // yield return WaitForSeconds(3f);
        playerShipCam.fieldOfView = 50;
        isInAtmosphere = false;

        if (currentPlanet == planet)
            currentPlanet = null;

        if (enteringPlanet != null)
            StopCoroutine(enteringPlanet);

        StartCoroutine(UnloadSceneAsync(new string[] { surfacePlanetSceneName, atmosferPlanetSceneName }));


        Debug.Log("[Landing Manager] keluir dari planet");
    }


    IEnumerator EnteringPlanet()
    {

        // nyalakan efek atmosfer masuk
        yield return StartCoroutine(PlayAtmosphereEffect());

        // 1. Hitung arah masuk (planet normal)
        Vector3 planetUp = (playerShip.position - transformCurrentPlanet.position).normalized;
        // 2. Rotasi baru pemain (supaya “atasnya” ikut permukaan)
        Quaternion newRot = Quaternion.LookRotation(playerShip.forward, planetUp);

        // load scene permukaan planet
        AsyncOperation asyncLoadAtmosphere = SceneManager.LoadSceneAsync(atmosferPlanetSceneName, LoadSceneMode.Additive);
        while (!asyncLoadAtmosphere.isDone)
            yield return null;

        AsyncOperation asyncLoadSurface = SceneManager.LoadSceneAsync(surfacePlanetSceneName, LoadSceneMode.Additive);
        while (!asyncLoadSurface.isDone)
            yield return null;


        // perulangan animasi memasukin atmosphere berkecepatan tinggi atau keluar atmos
        if (playerShip != null || playerShipCam != null)
        {
            float t = 0;
            while (t < 3f) // 3 detik masuk atmosfer
            {
                playerShip.position += playerShip.forward * slowDownSpeedEnterAtmosphere * Time.deltaTime;
                t += Time.deltaTime;
                yield return null;
            }
        }
        while (isInAtmosphere)
        {
            yield return StartCoroutine(playerAutoMove());
            if (isEnteringSurface)
            {
                // animation cloud or fog to disturb player vision, or just play with camera
                yield return StartCoroutine(FadeTransition(1f));

                isInAtmosphere = false;
                isEnteringSurface = false;

                // mematikan selurun scene A dan B
                yield return StartCoroutine(UnloadSceneAsync(new string[] { spaceSceneName, atmosferPlanetSceneName }));

                // rotasi execute setelah scene space hilang
                playerShipCam.fieldOfView = 50;
                RotateByPlanetCenter(planetUp, newRot);

                // mematikan routine ini karena sudah tercapai
                StopCoroutine(enteringPlanet);
            }
            //animation high speed, but the player move slowly, just effect and player char move forward automatic
            yield return null;
        }

        // UnloadSceneAsync(new string[] { surfacePlanetSceneName, atmosferPlanetSceneName });

        // StopCoro



        // efek fade transition
        // yield return StartCoroutine(FadeTransition(1f));

        // unload space
        // SceneManager.UnloadSceneAsync(spaceSceneName);

        // isLanding = false;
    }

    private IEnumerator playerAutoMove()
    {
        if (playerShip != null || playerShipCam != null)
        {
            float t = 0;
            while (t < 1.5f)
            {
                playerShip.position += playerShip.forward * approachSpeedInAtmosphere * Time.deltaTime;
                t += Time.deltaTime;
                yield return null;
            }
        }
    }

    private void RotateByPlanetCenter(Vector3 planetUp, Quaternion newRot)
    {
        // 4. Hitung titik teleport di world permukaan
        Vector3 surfacePoint = surfaceRootPosition.position
                             + (new Vector3(planetUp.x, 0, planetUp.z).normalized * landingDistance);

        surfacePoint.y = surfaceRootPosition.position.y + surfaceHeightOffset;

        // 5. Teleport
        playerShip.position = surfacePoint;
        playerShip.rotation = newRot;
    }

    IEnumerator EnteringAtmosphere()
    {
        yield return new WaitForSeconds(3f);
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

    public IEnumerator UnloadSceneAsync(string[] sceneToUnload)
    {
        foreach (string sceneName in sceneToUnload)
        {
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(sceneName);
            while (!unloadOp.isDone)
            {
                yield return null;
            }
            // log berhasil unload
        }
    }

    public IEnumerator LoadAndSetActiveScene(string sceneName)
    {
        AsyncOperation LoadOp = SceneManager.LoadSceneAsync(sceneName);
        while (!LoadOp.isDone)
        {
            yield return null;
        }

        Scene sceneToActive = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(sceneToActive);

        //log sukes set aktif scene
    }

    private IEnumerator CameraHighSpeedEffect()
    {
        float t = 0f;
        while (t < 100f)
        {
            playerShipCam.fieldOfView = Mathf.RoundToInt(t);
            t++;
            yield return null;
        }
    }
}
