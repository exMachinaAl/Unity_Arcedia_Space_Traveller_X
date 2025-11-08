using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public Transform player;
    public ChunkSettings settings;
    public GameObject chunkPrefab;

Queue<Chunk> treeSpawnQueue = new Queue<Chunk>();

    Dictionary<Vector2Int, Chunk> loadedChunks = new Dictionary<Vector2Int, Chunk>();
    //public List<GameObject> spawnedObjects = new List<GameObject>();


    void Update()
    {
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
    LoadChunk(c);

if (treeSpawnQueue.Count > 0)
{
    Chunk c = treeSpawnQueue.Peek();

    // kalau chunk sudah hilang dari loadedChunks, skip
    if (!loadedChunks.ContainsKey(c.coord) || c.chunkObject == null)
    {
        treeSpawnQueue.Dequeue();
        return;
    }

    treeSpawnQueue.Dequeue();
    SpawnTrees(c, c.coord);
}






        // for (int y = -settings.viewDistance; y <= settings.viewDistance; y++)
        // {
        //     for (int x = -settings.viewDistance; x <= settings.viewDistance; x++)
        //     {
        //         Vector2Int chunkCoord = new Vector2Int(playerChunk.x + x, playerChunk.y + y);

        //         if (!loadedChunks.ContainsKey(chunkCoord))
        //             LoadChunk(chunkCoord);
        //     }
        // }

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

    void LoadChunk(Vector2Int coord)
    {
        Chunk chunk = new Chunk(coord, chunkPrefab, transform);

        Mesh mesh = ChunkGenerator.GenerateTerrainMesh(settings, coord);
        chunk.filter.mesh = mesh;

        // --- Tambahkan collider ---
        MeshCollider col = chunk.chunkObject.AddComponent<MeshCollider>();
        col.sharedMesh = mesh;
        //SpawnTrees(chunk, coord);
        treeSpawnQueue.Enqueue(chunk);


        chunk.SetPosition(new Vector3(coord.x * settings.chunkSize, 0, coord.y * settings.chunkSize));
        loadedChunks.Add(coord, chunk);
    }

void SpawnTrees(Chunk chunk, Vector2Int coord)
{
        // PRNG berdasarkan chunk + seed → konsisten
        // int hash = coord.x * 73856093 ^ coord.y * 19349663 ^ settings.seed;
        // Unity.Mathematics.Random prng = new Unity.Mathematics.Random((uint)hash);

        long planetId = Game_SaveSystem.Instance.getCurrentPlanetId();
        int hash = coord.x * 73856093 ^ coord.y * 19349663 ^ (int)planetId;
    Unity.Mathematics.Random prng = new Unity.Mathematics.Random((uint)hash);


    // int galaxySeed = SeedUtil.SubSeed(Game_UniverseManager.Instance.universeSeed, 0);
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

            float noise = Mathf.PerlinNoise( //TEST MODE
                (worldX + planetId) / settings.noiseScale,
                (worldZ + planetId) / settings.noiseScale
            );
            float height = noise * settings.heightMultiplier;

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


}
