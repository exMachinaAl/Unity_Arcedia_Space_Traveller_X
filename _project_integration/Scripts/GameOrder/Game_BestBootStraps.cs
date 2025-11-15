using UnityEngine;
public static class Game_BestBootStraps
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        Debug.Log("Game_BestBootStraps Init testing for every run");
        CreateIfNotExists<Game_SaveSystem>("----Game_SaveSystem----");
        CreateIfNotExists<Root_GameStartManager>("_StarterManager_");
        CreateIfNotExists<QuestManager>("----QuestManager----");
        CreateIfNotExists<UIQuest>("----UIManager----"); // ini hanya sementara, karena harus ui manager global
        CreateIfNotExists<Game_SeedManager>("---SeedManager---");
        CreateIfNotExists<Manager_Player>("---PlayerManager---");
        CreateIfNotExists<Manager_Landing>("---LandingManager---");
        // CreateIfNotExists<ChunkManager>("ChunkManager");

        //starting game
        // Root_GameStartManager.Instance.InitGameManager();
        // var GameManager = FindObjectOfType<GameManager>();
        // manager.Init();
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