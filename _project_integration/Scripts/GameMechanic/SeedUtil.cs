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
}
