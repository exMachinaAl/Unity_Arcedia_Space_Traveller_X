using UnityEngine;

public static class ChunkGenerator
{
    public static Mesh GenerateTerrainMesh(ChunkSettings settings, Vector2Int chunkCoord) 
    {
        int size = settings.chunkSize;
        float scale = settings.noiseScale;
        float height = settings.heightMultiplier;

        Vector3[] vertices = new Vector3[(size + 1) * (size + 1)];
        int[] triangles = new int[size * size * 6];

        int triIndex = 0;
        int i = 0;

        for (int z = 0; z <= size; z++)
        {
            for (int x = 0; x <= size; x++)
            {
                float worldX = x + chunkCoord.x * size;
                float worldZ = z + chunkCoord.y * size;

                float noise = Mathf.PerlinNoise(
                    (worldX + settings.seed) / scale,
                    (worldZ + settings.seed) / scale
                );

                vertices[i] = new Vector3(x, noise * height, z);

                if (x < size && z < size)
                {
                    triangles[triIndex + 0] = i;
                    triangles[triIndex + 1] = i + size + 1;
                    triangles[triIndex + 2] = i + 1;

                    triangles[triIndex + 3] = i + 1;
                    triangles[triIndex + 4] = i + size + 1;
                    triangles[triIndex + 5] = i + size + 2;

                    triIndex += 6;
                }

                i++;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }
}
