using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


[System.Serializable]
public class PlanetInformation
{
    public long planetId;
    public string planetName;
    public long planetSeed;
    public Vector3 position;
    public float size;
    public float atmosphereThickness;
    public float radius;
    public float mineralDensity;
    public float mineralMaxRichness;
}

public class Game_PlanetGenerationDataV2 : MonoBehaviour
{
    public int viewRange;
    public int chunkSize;
    public long galaxySeed;
    public Transform player;
    public Camera playerCam;
    public GameObject planetPrefab;
    public Transform generationRoot;
    

    public Dictionary<Vector2Int, List<PlanetInformation>> generatedChunks = new();
    public Dictionary<long, GameObject> spawnedPlanets = new();

    void Start()
    {
        // lot
    }

    void Update()
    {
        try
        {
            if (player == null && Manager_Player.Instance != null)
                player = Manager_Player.Instance.flightCtrl.shipTransform;

            if (galaxySeed == 0 && Game_SeedManager.Instance != null)
                galaxySeed = Game_SeedManager.Instance.currentGalaxySeed;

            if (galaxySeed == 0 || player == null)
                return;
        }
        catch (System.NullReferenceException ex)
        {
            Debug.LogWarning("kemungkinankarena anda nggak di pesawat dan di space, sementara error, ato debug" + ex.Message);
            return;
        }

            UpdateVisibleChunks();
    }

    void UpdateVisibleChunks()
    {
        Vector3 p = player.position;
        int cx = Mathf.FloorToInt(p.x / chunkSize);
        int cz = Mathf.FloorToInt(p.z / chunkSize);

        var keepPlanetIDs = new HashSet<long>();
        var keepChunks = new HashSet<Vector2Int>();

        for (int x = cx - viewRange; x <= cx + viewRange; x++)
        {
            for (int z = cz - viewRange; z <= cz + viewRange; z++)
            {
                var key = new Vector2Int(x, z);
                keepChunks.Add(key);

                if (!generatedChunks.ContainsKey(key))
                {
                    generatedChunks[key] = GeneratePlanetsInChunk((int)galaxySeed, x, z, 2000f, 3);
                }

                foreach (var data in generatedChunks[key])
                {
                    keepPlanetIDs.Add(data.planetId);

                    if (!spawnedPlanets.ContainsKey(data.planetId))
                        SpawnPlanet(data);
                }
            }
        }

        // UNLOAD PLANETS
        foreach (var id in spawnedPlanets.Keys.ToList())
        {
            if (!keepPlanetIDs.Contains(id))
            {
                Destroy(spawnedPlanets[id]);
                spawnedPlanets.Remove(id);
            }
        }

        // UNLOAD CHUNKS
        foreach (var chunk in generatedChunks.Keys.ToList())
        {
            if (!keepChunks.Contains(chunk))
                generatedChunks.Remove(chunk);
        }
    







        // Vector3 p = player.position;

            // int cx = Mathf.FloorToInt(p.x / 10000);
            // int cz = Mathf.FloorToInt(p.z / 10000);


            // List<int> keepPlanetIDs = new();
            // int viewRange = 1; // chunk radius

            // for (int x = cx - viewRange; x <= cx + viewRange; x++)
            // {
            //     for (int z = cz - viewRange; z <= cz + viewRange; z++)
            //     {
            //         Vector2Int key = new(x, z);

            //         if (!generatedChunks.ContainsKey(key))
            //         {
            //             // var planets = GeneratePlanetsInChunk((int)galaxySeed, x, z, 500f, 4);
            //             var planets = GeneratePlanetsInChunk((int)galaxySeed, x, z, 2000f, 3);
            //             generatedChunks[key] = planets;

            //             foreach (var pData in planets)
            //             {
            //                 //keepPlanetIDs.Add(pData.planetId);

            //                 // Debug.Log("masih deteksi kemunculuna planet");
            //                 if (!spawnedPlanets.ContainsKey(pData.planetId))
            //                 {
            //                     // Debug.Log("memunculkan gameObject planet");
            //                     SpawnPlanet(pData); // instantiate prefab
            //                 }
            //             }
            //         }

            //         foreach (var pData in generatedChunks[key])
            //         {
            //             keepPlanetIDs.Add(pData.planetId);
            //         }
            //     }
            // }

            // // unloading bisa kamu tambahkan nanti
            // var keys = spawnedPlanets.Keys.ToList();
            // foreach (int id in keys)
            // {
            //     Debug.Log($"ini id ap sih: {id}");
            //     if (!keepPlanetIDs.Contains(id))
            //     {
            //         Debug.Log("menghapus chunk yg g ke render");
            //         Destroy(spawnedPlanets[id]);
            //         spawnedPlanets.Remove(id);
            //     }
            // }
        }



    public List<PlanetInformation> GeneratePlanetsInChunk(int rootSeed, int chunkX, int chunkZ, float minDistance, int planetsPerChunk)
    {
        List<PlanetInformation> planets = new List<PlanetInformation>();

        for (int i = 0; i < planetsPerChunk; i++)
        {
            var rng = SeedUtil.GetRNG(rootSeed, chunkX, chunkZ, i);

            float radius = rng.NextInt(50, 500);  // bebas, tergantung desain planet

            // generate posisi lokal dalam chunk
            float x = rng.NextInt(-5000, 5000);
            float z = rng.NextInt(-5000, 5000);
            float y = rng.NextInt(-1000, 1000); // atur ketinggian space

            Vector3 pos = new Vector3(
                chunkX * 10000 + x,
                y,
                chunkZ * 10000 + z
            );

            // cek tubrukan planet dengan planet lain di chunk
            bool ok = true;
            foreach (var existing in planets)
            {
                float dist = Vector3.Distance(existing.position, pos);
                if (dist < (existing.radius + radius) + minDistance)
                {
                    ok = false;
                    break;
                }
            }

            if (!ok) continue; // gagal, planet ini di-skip

            planets.Add(new PlanetInformation
            {
                // planetId = i + chunkX * 100 + chunkZ * 1000,
                planetId = SeedUtil.MakePlanetId((int)galaxySeed, chunkX, chunkZ, i),
                planetName = "Planet_" + i,
                planetSeed = SeedUtil.SubSeed(galaxySeed, i),
                position = pos,
                radius = radius,
                mineralMaxRichness = rng.NextInt(0, 5001)
            });
        }

        return planets;
    }

    void SpawnPlanet(PlanetInformation pd)
    {
        GameObject obj = Instantiate(planetPrefab, pd.position, Quaternion.identity, generationRoot);
        // obj.transform.localScale = Vector3.one * (pd.radius * 2);
        obj.name = pd.planetName;

        
        // obj.AddComponent<Game_PlanetFullInformation>();
        obj.GetComponent<Game_PlanetFullInformation>().Init(pd);

        spawnedPlanets[pd.planetId] = obj;
    }
}
