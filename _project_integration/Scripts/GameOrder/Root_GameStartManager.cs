using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class Root_GameStartManager : MonoBehaviour
{
    public RNG_PlanetData planetDataConfigGen;
    public static Root_GameStartManager Instance;
    // Start is called before the first frame update
    public static bool isDebugMode = true;
    public int maxLoadPlanets = 5;

    public bool isNewGame = true;
    void Awake()
    {
        if (Root_GameStartManager.Instance != null && Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }



    void Start()
    {
        // StartingGame();
    }
    public void InitGameManager()
    {
        // init game manager here
        var data = Root_GameInitManager.Instance;
        planetDataConfigGen = data.planetDataConfigGen;

        // start gameLogical
        StartingGame();
    }

    bool StartingGame()
    {
        if (Game_SaveSystem.Instance != null && Game_SaveSystem.Instance.save != null)
        {
            if (Game_SaveSystem.Instance.save.playerName == null || Game_SaveSystem.Instance.save.playerName == "")
            {
                Debug.Log("No existing player data found. Starting new player.");
                isNewGame = true;
                return StartNewPlayer();
            }
            else
            {
                Debug.Log("Existing player data found. Loading player.");
                isNewGame = false;
                return LoadExistingPlayer();
            }
        }
        else
        {
            Debug.LogError("Save system not found! and game error: 21RGM");
            return false;
        }


    }

    bool StartNewPlayer()
    {
        Game_SaveSystem.Instance.setNewGame();
        var loadData = Game_SaveSystem.Instance.getFullSaveData();

        Game_SeedManager.Instance.Init(loadData.universeSeed, loadData.galaxySeed, loadData.lastWorldSeed);
        SceneManager.LoadScene("Template_SpaceWorld");
        // SceneManager.LoadScene("Template_UnderWorld");
        Debug.Log("New Game created");


        return true;
    }
    bool LoadExistingPlayer()
    {
        var loadData = Game_SaveSystem.Instance.getFullSaveData();

        // set load to game
        Game_SeedManager.Instance.Init(loadData.universeSeed, loadData.galaxySeed, loadData.lastWorldSeed);

        // if (loadData.playerInThe != PlayerInThe.Space) {
        if (loadData.playerInThe == PlayerInThe.Space)
        {
            SceneManager.LoadScene("Template_SpaceWorld");
        }
        else
        {
            SceneManager.LoadScene("Template_UnderWorld");
        }

        Debug.Log("Loaded player: " + loadData.playerName);
        // set current planet id #######
        // int galaxySeed = SeedUtil.SubSeed(Game_SeedManager.Instance.universeSeed, 0);
        // int planetSeed = SeedUtil.SubSeed(galaxySeed, 0);
        // Game_SaveSystem.Instance.setCurrentPlanetId(planetSeed);
        return true;
    }
    public bool IsNewGame()
    {
        return isNewGame;
    }

}
