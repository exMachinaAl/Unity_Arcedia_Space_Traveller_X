using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Game_WorldGenerator : MonoBehaviour
{
    public int seedWorld = -1; // seed utama
    public int worldObject = 10;
    public GameObject resourcePrefab;
    public Transform rootGObject;
	public ResourceData data; // reference ke data planet

    [System.Serializable]
    public class ResourceData
    {
        public string resourceName;
        public long resourceId;
        public Vector3 position;
        public float size;
        public float mineralDensity;
    }

    public List<ResourceData> resources = new List<ResourceData>();

    // dipanggil setelah dibuat oleh generator
    public void Init(ResourceData ResourceData)
    {
        data = ResourceData;
    }

    void Start()
    {
        int galaxySeed = SeedUtil.SubSeed(Game_SeedManager.Instance.universeSeed, 0);
        seedWorld = SeedUtil.SubSeed(galaxySeed, 0);
        GenerateWorld(seedWorld);
    }

    public void GenerateWorld(int resId)
    {
        Random.InitState(resId);
        resources.Clear();

        for (int i = 0; i < worldObject; i++)
        {
            // chaining resId dari universe
            //int resourceId = Random.Range(int.MinValue, int.MaxValue);
            // int resourceId = SeedUtil.SubSeed(resId, i);
            float x = Random.Range(-38f, 36f);
            float z = Random.Range(-42f, 32f);

            Vector2Int gridPos = new Vector2Int(
                Mathf.RoundToInt(x),
                Mathf.RoundToInt(z)
            );
            Vector3 posRes = new Vector3(
                    x,
                    -7f,
                    z
            );

            long resourceId = SeedUtil.makeResourcesId(resId, gridPos, i);

            ResourceData resource = new ResourceData
            {
                resourceName = "weit_" + i,
                resourceId = resourceId,
                position = posRes,
                size = Random.Range(50f, 500f),
                mineralDensity = Random.Range(0.1f, 1f)
            };

            if (Game_SaveSystem.Instance.IsNodeDepleted(resId, resourceId))
            {
                // Node sudah pernah dihancurkan → jangan spawn
                continue;
            }

            resources.Add(resource);
            spawnResourceObject(resource);
			// SpawnPlanetObject(planet);
			//Debug.Log("index " + i + ": " + planet.position);
            // bisa langsung generate isi planet di sini
            // GeneratePlanetSurface(planet);
        }
    }

    // void GeneratePlanetSurface(ResourceData planet)
    // {
    //     Random.InitState(planet.resourceId);

    //     int mineralClusterCount = Mathf.RoundToInt(planet.mineralDensity * 100);
    //     for (int i = 0; i < mineralClusterCount; i++)
    //     {
    //         Vector3 localPos = new Vector3(
    //             Random.Range(-planet.size, planet.size),
    //             Random.Range(-planet.size, planet.size),
    //             Random.Range(-planet.size, planet.size)
    //         );

    //         float richness = Random.Range(10f, 1000f);

    //         // nanti ini bisa dipetakan jadi prefab mineral
    //         //Debug.Log($"Mineral at {localPos}, Richness: {richness}");
    //     }
    // }

    void spawnResourceObject(ResourceData data) 
    {
        GameObject resource = Instantiate(resourcePrefab, Vector3.zero, Quaternion.identity, rootGObject);
        resource.transform.position = data.position;
        //resource.transform.localScale = Vector3.one * 20f;
        resource.name = data.resourceName;
        SetLayerRecursively(resource, resource.layer);

        resource.GetComponent<Game_ResourceNode>().Init(data.resourceId);
        

        // Warna planet berdasarkan resId (biar unik)
        // Random.InitState(data.resourceId);
        // Color c = new Color(Random.value, Random.value, Random.value);
        // resource.GetComponent<Renderer>().material.color = c;

        // Tambahkan komponen PlanetBehaviour
        //planet.AddComponent<PlanetBehaviour>().Init(data);
    }

    void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, layer);
    }

	
	// void SpawnPlanetObject(ResourceData data)
	// {
	// 	GameObject planet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
	// 	planet.transform.position = data.position;
	// 	planet.transform.localScale = Vector3.one * 200f;
	// 	planet.name = data.resourceName;

	// 	// Warna planet berdasarkan resId (biar unik)
	// 	Random.InitState(data.resourceId);
	// 	Color c = new Color(Random.value, Random.value, Random.value);
	// 	planet.GetComponent<Renderer>().material.color = c;

	// 	// Tambahkan komponen PlanetBehaviour
	// 	//planet.AddComponent<PlanetBehaviour>().Init(data);
	// }

}
