using UnityEngine;
public static class Game_BestBootStraps
{
    private const string DebugModeKey = "IsDebugMode";

    static Game_BestBootStraps()
    {
        // Membaca nilai debug mode dari PlayerPrefs
        if (!PlayerPrefs.HasKey(DebugModeKey))
        {
            PlayerPrefs.SetInt(DebugModeKey, 0); // Default off
        }
    }
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        bool isDebugMode = PlayerPrefs.GetInt(DebugModeKey, 0) == 1;

        if (isDebugMode)
        {
            Debug.Log("Skipping auto run logic for debugging");
            return; // Menghentikan eksekusi jika flag aktif
        }

        // Logic yang seharusnya dijalankan
        Debug.Log("[Bootsraps] Running auto run logic! via script");
        Debug.Log("Game_BestBootStraps Init testing for every run");

        CreateIfNotExists<Game_SaveSystem>("----Game_SaveSystem----");
        CreateIfNotExists<Root_GameStartManager>("_StarterManager_");
        // CreateIfNotExists<Manager_Quest>("----QuestManager----");
        // CreateIfNotExists<Manager_UI>("----UIManager----"); // ini hanya sementara, karena harus ui manager global
        CreateIfNotExists<Game_SeedManager>("---SeedManager---");
        CreateIfNotExists<Manager_Player>("---PlayerManager---");
        CreateIfNotExists<Manager_Controller>("---ControllerManager---");
        CreateIfNotExists<Manager_Landing>("---LandingManager---");
        CreateIfNotExists<Manager_Audio>("---AudioManager---");
        // CreateIfNotExists<ChunkManager>("ChunkManager");

        //starting game
        // Root_GameStartManager.Instance.InitGameManager();
        // var GameManager = FindObjectOfType<GameManager>();
        // manager.Init();
    }

    public static void ToggleDebugMode()
    {
        bool currentMode = PlayerPrefs.GetInt(DebugModeKey, 0) == 1;
        PlayerPrefs.SetInt(DebugModeKey, currentMode ? 0 : 1);
        PlayerPrefs.Save();
        Debug.Log($"Debug Mode {(currentMode ? "Disabled" : "Enabled")}");
        Debug.Log($"Bootsraps Script [{(currentMode ? "Activate" : "Disabled")}]");
    }

    static void CreateIfNotExists<T>(string name) where T : Component
    {
        if (GameObject.FindObjectOfType<T>() == null)
        {
            var obj = new GameObject(name);
            obj.AddComponent<T>();
            Object.DontDestroyOnLoad(obj);
        }
    }
}