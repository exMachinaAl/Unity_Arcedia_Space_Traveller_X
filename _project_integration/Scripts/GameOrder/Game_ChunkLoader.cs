using UnityEngine;
using System.Collections.Generic;

public class Game_ChunkLoader : MonoBehaviour
{
    public Transform player;
    public int chunkSize = 100;
    public int loadRadius = 2;
    private Dictionary<Vector2Int, GameObject> loadedChunks = new();

    void Update()
    {
        Vector3 globalPos = player.position + Game_FloatingOrigin.globalOffset;
        Vector2Int currentChunk = new Vector2Int(
            Mathf.FloorToInt(globalPos.x / chunkSize),
            Mathf.FloorToInt(globalPos.z / chunkSize)
        );

        // Load di sekitar player
        for (int x = -loadRadius; x <= loadRadius; x++)
        {
            for (int z = -loadRadius; z <= loadRadius; z++)
            {
                Vector2Int coord = new Vector2Int(currentChunk.x + x, currentChunk.y + z);
                if (!loadedChunks.ContainsKey(coord))
                    LoadChunk(coord);
            }
        }

        // Unload jauh
        List<Vector2Int> toUnload = new();
        foreach (var c in loadedChunks.Keys)
        {
            if (Vector2Int.Distance(c, currentChunk) > loadRadius)
                toUnload.Add(c);
        }

        foreach (var c in toUnload)
            UnloadChunk(c);
    }

    void LoadChunk(Vector2Int coord)
    {
        GameObject chunk = GameObject.CreatePrimitive(PrimitiveType.Cube);
        chunk.transform.localScale = new Vector3(chunkSize, 1, chunkSize);
        chunk.transform.position = new Vector3(coord.x * chunkSize, 0, coord.y * chunkSize);
        chunk.name = $"Chunk_{coord.x}_{coord.y}";
        loadedChunks[coord] = chunk;
    }

    void UnloadChunk(Vector2Int coord)
    {
        Destroy(loadedChunks[coord]);
        loadedChunks.Remove(coord);
    }

    void OnDrawGizmos()
    {
        if (player == null) return;
        Gizmos.color = Color.green;

        Vector3 globalPos = player.position + Game_FloatingOrigin.globalOffset;
        Vector2Int currentChunk = new Vector2Int(
            Mathf.FloorToInt(globalPos.x / chunkSize),
            Mathf.FloorToInt(globalPos.z / chunkSize)
        );

        // Visualisasi grid
        for (int x = -loadRadius; x <= loadRadius; x++)
        {
            for (int z = -loadRadius; z <= loadRadius; z++)
            {
                Vector2Int coord = new Vector2Int(currentChunk.x + x, currentChunk.y + z);
                Vector3 pos = new Vector3(coord.x * chunkSize, 0, coord.y * chunkSize);
                Gizmos.DrawWireCube(pos, new Vector3(chunkSize, 1, chunkSize));
            }
        }
    }
}
