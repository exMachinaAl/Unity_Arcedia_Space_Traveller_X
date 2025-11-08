using UnityEngine;

public class Game_UniverseManager : MonoBehaviour
{
    public static Game_UniverseManager Instance;

    public int universeSeed;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); 

        // Generate or load existing seed
        if (universeSeed == 0)
            universeSeed = Random.Range(int.MinValue, int.MaxValue);
    }
	
	void Start()
	{
		DebugBootstrapSetPlanetId();
	}
	
	private void DebugBootstrapSetPlanetId () { // ini cuma testing, nanti hapus
        if (Game_SaveSystem.Instance != null) {
            int galaxySeed = SeedUtil.SubSeed(Game_UniverseManager.Instance.universeSeed, 0);
            int planetSeed = SeedUtil.SubSeed(galaxySeed, 0);
            Game_SaveSystem.Instance.setCurrentPlanetId(planetSeed);
        }
    }
}
