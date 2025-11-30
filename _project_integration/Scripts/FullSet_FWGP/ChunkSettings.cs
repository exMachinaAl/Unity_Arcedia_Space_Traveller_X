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
    public float grassMax = 0.35f;
    public float grassFlatLevel = 0.32f;
    public float mountainStrength = 0.6f;

    [Header("Height Levels")]
    public float waterLevelWorld = 2f;
    public Color waterColor = Color.blue;
    public float sandLevelWorld = 5f;
    public Color sandColor = new Color(0.9f, 0.85f, 0.5f);
    public float grassLevelWorld = 15f;
    public Color grassColor = Color.green;
    public float hillLevelWorld  = 35f;
    public Color hillColor = new Color(0.3f, 0.5f, 0.2f);

    [Header("Colors")]
    public Gradient terrainGradient;

    [Header("Vegetattion")]
    public GameObject grassPrefab;
    public GameObject treePrefab;
    public int treesPerChunk = 5;
    public int seed = 12345;
}
