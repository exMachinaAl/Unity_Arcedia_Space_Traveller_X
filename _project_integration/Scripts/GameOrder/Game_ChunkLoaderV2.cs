using UnityEngine;
using System.Collections.Generic;

public class Game_ChunkLoaderV2 : MonoBehaviour
{
    public Transform player;
    public int chunkSize = 100;
    public int viewDistance = 3;
    public GameObject chunkPrefab;

    private Dictionary<Vector2Int, GameObject> activeChunks = new();

    void Update()
    {
        if (player == null || chunkPrefab == null)
            return;

        // Hitung posisi chunk tempat player berada sekarang
        Vector2Int playerChunk = new Vector2Int(
            Mathf.FloorToInt(player.position.x / chunkSize),
            Mathf.FloorToInt(player.position.z / chunkSize) // z dunia → y Vector2Int
        );

        // Loop semua chunk dalam jarak pandang
        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int y = -viewDistance; y <= viewDistance; y++) // pakai y bukan z
            {
                Vector2Int chunkCoord = new Vector2Int(playerChunk.x + x, playerChunk.y + y);

                if (!activeChunks.ContainsKey(chunkCoord))
                {
                    Vector3 chunkPos = new Vector3(chunkCoord.x * chunkSize, 0, chunkCoord.y * chunkSize);
                    GameObject newChunk = Instantiate(chunkPrefab, chunkPos, Quaternion.identity);
                    newChunk.name = $"Chunk_{chunkCoord.x}_{chunkCoord.y}";
                    activeChunks[chunkCoord] = newChunk;
                }
            }
        }
    }
}
