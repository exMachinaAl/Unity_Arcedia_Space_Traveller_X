using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game_SeedManager : MonoBehaviour
{
    public static Game_SeedManager Instance;

    

    public int universeSeed;
    public long currentGalaxySeed;
    public long currentPlanetSeed;
    public List<long> currentGalaxyPlanetsSeed = new List<long>();

    void Awake()
    {
        if (Game_SeedManager.Instance != null && Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // // Generate or load existing seed
        // if (universeSeed == 0)
        //     universeSeed = Random.Range(int.MinValue, int.MaxValue);
    }

    public void Init(int rootSeed, long galaxySeed, long planetSeed)
    {
        universeSeed = rootSeed;
        currentGalaxySeed = galaxySeed;
        currentPlanetSeed = planetSeed;

        if (Root_GameStartManager.Instance != null)
        {
            int maxLoad = Root_GameStartManager.Instance.maxLoadPlanets;
            for (int i = 0; i < maxLoad; i++) // jff
            {
                long planetSeedGen = SeedUtil.SubSeed(currentGalaxySeed, i);
                currentGalaxyPlanetsSeed.Add(planetSeedGen);
            }
        }
    }
	// void Start()
    // {
    // 	// DebugBootstrapSetPlanetId();
    // }

    // private void DebugBootstrapSetPlanetId () { // ini cuma testing, nanti hapus
    // if (Game_SaveSystem.Instance != null) {
    //     int galaxySeed = SeedUtil.SubSeed(Game_SeedManager.Instance.universeSeed, 0);
    //     int planetSeed = SeedUtil.SubSeed(galaxySeed, 0);
    //     Game_SaveSystem.Instance.setCurrentPlanetId(planetSeed);
    // }
    // }
}
