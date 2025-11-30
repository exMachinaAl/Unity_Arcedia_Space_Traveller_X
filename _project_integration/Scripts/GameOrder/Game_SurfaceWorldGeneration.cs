using System.Collections.Generic;
using UnityEngine;

public class Game_SurfaceWorldGeneration : MonoBehaviour
{
    public Transform player;
    public ChunkSettings settings;
    public GameObject chunkPrefab;

    float maxSlopeGrass = 0.6f;
    int grassStep = 2;

    Queue<Chunk> VegetationSpawnQueue = new Queue<Chunk>();
    // Queue<Chunk> treeSpawnQueue = new Queue<Chunk>();

    // [SerializeField]public float[,] heightMap;

    Dictionary<Vector2Int, Chunk> loadedChunks = new Dictionary<Vector2Int, Chunk>();
    //public List<GameObject> spawnedObjects = new List<GameObject>();

    void Start()
    {
        // if (player == null && Manager_Player.Instance != null && Manager_Player.Instance.mode == PlayerMode.Human)
        // {
        //     player = Manager_Player.Instance.player.transform;
        // }
        // else if (player == null && Manager_Player.Instance != null && Manager_Player.Instance.mode == PlayerMode.Flight)
        // {
        //     player = Manager_Player.Instance.flightCtrl.shipTransform;
        // }
        // heightMap = new float[settings.vertexPerLine, settings.vertexPerLine];

        //settings re Set
        settings.seed = (int)Game_SaveSystem.Instance.getFullSaveData().lastWorld;

        player = Manager_Player.Instance.GetCurrentModePlayerTransform();

        // if (Manager)
        // Manager_Landing.Instance.surfaceRootPosition = transform;
    }
    void Update()
    {
        var playerCurrentStateWorld = Game_SaveSystem.Instance.GetPlayerInStateWorld();
        if (playerCurrentStateWorld == PlayerInThe.Atmosphere || playerCurrentStateWorld == PlayerInThe.Space) return;
        if (player == null)
            OnChangePlayerTransform();
        // if (Manager_Landing.Instance.isInAtmosphere || Manager_Landing.Instance.isInSpace) return;

        Vector2Int playerChunk = new Vector2Int(
            Mathf.FloorToInt(player.position.x / settings.chunkSize),
            Mathf.FloorToInt(player.position.z / settings.chunkSize)
        );

        List<Vector2Int> toLoad = new List<Vector2Int>();

        for (int y = -settings.viewDistance; y <= settings.viewDistance; y++)
        {
            for (int x = -settings.viewDistance; x <= settings.viewDistance; x++)
            {
                Vector2Int chunkCoord = new Vector2Int(playerChunk.x + x, playerChunk.y + y);

                if (!loadedChunks.ContainsKey(chunkCoord))
                    toLoad.Add(chunkCoord);
            }
        }

        // Sort by distance to player so nearest chunks load first
        toLoad.Sort((a, b) =>
        {
            float da = Vector2Int.Distance(a, playerChunk);
            float db = Vector2Int.Distance(b, playerChunk);
            return da.CompareTo(db);
        });

        // Load in sorted priority
        foreach (var c in toLoad)
            LoadChunkV2(c);

        // SpawnGrass();

        // tree spawner
        if (VegetationSpawnQueue.Count > 0)
        {
            Chunk c = VegetationSpawnQueue.Peek();

            // kalau chunk sudah hilang dari loadedChunks, skip
            if (!loadedChunks.ContainsKey(c.coord) || c.chunkObject == null)
            {
                VegetationSpawnQueue.Dequeue();
                return;
            }

            VegetationSpawnQueue.Dequeue();
            // SpawnTrees(c, c.coord);
            SpawnGrassV2(c, c.coord);
        }

        // unload chunks too far
        List<Vector2Int> toRemove = new List<Vector2Int>();
        foreach (var chunk in loadedChunks)
        {
            if (Mathf.Abs(chunk.Key.x - playerChunk.x) > settings.viewDistance ||
                Mathf.Abs(chunk.Key.y - playerChunk.y) > settings.viewDistance)
            {
                foreach (var obj in chunk.Value.spawnedObjects)
                {
                    if (obj != null)
                        obj.SetActive(false);
                }

                GameObject.Destroy(chunk.Value.chunkObject);
                toRemove.Add(chunk.Key);


            }
        }

        foreach (var c in toRemove)
            loadedChunks.Remove(c);



    }

    // void LoadChunk(Vector2Int coord)
    // {
    //     Chunk chunk = new Chunk(coord, chunkPrefab, transform);

    //     // Mesh mesh = ChunkGenerator.GenerateTerrainMesh(settings, coord);
    //     Mesh mesh = ChunkGenerator.GenerateTerrainMeshV2(settings, coord, heightMap);
    //     chunk.filter.mesh = mesh;

    //     // --- Tambahkan collider ---
    //     MeshCollider col = chunk.chunkObject.AddComponent<MeshCollider>();
    //     col.sharedMesh = mesh;
    //     //SpawnTrees(chunk, coord);
    //     chunk.SetPosition(new Vector3(coord.x * settings.chunkSize, 0, coord.y * settings.chunkSize));
    //     loadedChunks.Add(coord, chunk);

    //     VegetationSpawnQueue.Enqueue(chunk);
    // }

    void LoadChunkV2(Vector2Int coord)
    {
        Chunk chunk = new Chunk(coord, chunkPrefab, transform);

        // BUAT HEIGHTMAP PER-CHUNK (penting!)
        float[,] chunkHeightMap = new float[settings.vertexPerLine, settings.vertexPerLine];

        // Generate mesh dan isi chunkHeightMap
        Mesh mesh = ChunkGenerator.GenerateTerrainMeshV3(settings, coord, chunkHeightMap);
        chunk.filter.mesh = mesh;

        // Tambahkan collider
        MeshCollider col = chunk.chunkObject.AddComponent<MeshCollider>();
        col.sharedMesh = mesh;

        // set posisi chunk DI DUNIA sebelum enqueue spawn vegetasi
        chunk.SetPosition(new Vector3(coord.x * settings.chunkSize, 0, coord.y * settings.chunkSize));
        loadedChunks.Add(coord, chunk);

        // simpan heightmap ke chunk supaya spawn memakai data lokal
        chunk.heightMap = chunkHeightMap;

        // baru enqueue vegetasi
        VegetationSpawnQueue.Enqueue(chunk);
    }


    void SpawnTrees(Chunk chunk, Vector2Int coord)
    {
        // PRNG berdasarkan chunk + seed → konsisten
        // int hash = coord.x * 73856093 ^ coord.y * 19349663 ^ settings.seed;
        // Unity.Mathematics.Random prng = new Unity.Mathematics.Random((uint)hash);

        long planetId = Game_SaveSystem.Instance.getCurrentPlanetId();
        int hash = coord.x * 73856093 ^ coord.y * 19349663 ^ (int)planetId;
        Unity.Mathematics.Random prng = new Unity.Mathematics.Random((uint)hash);


        // int galaxySeed = SeedUtil.SubSeed(Game_SeedManager.Instance.universeSeed, 0);
        // int seedWorld = SeedUtil.SubSeed(galaxySeed, 0);

        int size = settings.chunkSize;

        for (int i = 0; i < settings.treesPerChunk; i++)
        {
            float x = prng.NextFloat(0, size);
            float z = prng.NextFloat(0, size);

            // world pos height check
            float worldX = coord.x * size + x;
            float worldZ = coord.y * size + z;

            long resourceId = SeedUtil.makeResourcesId((int)planetId, new Vector2Int(Mathf.RoundToInt(x), Mathf.RoundToInt(z)), i);


            // float noise = Mathf.PerlinNoise(
            //     (worldX + settings.seed) / settings.noiseScale,
            //     (worldZ + settings.seed) / settings.noiseScale
            // );

            // float noise = Mathf.PerlinNoise( //TEST MODE
            //     (worldX + planetId) / settings.noiseScale,
            //     (worldZ + planetId) / settings.noiseScale
            // );
            double nx = (worldX / (double)settings.noiseScale) + planetId * 0.0000000000001;
            double nz = (worldZ / (double)settings.noiseScale) + planetId * 0.0000000000001;

            float hght = Mathf.PerlinNoise((float)nx, (float)nz) * settings.maxHeight;
            // float hght = Mathf.PerlinNoise((float)nx, (float)nz) * settings.heightMultiplier;

            float height = Mathf.Round(hght * 1000f) / 1000f;

            // baru spawn pohon
            Vector3 pos = new Vector3(worldX, height, worldZ);

            if (prng.NextDouble() < 0.8f)
                continue;

            if (Game_SaveSystem.Instance.IsNodeDepleted(planetId, resourceId))
                continue;

            GameObject tree = Instantiate(settings.treePrefab, pos, Quaternion.identity, chunk.chunkObject.transform);
            chunk.spawnedObjects.Add(tree);
            tree.GetComponent<Game_ResourceNode>().Init(resourceId);
        }
    }

    public void SpawnGrassV2(Chunk chunk, Vector2Int coord)
    {
        float waterLevel = settings.waterLevelWorld;
        int vertexPerLine = settings.vertexPerLine;
        int seed = settings.seed;
        int size = settings.chunkSize;

        // chunk world offset (in world units)
        float chunkWorldX = coord.x * size;
        float chunkWorldZ = coord.y * size;

        for (int x = 0; x < vertexPerLine - 1; x += grassStep)
        {
            for (int z = 0; z < vertexPerLine - 1; z += grassStep)
            {
                // AMBIL DARI heightMap LOKAL milik chunk
                float h = chunk.heightMap[x, z];

                // 1. Jangan di air (bandingkan dengan waterLevel world-space)
                if (h < waterLevel)
                    continue;

                // 2. Slope (cek safe bound)
                float neighborH = chunk.heightMap[Mathf.Min(x + 1, vertexPerLine - 1), z];
                float slope = Mathf.Abs(neighborH - h);
                if (slope > maxSlopeGrass)
                    continue;

                // 3. Density — gunakan world pos untuk Perlin density
                float density = Mathf.PerlinNoise(
                    (chunkWorldX + x) * 0.12f + seed,
                    (chunkWorldZ + z) * 0.12f + seed
                );

                if (density < 0.5f)
                    continue;

                // 4. POSISI LOKAL untuk child chunk (x,z dalam range 0..chunkSize)
                Vector3 localPos = new Vector3(x, h, z);

                // Instantiate as child and set localPosition — ini menghindari double-offset
                GameObject grass = Instantiate(settings.grassPrefab, chunk.chunkObject.transform);
                grass.transform.localPosition = localPos;
                grass.transform.localRotation = Quaternion.identity;

                chunk.spawnedObjects.Add(grass);
            }
        }
    }

    // public void SpawnGrass(Chunk chunk, Vector2Int coord)
    // {
    //     float waterLevel = settings.waterLevel;
    //     int vertexPerLine = settings.vertexPerLine;
    //     int seed = settings.seed;
    //     int size = settings.chunkSize;

    //     for (int x = 0; x < vertexPerLine - 1; x += grassStep)
    //     {
    //         for (int z = 0; z < vertexPerLine - 1; z += grassStep)
    //         {
    //             float h = heightMap[x, z];

    //             // 1. Jangan di air
    //             if (h < waterLevel)
    //                 continue;

    //             // 2. Hitung slope
    //             float slope = Mathf.Abs(heightMap[x + 1, z] - h);

    //             // 3. Jangan di tanah curam
    //             if (slope > maxSlopeGrass)
    //                 continue;

    //             // 4. Density (agar tidak semua titik ditanami)
    //             float density = Mathf.PerlinNoise(
    //                 (coord.x + x) * 0.15f + seed,
    //                 (coord.y + z) * 0.15f + seed
    //             );

    //             if (density < 0.5f)
    //                 continue;

    //             // 5. Posisi world
    //             float worldX = coord.x * size + x;
    //             float worldZ = coord.y * size + z; // jan bingung lah ya, apa kenapa itu Y, itu Vector2
    //             Vector3 spawnPos = new Vector3(worldX, h, worldZ);

    //             Debug.DrawLine(
    //                 new Vector3(worldX, h + 20, worldZ),
    //                 new Vector3(worldX, h, worldZ),
    //                 Color.red,
    //                 10f
    //             );

    //             GameObject grass = Instantiate(settings.grassPrefab, spawnPos, Quaternion.identity, chunk.chunkObject.transform);
    //             // GameObject grass = Instantiate(
    //             //     settings.grassPrefab,
    //             //     chunk.chunkObject.transform
    //             // );
    //             // grass.transform.localPosition = spawnPos;
    //             chunk.spawnedObjects.Add(grass);
    //         }
    //     }
    // }


    public void OnChangePlayerTransform()
    {
        // if (Manager_Player.Instance != null && Manager_Player.Instance.mode == PlayerMode.Human)
        // {
        //     player = Manager_Player.Instance.player.transform;
        // }
        // else if (Manager_Player.Instance != null && Manager_Player.Instance.mode == PlayerMode.Flight)
        // {
        //     player = Manager_Player.Instance.flightCtrl.shipTransform;
        // }
        player = Manager_Player.Instance.GetCurrentModePlayerTransform();
    }

}
