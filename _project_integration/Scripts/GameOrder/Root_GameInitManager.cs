using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class Root_GameInitManager : MonoBehaviour
{
    // public event Action startInjectDataEvent;
    public static Root_GameInitManager Instance;

    public RNG_PlanetData planetDataConfigGen;
    public GameObject PlayerPrefab;

    void Awake()
    {
        if (Root_GameInitManager.Instance != null && Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Root_GameStartManager.Instance.InitGameManager();
        PlayerSpawn();

        // start gameplay mode, if not err
        Manager_Controller.Instance.SetGameplayMode();
        // Manager_Controller.Instance.SetInputPlayer();

        // StartIfNewGame();
    }

    // private void OnEnable()
    // {
    //     // Menambahkan event listener untuk sceneLoaded
    //     SceneManager.sceneLoaded += OnSceneLoaded;
    // }

    // private void OnDisable()
    // {
    //     // Menghapus event listener jika sudah tidak diperlukan
    //     SceneManager.sceneLoaded -= OnSceneLoaded;
    // }

    // void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    // {
    //     StartCoroutine(StartIfNewGameCoroutine());
    // }

    void PlayerSpawn()
    {
        Vector3 spawnPosition = Game_SaveSystem.Instance.GetPlayerPosition();

        GameObject player = Instantiate(PlayerPrefab, spawnPosition, Quaternion.identity);
        Manager_Player.Instance.RegisterPlayer(player);
        DontDestroyOnLoad(player);

        //setup inventory saveMnaghaer sync
        // player.GetComponent<Game_PlayerInventory>().inventory = new Inventory();
        Game_SaveSystem.Instance.LoadInventory(player.GetComponent<Game_PlayerInventory>());
        player.GetComponent<Game_PlayerInventory>().OnInventoryChanged += () =>
        {
            Game_SaveSystem.Instance.SaveInventory(player.GetComponent<Game_PlayerInventory>());
        };
    }

    public void StartIfNewGame()
    {
        StartCoroutine(StartIfNewGameCoroutine());
    }

    IEnumerator StartIfNewGameCoroutine()
    {
        Logger.LogNormal("InitGameManager", "waiting to check isNewGame flag...");
        // yield return new WaitForSecondsRealtime(10f);

        // Cek apakah Root_GameStartManager.Instance tidak null
        if (Root_GameStartManager.Instance == null)
        {
            Logger.LogError("InitGameManager", "Root_GameStartManager.Instance is null!");
            yield break;  // Hentikan coroutine jika instance null
        }

        Logger.LogNormal("InitGameManager", "isNewGame: " + Root_GameStartManager.Instance.IsNewGame());
        if (Root_GameStartManager.Instance.IsNewGame())
        {
            // activate director player cinematic intro
            PesawatMbeledos pesawat = FindObjectOfType<PesawatMbeledos>();
            if (pesawat == null)
            {
                Logger.LogError("InitGameManager", "PesawatMbeledos not found!");
                yield break;  // Hentikan jika PesawatMbeledos tidak ditemukan
            }

            PlayableDirector director = pesawat.GetComponent<PlayableDirector>();
            if (director == null)
            {
                Logger.LogError("InitGameManager", "PlayableDirector not found on PesawatMbeledos!");
                yield break;  // Hentikan jika PlayableDirector tidak ditemukan
            }

            director.Play();
        }
    }

    // IEnumerator StartIfNewGameCoroutine()
    // {
    //     Logger.LogNormal("InitGameManager", "waiting to check isNewGame flag...");
    //     // yield return new WaitForSeconds(10f); // waiting flag activate to know new game
    //     yield return new WaitForSecondsRealtime(10f); // menggunakan WaitForSecondsRealtime
    //     // int iterrasi = 0;
    //     // while (Manager_Player.Instance == null || Manager_Player.Instance.humanCtrl == null || iterrasi > 100)
    //     // {
    //     //     yield return null; // tunggu frame berikutnya
    //     //     iterrasi++;
    //     // }

    //     Logger.LogNormal("InitGameManager", "isNewGame: " + Root_GameStartManager.Instance.IsNewGame());
    //     if (Root_GameStartManager.Instance.IsNewGame())
    //     {
    //         // activate director player cinematic intro
    //         FindObjectOfType<PesawatMbeledos>().GetComponent<PlayableDirector>().Play();
    //     }
    // }
}
