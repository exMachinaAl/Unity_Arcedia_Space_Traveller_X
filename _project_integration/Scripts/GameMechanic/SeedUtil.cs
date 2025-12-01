using UnityEngine;

public static class SeedUtil
{
    public static long SubSeed(long parentSeed, int index)
    {
        unchecked
        {
            long result = parentSeed * 73856093;
            result ^= index * 19349663;

            if (result < 0) 
            {
                Logger.LogWarning("SEED UTIL", "Overflow detected!");
            }
            return result;
        }
    }
    public static int SubSeed(int parentSeed, int index)
    {
        unchecked
        {
            return parentSeed * 73856093 ^ index * 19349663;
        }
    }

    public static int ReverseSubSeed(int parentSeed, int resultingSeed)
    {
        // Coba semua nilai index dalam rentang yang mungkin
        for (int index = 0; index < int.MaxValue; index++)
        {
            // Hitung seed dengan index yang saat ini
            int subSeed = SubSeed(parentSeed, index);

            // Jika cocok dengan resultingSeed, kembalikan index
            if (subSeed == resultingSeed)
            {
                return index;
            }
        }

        // Jika tidak ditemukan, kembalikan nilai tidak valid
        return -1;
    }


    public static long SurfaceSeedV3(long parentSeed, int index)
    {
        unchecked
        {
            long result = parentSeed * 73856093;
            result ^= index * 19349663;

            // Membatasi hasil seed untuk tidak melebihi 7 digit
            result = result % 10000000;  // Membatasi hasil seed pada 7 digit (10 juta)

            if (result < 0)
            {
                Logger.LogWarning("SEED UTIL", "Overflow detected!");
            }
            return result;
        }
    }

    public static long SurfaceSeedV2(long parentSeed, int index)
    {
        unchecked
        {
            long result = parentSeed * 73856093;
            result ^= index * 19349663;

            // Jika ini adalah seed untuk surface world, batasi besar seed
            // if (isSurfaceWorldGeneration)  // Misalnya, gunakan flag untuk menentukan
            {
                result = result & 0x7FFFFFFF;  // Batasi untuk menggunakan hanya nilai positif dalam rentang int
            }

            if (result < 0)
            {
                Logger.LogWarning("SEED UTIL", "Overflow detected!");
            }
            return result;
        }
    }
    public static long GetHashedSeed(long seed)
    {
        // Gunakan hash untuk memperkecil ukuran seed, misalnya dengan menggunakan XOR atau hash sederhana
        unchecked
        {
            long hashedSeed = seed ^ (seed >> 32);  // Contoh hashing sederhana dengan XOR
            return hashedSeed & 0x7FFFFFFF;  // Ambil hanya bagian positif
        }
    }




    // public static int PlanetSeedChain(int parentSeed, , int index)
    // {
    //     unchecked
    //     {
    //         return parentSeed * 73856093 ^ index * 19349663;
    //     }
    // }

    public static long MakePlanetId(int galaxySeed, int chunkX, int chunkY, int nodeIndex)
    {
        unchecked
        {
            long hash = galaxySeed;
            hash = hash * 31 + chunkX;
            hash = hash * 31 + chunkY;
            hash = hash * 31 + nodeIndex;
            return hash;
        }
    }
    public static long MakeResourcesId(int planetSeed, int chunkX, int chunkY, int nodeIndex)
    {
        unchecked
        {
            long hash = planetSeed;
            hash = hash * 31 + chunkX;
            hash = hash * 31 + chunkY;
            hash = hash * 31 + nodeIndex;
            return hash;
        }
    }
    // public static long makeResourcesId(int planetSeed, Vector2Int chunk, int nodeIndex)
    // {
    //     unchecked
    //     {
    //         long hash = planetSeed;
    //         hash = hash * 31 + chunk.x;
    //         hash = hash * 31 + chunk.y;
    //         hash = hash * 31 + nodeIndex;
    //         return hash;
    //     }
    // }
    public static Unity.Mathematics.Random GetRNG(int seed, int chunkX, int chunkZ, int planetIndex)
    {
        int finalSeed = seed;
        finalSeed = finalSeed * 73856093 ^ chunkX * 19349663 ^ chunkZ * 83492791 ^ planetIndex;
        return new Unity.Mathematics.Random((uint)finalSeed);
    }
}
