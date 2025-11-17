using UnityEngine;

public static class SeedUtil
{
    public static long SubSeed(long parentSeed, int index)
    {
        unchecked
        {
            return parentSeed * 73856093 ^ index * 19349663;
        }
    }
    public static int SubSeed(int parentSeed, int index)
    {
        unchecked
        {
            return parentSeed * 73856093 ^ index * 19349663;
        }
    }

    // public static int PlanetSeedChain(int parentSeed, , int index)
    // {
    //     unchecked
    //     {
    //         return parentSeed * 73856093 ^ index * 19349663;
    //     }
    // }

    public static long makeResourcesId(int planetSeed, Vector2Int chunk, int nodeIndex)
    {
        unchecked
        {
            long hash = planetSeed;
            hash = hash * 31 + chunk.x;
            hash = hash * 31 + chunk.y;
            hash = hash * 31 + nodeIndex;
            return hash;
        }
    }
    public static Unity.Mathematics.Random GetRNG(int seed, int chunkX, int chunkZ, int planetIndex)
    {
        int finalSeed = seed;
        finalSeed = finalSeed * 73856093 ^ chunkX * 19349663 ^ chunkZ * 83492791 ^ planetIndex;
        return new Unity.Mathematics.Random((uint)finalSeed);
    }
}
