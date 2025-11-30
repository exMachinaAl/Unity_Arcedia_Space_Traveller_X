using UnityEngine;

public static class ChunkGenerator
{
    // public static float FBM(float worldX, float worldZ, int seed, float noiseScale)
    // {
    //     float total = 0f;
    //     float amplitude = 1f;
    //     float frequency = 1f;

    //     for(int i = 0; i < 5; i++){
    //         float x = (worldX + seed) / noiseScale * frequency;
    //         float z = (worldZ + seed) / noiseScale * frequency;

    //         total += Mathf.PerlinNoise(x, z) * amplitude;

    //         amplitude *= 0.5f;
    //         frequency *= 2f;
    //     }

    //     return total; // 0..1 kira-kira
    // }
    public static float FBM(float worldX, float worldZ, int seed, float noiseScale)
    {
        float total = 0f;
        float amplitude = 1f;
        float frequency = 1f;

        for(int i = 0; i < 5; i++){
            float x = (worldX + seed) / noiseScale * frequency;
            float z = (worldZ + seed) / noiseScale * frequency;

            total += Mathf.PerlinNoise(x, z) * amplitude;

            amplitude *= 0.5f;
            frequency *= 2f;
        }

        // Optional: normalize by max amplitude (1 + 0.5 + 0.25 + ...) -> 1.96875
        float maxAmp = 1f;
        float amp = 1f;
        for(int i = 1; i < 5; i++){
            amp *= 0.5f;
            maxAmp += amp;
        }
        total /= maxAmp;

        return total; // approximately 0..1
    }


    public static Mesh GenerateTerrainMeshV3(ChunkSettings settings, Vector2Int chunkCoord, float[,] heightMap)
    {
        float minH = float.MaxValue;
        float maxH = float.MinValue;
        float avgH = 0f;

        int chunkSize = settings.chunkSize;
        int vertexPerLine = settings.vertexPerLine;
        int seed = settings.seed;
        float baseHeight = settings.baseHeight;
        float maxHeight = settings.maxHeight;

        float grassMin = settings.grassMin;
        float grassMax = settings.grassMax;
        float grassFlatLevel = settings.grassFlatLevel;
        float mountainStrength = settings.mountainStrength;

        Gradient terrainGradient = settings.terrainGradient;

        int vCount = vertexPerLine * vertexPerLine;
        Vector3[] vertices = new Vector3[vCount];
        int[] triangles = new int[(vertexPerLine - 1) * (vertexPerLine - 1) * 6];
        Color[] colors = new Color[vCount];

        int triIndex = 0;

        for (int z = 0; z < vertexPerLine; z++)
        {
            for (int x = 0; x < vertexPerLine; x++)
            {
                int i = z * vertexPerLine + x;

                // world coordinates (untuk noise)
                float worldX = chunkCoord.x * chunkSize + x;
                float worldZ = chunkCoord.y * chunkSize + z;

                // continent (low freq)
                float continent = Mathf.PerlinNoise(
                    (worldX + seed) / 600f,
                    (worldZ + seed) / 600f
                );

                // detail (FBM)
                float detail = FBM(worldX, worldZ, seed, settings.noiseScale);

                float height01 = Mathf.Lerp(continent, detail, 0.45f);
                height01 = Mathf.Pow(height01, 2.0f);

                // mountain mask langka
                float mountainMask = Mathf.PerlinNoise(
                    (worldX + seed + 9999) / 900f,
                    (worldZ + seed + 9999) / 900f
                );
                mountainMask = Mathf.Pow(mountainMask, 4.0f);

                // flatten plains
                // float plainMask = Mathf.InverseLerp(grassMin, grassMax, height01);
                // plainMask = Mathf.SmoothStep(0f, 1f, plainMask);
                // float flattenedHeight01 = Mathf.Lerp(grassFlatLevel, height01, plainMask);
                // flattenedHeight01 += mountainMask * mountainStrength;
                // float flattenedHeight01 = height01 + mountainMask * mountainStrength;

                float flattenedHeight01;
                if (height01 < grassMin)
                    flattenedHeight01 = height01;
                else
                {
                    float plainMask = Mathf.InverseLerp(grassMin, grassMax, height01);
                    plainMask = Mathf.SmoothStep(0f, 1f, plainMask);
                    flattenedHeight01 = Mathf.Lerp(grassFlatLevel, height01, plainMask);
                    flattenedHeight01 += mountainMask * mountainStrength;
                }


                // final height in world space units
                float height = baseHeight + flattenedHeight01 * maxHeight;

                // SIMPAN KE heightMap lokal
                heightMap[x, z] = height;

                // stats debug logging chunks
                float h = heightMap[x, z];
                minH = Mathf.Min(minH, h);
                maxH = Mathf.Max(maxH, h);
                avgH += h;

                // vertices in local chunk coordinates (chunk transform places it in world)
                vertices[i] = new Vector3(x, height, z);

                // WARNA: gunakan height final -> normalisasi ke 0..1 berdasarkan baseHeight..baseHeight+maxHeight
                // float color01 = Mathf.InverseLerp(baseHeight, baseHeight + maxHeight, height);
                // colors[i] = terrainGradient.Evaluate(color01);

                // alternatif: warna berdasarkan level ketinggian tetap
                // float h = height;
                if (height < settings.waterLevelWorld)
                    colors[i] = Color.blue;
                else if (height < settings.sandLevelWorld)
                    colors[i] = new Color(0.9f, 0.85f, 0.5f);
                else if (height < settings.grassLevelWorld)
                    colors[i] = Color.green;
                else if (height < settings.hillLevelWorld)
                    colors[i] = new Color(0.3f, 0.5f, 0.2f);
                else
                    colors[i] = Color.gray;


                // TRIANGLES
                if (x < vertexPerLine - 1 && z < vertexPerLine - 1)
                {
                    int a = i;
                    int b = i + vertexPerLine;
                    int c = i + vertexPerLine + 1;
                    int d = i + 1;

                    triangles[triIndex++] = a;
                    triangles[triIndex++] = b;
                    triangles[triIndex++] = c;

                    triangles[triIndex++] = a;
                    triangles[triIndex++] = c;
                    triangles[triIndex++] = d;
                }
            }
        }

        
        avgH /= (vertexPerLine * vertexPerLine);

        Debug.Log($"[CHUNK {chunkCoord}] Min:{minH:F1} Max:{maxH:F1} Avg:{avgH:F1}");

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // jika chunk besar
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }


    public static Mesh GenerateTerrainMeshV2(ChunkSettings settings, Vector2Int chunkCoord, float[,] heightMap) 
    {
        int chunkSize = settings.chunkSize;
        float scale = settings.noiseScale;
        // float height = settings.heightMultiplier;
        int seed = settings.seed;
        int vertexPerLine = settings.vertexPerLine;
        float baseHeight = settings.baseHeight;
        float maxHeight = settings.maxHeight;

        float grassMin = settings.grassMin;
        float grassMax = settings.grassMax;
        float grassFlatLevel = settings.grassFlatLevel;
        float mountainStrength = settings.mountainStrength;

        Gradient terrainGradient = settings.terrainGradient;

        int vCount = vertexPerLine * vertexPerLine;

        Vector3[] vertices = new Vector3[vCount];
        int[] triangles = new int[(vertexPerLine - 1) * (vertexPerLine - 1) * 6];
        Color[] colors = new Color[vCount];

        int triIndex = 0;

        for(int z = 0; z < vertexPerLine; z++){
            for(int x = 0; x < vertexPerLine; x++){

                int i = z * vertexPerLine + x;

                float worldX = chunkCoord.x * chunkSize + x;
                float worldZ = chunkCoord.y * chunkSize + z;

                float continent = Mathf.PerlinNoise(
                    (worldX + seed) / 600f,
                    (worldZ + seed) / 600f
                );

                // ✅ HEIGHT
                float detail = FBM(worldX, worldZ, seed, settings.noiseScale);

                float height01 = Mathf.Lerp(continent, detail, 0.45f);

                height01 = Mathf.Pow(height01, 2.0f);

                // pegunungan kemunculan dalam dunia
                float mountainMask = Mathf.PerlinNoise(
                    (worldX + seed + 9999) / 900f,
                    (worldZ + seed + 9999) / 900f
                );

                mountainMask = Mathf.Pow(mountainMask, 4.0f); // bikin langka


                // mask 0 = datar penuh, 1 = full noise
                float plainMask = Mathf.InverseLerp(grassMin, grassMax, height01);
                plainMask = Mathf.SmoothStep(0f, 1f, plainMask);

                // redam noise di grass
                float flattenedHeight01 = Mathf.Lerp(grassFlatLevel, height01, plainMask);

                flattenedHeight01 += mountainMask * mountainStrength;

                // normalisasi rentang efektif
                // height01 = Mathf.InverseLerp(0.15f, 0.9f, height01);

                // tinggi dunia sesungguhnya
                // float height = baseHeight + height01 * maxHeight;
                float height = baseHeight + flattenedHeight01 * maxHeight;

                // float height = height01 * maxHeight;

                // float height = CalculateHeight(worldX, worldZ);
                heightMap[x, z] = height;

                vertices[i] = new Vector3(x, height, z);

                // ✅ WARNA GRADIEN
                // colors[i] = terrainGradient.Evaluate(height01); // error !!! miss terrain color
                float color01 = Mathf.InverseLerp(
                    baseHeight,
                    baseHeight + maxHeight,
                    height
                );

                colors[i] = terrainGradient.Evaluate(color01);


                // ✅ TRIANGLE
                if (x < vertexPerLine - 1 && z < vertexPerLine - 1)
                {
                    int a = i;
                    int b = i + vertexPerLine;
                    int c = i + vertexPerLine + 1;
                    int d = i + 1;

                    triangles[triIndex++] = a;
                    triangles[triIndex++] = b;
                    triangles[triIndex++] = c;

                    triangles[triIndex++] = a;
                    triangles[triIndex++] = c;
                    triangles[triIndex++] = d;
                }
            }
        }

        Mesh mesh = new Mesh();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors    = colors;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

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
