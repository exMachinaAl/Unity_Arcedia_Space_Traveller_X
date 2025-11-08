using UnityEngine;

[CreateAssetMenu(fileName = "ChunkSettings", menuName = "World/Chunk Settings")]
public class ChunkSettings : ScriptableObject
{
    public int chunkSize = 32;
    public float noiseScale = 10f;
    public float heightMultiplier = 5f;
    public int viewDistance = 3; // chunk radius

    public GameObject treePrefab;
public int treesPerChunk = 5;
    public int seed = 12345;
}
