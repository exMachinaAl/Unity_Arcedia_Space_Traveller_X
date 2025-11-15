using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    }

    void PlayerSpawn()
    {
        GameObject player = Instantiate(PlayerPrefab, new Vector3(0, 9, 0), Quaternion.identity);
        Manager_Player.Instance.RegisterPlayer(player);
        DontDestroyOnLoad(player);
    }


}
