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
    public float mountainLevelWorld  = 50;
    public Color mountainColor = Color.gray;

    [Header("Vegetation Slope")]
    public float minGrassSpacing = 0.05f;
    public float maxGrassSpacing = 0.5f;
    public float grassDensityThreshold = 0.5f;
    public float maxSlopeGrass = 0.6f;

    public float bushDensityThreshold = 0.6f;
    
    public float minTreeDistance = 4.5f;
    public float maxSlopeTree = 2.2f;
    public int treesPerChunk = 8;

    public float maxSlopeRock = 3.2f;
    public float minRockDistance = 5f;
    public int rocksPerChunk = 3;

    [Header("Colors")]
    public Gradient terrainGradient;
    

    [Header("Vegetattion")]
    public GameObject grassPrefab;
    public GameObject treePrefab;
    public GameObject bushPrefab; 
    public GameObject rockPrefab;

    public int seed = 12345;
}




// [CreateAssetMenu(fileName = "ChunkSettings", menuName = "World/Chunk Settings")]
// public class ChunkSettings : ScriptableObject
// {
//     public int chunkSize = 32;
//     public int vertexPerLine = 33;
//     public int viewDistance = 3;

//     [Header("Noise")]
//     public float noiseScale = 10f;
//     public float heightMultiplier = 5f;
//     public int seed = 12345;

//     [Header("Height World Units")]
//     public float baseHeight = -20f;
//     public float maxHeight = 80f;

//     [Header("Biome Levels (WORLD HEIGHT)")]
//     public float waterLevelWorld = 2f;
//     public float sandLevelWorld  = 5f;
//     public float grassLevelWorld = 15f;
//     public float hillLevelWorld  = 35f;

//     [Header("Vegetation")]
//     public GameObject grassPrefab;
//     public GameObject treePrefab;
// }
