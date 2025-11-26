using UnityEngine;

[CreateAssetMenu(fileName = "ChunkSettings", menuName = "World/Chunk Settings")]
public class ChunkSettings : ScriptableObject
{
    public int chunkSize = 32;
    public float noiseScale = 10f;
    public float heightMultiplier = 5f;
    public int viewDistance = 3; // chunk radius
    
    // public float noiseScale = 100f;
    public float baseHeight = -20f;
    public float maxHeight = 80f;
    public int vertexPerLine = 33;

    [Header("terrain Levels")]
    public float grassMin = 0.25f;
    public float grassMax = 0.55f;
    public float grassFlatLevel = 0.32f;
    public float mountainStrength = 0.6f;

    [Header("Height Levels")]
    public float waterLevel = 0.25f;
    public float sandLevel  = 0.35f;
    public float grassLevel = 0.60f;
    public float hillLevel  = 0.80f;

    [Header("Colors")]
    public Gradient terrainGradient;

    [Header("Vegetattion")]
    public GameObject grassPrefab;
    public GameObject treePrefab;
    public int treesPerChunk = 5;
    public int seed = 12345;
}
