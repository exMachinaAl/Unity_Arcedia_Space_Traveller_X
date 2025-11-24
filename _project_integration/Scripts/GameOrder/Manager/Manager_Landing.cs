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
    public float exitAltitude = 50f;
    private Coroutine enteringPlanet;
    public Game_PlanetFullInformation currentPlanetFullData;
    private bool isInAtmosphere = false;
    private bool isEnteringSurface = false;

    float slowDownSpeedEnterAtmosphere = 5f;
    float approachSpeedInAtmosphere = 20f;

    // testing new lanfing
    public WorldSession loadPlanetData;
    bool isEnteringPlanet = false;
    bool isOnSurface = false;
    bool isExitingSurface = false;
    float exitHeight = 500f;


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


    void Update()
    {
        var loadData = Game_SaveSystem.Instance.getFullSaveData();
        // Vector3 playerPos = playerShip.position;
        // Vector3 exitPoint = loadData.worldSession.planetCenter + loadData.worldSession.entryNormal * (loadData.worldSession.planetCenter + exitAltitude);

        // // Vector3 exitHeight = (playerShip.position - transformCurrentPlanet.position).magnitude;
        // // Vector3 exitHeight = exMagnit + 
        // if (loadData.playerInThe == PlayerInThe.Ground && playerPos.y > exitPoint.y)
        // {
        //     Game_SaveSystem.Instance.SetPlayerInSpace();
        //     StartCoroutine(ExitPlanetFromSurface(exitPoint));
        // }

        // loadPlanetData = Game_SaveSystem.Instance.getFullSaveData().worldSession;
        // float distFromCenter = Vector3.Distance(playerShip.position, loadPlanetData.planetCenter) - loadPlanetData.planetVisualRadius;
        // float currentHeight = distFromCenter - loadPlanetData.planetRadius;

        float distFromCenter = playerShip.position.y;

        Debug.Log($"[ExitHeight] : {loadPlanetData.atmosphereHeight}");
        Debug.Log($"[CurHeight] : {distFromCenter}]");

        if (loadData.playerInThe == PlayerInThe.Ground && distFromCenter > loadPlanetData.atmosphereHeight)
        {
            Game_SaveSystem.Instance.SetPlayerInSpace();
            StartCoroutine(ExitPlanetFromSurface());
            // StartCoroutine(ExitSurface());
        }


    }
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
    public void EnteringPlanetSurfaceActiveFlags(Transform planetVisualTrn)
    {
        // hitung normal
        Vector3 normal = (playerShip.position - transformCurrentPlanet.position).normalized;
        float SurfaceAtmosRad = planetVisualTrn.Find("SurfaceEnterT").GetComponent<SphereCollider>().radius * planetVisualTrn.Find("SurfaceEnterT").localScale.x;
        float PlanetVisualRad = planetVisualTrn.Find("MeshPlanet").GetComponent<SphereCollider>().radius * planetVisualTrn.Find("MeshPlanet").localScale.x;

        Debug.Log($"[Landing Manager surfaceAtmos] : {SurfaceAtmosRad}");
        Debug.Log($"[Landing Manager PlanetVisualRad] : {PlanetVisualRad}");

        float atmosphereHeight = surfaceHeightOffset + ( SurfaceAtmosRad - PlanetVisualRad ) * 2;
        Debug.Log($"[Landing Manager atmosfer height] : {atmosphereHeight}");

        // float distFromCenter = Vector3.Distance(playerShip.position, loadPlanetData.planetCenter) - loadPlanetData.planetVisualRadius;
        // Debug.Log($"[Landing Manager player pos height] : {distFromCenter}");

        // simpan session planet
        Game_SaveSystem.Instance.SetWorldSession(
            currentPlanetFullData.planetInfo.planetId,
            currentPlanetFullData.planetInfo.planetSeed,
            SurfaceAtmosRad,
            PlanetVisualRad,
            atmosphereHeight,
            transformCurrentPlanet.position,
            normal,
            playerShip.position
        );

        

        Debug.Log("[Landing Manager] flag control enter planet");
        isEnteringSurface = true;
    }

    public void EnterPlanetAtmosphere(Game_PlanetFullInformation planet)
    {
        playerShip = Manager_Player.Instance.flightCtrl.shipTransform;
        playerShipCam = Manager_Player.Instance.shipCam;
        transformCurrentPlanet = planet.planetTransform;
        currentPlanetFullData = planet;
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

        if (currentPlanetFullData == planet)
            currentPlanetFullData = null;

        if (enteringPlanet != null)
            StopCoroutine(enteringPlanet);

        StartCoroutine(UnloadSceneAsync(new string[] { surfacePlanetSceneName, atmosferPlanetSceneName }));


        Debug.Log("[Landing Manager] keluir dari planet");
    }

    IEnumerator ExitPlanetFromSurface() // utama ###
    {
        yield return StartCoroutine(PlayAtmosphereEffect());

        // 1. Hitung arah masuk (planet normal)
        Vector3 planetUp = (playerShip.position - loadPlanetData.planetCenter).normalized;
        // 2. Rotasi baru pemain (supaya “atasnya” ikut permukaan)
        Quaternion newRot = Quaternion.LookRotation(playerShip.forward, planetUp);

        // load scene permukaan planet
        yield return StartCoroutine(LoadSceneIfNotLoaded(atmosferPlanetSceneName));
        // AsyncOperation asyncLoadAtmosphere = SceneManager.LoadSceneAsync(atmosferPlanetSceneName, LoadSceneMode.Additive);
        // while (!asyncLoadAtmosphere.isDone)
        //     yield return null;

        yield return StartCoroutine(LoadSceneIfNotLoaded(spaceSceneName));
        // AsyncOperation asyncLoadSpace = SceneManager.LoadSceneAsync(spaceSceneName, LoadSceneMode.Additive);
        // while (!asyncLoadSpace.isDone)
        //     yield return null;

        // teleport procedure to space cur planet
        float exitDistance = loadPlanetData.planetVisualRadius + exitHeight + 50f; // aman

        Vector3 exitPoint =
            loadPlanetData.planetCenter + planetUp * exitDistance;

        playerShip.position = exitPoint;
        playerShip.rotation = Quaternion.LookRotation(playerShip.forward, planetUp);
        // playerShip.position = exitPos;

        // if (playerShip != null || playerShipCam != null)
        // {
        //     float t = 0;
        //     while (t < 3f) // 3 detik masuk atmosfer
        //     {
        //         playerShip.position += playerShip.forward * slowDownSpeedEnterAtmosphere * Time.deltaTime;
        //         t += Time.deltaTime;
        //         yield return null;
        //     }
        // }
        // while (isInAtmosphere)
        // {
        //     yield return StartCoroutine(playerAutoMove());
        //     if (isEnteringSurface)
        //     {
        //         // animation cloud or fog to disturb player vision, or just play with camera
        //         yield return StartCoroutine(FadeTransition(1f));

        //         isInAtmosphere = false;
        //         isEnteringSurface = false;

        //         // mematikan selurun scene A dan B
        //         yield return StartCoroutine(UnloadSceneAsync(new string[] { spaceSceneName, atmosferPlanetSceneName }));

        //         // rotasi execute setelah scene space hilang
        //         playerShipCam.fieldOfView = 50;
        //         RotateByPlanetCenter(planetUp, newRot);

        //         // mematikan routine ini karena sudah tercapai
        //         StopCoroutine(enteringPlanet);
        //     }
        //     //animation high speed, but the player move slowly, just effect and player char move forward automatic
        //     yield return null;
        // }

    }

    // IEnumerator ExitSurface()
    // {
    //     // simpan rotasi exit
    //     Vector3 planetUp = (playerShip.position - planetCenter).normalized;

    //     Quaternion newRot = Quaternion.LookRotation(playerShip.forward, planetUp);

    //     // LOAD ATMOSPHERE SCENE SAJA (space nanti unload di akhir)
    //     yield return SceneManager.LoadSceneAsync(atmosferPlanetSceneName, LoadSceneMode.Additive);

    //     // Teleport ke posisi exit di atmosfer
    //     Vector3 exitPoint = planetCenter + planetUp * exitHeight;
    //     playerShip.position = exitPoint;
    //     playerShip.rotation = newRot;

    //     // Unload surface
    //     yield return SceneManager.UnloadSceneAsync(surfacePlanetSceneName);

    //     isOnSurface = false;
    //     isInAtmosphere = true;
    //     isTransitioning = false;



    //     // if (isExitingSurface) yield break;
    //     // isExitingSurface = true;

    //     // Vector3 planetUp = (playerShip.position - loadPlanetData.planetCenter).normalized;

    //     // // 1. Fade
    //     // yield return StartCoroutine(FadeTransition(1f));

    //     // // 2. Load atmosphere ONLY
    //     // yield return StartCoroutine(LoadSceneIfNotLoaded(atmosferPlanetSceneName));

    //     // // 3. Unload surface ONLY
    //     // yield return StartCoroutine(UnloadSceneIfLoaded(surfacePlanetSceneName));

    //     // // 4. Teleport ke posisi luar planet *dengan arah yang sama*
    //     // float exitDistance = loadPlanetData.planetRadius + exitHeight + 50f; // aman

    //     // Vector3 exitPoint =
    //     //     loadPlanetData.planetCenter + planetUp * exitDistance;

    //     // playerShip.position = exitPoint;
    //     // playerShip.rotation = Quaternion.LookRotation(playerShip.forward, planetUp);

    //     // // 5. Set state
    //     // isOnSurface = false;
    //     // isExitingSurface = false;
    //     // isEnteringPlanet = false;
    // }

    // IEnumerator EnteringPlanet()
    // {
    //     //planetUp untuk rotasi
    //     Vector3 planetUp = (playerShip.position - transformCurrentPlanet.position).normalized;
    //     Quaternion newRot = Quaternion.LookRotation(playerShip.forward, planetUp);

    //     // LOAD SCENE ATMOS + SURFACE
    //     yield return SceneManager.LoadSceneAsync(atmosferPlanetSceneName, LoadSceneMode.Additive);
    //     yield return SceneManager.LoadSceneAsync(surfacePlanetSceneName, LoadSceneMode.Additive);

    //     // Teleport ke surface:
    //     Vector3 surfacePoint = GetSurfaceSpawnPoint(planetUp);
    //     playerShip.position = surfacePoint;
    //     playerShip.rotation = newRot;

    //     isOnSurface = true;
    //     isTransitioning = false;

    //     yield break;






    //     // if (isEnteringPlanet) yield break; // anti-double-trigger
    //     // isEnteringPlanet = true;

    //     // Vector3 planetUp = (playerShip.position - loadPlanetData.planetCenter).normalized;
    //     // Quaternion newRot = Quaternion.LookRotation(playerShip.forward, planetUp);

    //     // // 1. Fade / efek atmosfer
    //     // yield return StartCoroutine(PlayAtmosphereEffect());

    //     // // 2. Load ONLY IF not loaded
    //     // yield return StartCoroutine(LoadSceneIfNotLoaded(atmosferPlanetSceneName));
    //     // yield return StartCoroutine(LoadSceneIfNotLoaded(surfacePlanetSceneName));

    //     // // 3. Auto gerak masuk atmosfer
    //     // float t = 0;
    //     // while (t < 3f)
    //     // {
    //     //     playerShip.position += playerShip.forward * slowDownSpeedEnterAtmosphere * Time.deltaTime;
    //     //     t += Time.deltaTime;
    //     //     yield return null;
    //     // }

    //     // // 4. Masuk mode atmosfer → surface
    //     // yield return StartCoroutine(FadeTransition(1f));

    //     // // unload space (bukan atmosphere/surface)
    //     // yield return StartCoroutine(UnloadSceneIfLoaded(spaceSceneName));

    //     // // 5. Apply rotasi relatif planet
    //     // playerShipCam.fieldOfView = 50;
    //     // RotateByPlanetCenter(planetUp, newRot);

    //     // isOnSurface = true;
    //     // isEnteringPlanet = false;
    // }                   

    IEnumerator EnteringPlanet() // utama nmih ###
    {

        // nyalakan efek atmosfer masuk
        yield return StartCoroutine(PlayAtmosphereEffect());

        // 1. Hitung arah masuk (planet normal)
        Vector3 planetUp = (playerShip.position - transformCurrentPlanet.position).normalized;
        // 2. Rotasi baru pemain (supaya “atasnya” ikut permukaan)
        Quaternion newRot = Quaternion.LookRotation(playerShip.forward, planetUp);

        Debug.Log($"### Entering planet terpicu ###");
        // load scene permukaan planet
        yield return StartCoroutine(LoadSceneIfNotLoaded(atmosferPlanetSceneName));
        // AsyncOperation asyncLoadAtmosphere = SceneManager.LoadSceneAsync(atmosferPlanetSceneName, LoadSceneMode.Additive);
        // while (!asyncLoadAtmosphere.isDone)
        //     yield return null;

        yield return StartCoroutine(LoadSceneIfNotLoaded(surfacePlanetSceneName));
        // AsyncOperation asyncLoadSurface = SceneManager.LoadSceneAsync(surfacePlanetSceneName, LoadSceneMode.Additive);
        // while (!asyncLoadSurface.isDone)
        //     yield return null;


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
        Game_SaveSystem.Instance.SetPlayerInWorld();
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
    IEnumerator LoadSceneIfNotLoaded(string name)
    {
        Scene sc = SceneManager.GetSceneByName(name);
        if (sc.isLoaded) yield break;

        AsyncOperation op = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
        while (!op.isDone) yield return null;
    }
    IEnumerator UnloadSceneIfLoaded(string name)
    {
        Scene sc = SceneManager.GetSceneByName(name);
        if (!sc.isLoaded) yield break;

        AsyncOperation op = SceneManager.UnloadSceneAsync(name);
        while (!op.isDone) yield return null;
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
