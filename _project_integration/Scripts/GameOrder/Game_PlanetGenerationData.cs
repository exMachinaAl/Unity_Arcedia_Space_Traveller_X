using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class PlanetInformation
{
    public string planetName;
    public long planetSeed;
    public Vector3 position;
    public float size;
    public float atmosphereThickness;
    public float mineralDensity;
    public float mineralMaxRichness;
}

public class Game_PlanetGenerationData : MonoBehaviour
{
    //public int universeSeed = 123456; // seed utama
    public Transform ParentPlanetGenerated;
    public int planetCount = 10;
    public GameObject planetPrefab;
    public static float PLANET_RADIUS = 1500f; // must be init in scriptObj
	// public PlanetInformation data; // reference ke data planet


    public List<PlanetInformation> planets = new List<PlanetInformation>();
	
	
	

    // dipanggil setelah dibuat oleh generator
    // public void Init(PlanetInformation PlanetInformation)
    // {
    //     data = PlanetInformation;
    // }
	
	private void OnMouseDown()
    {
        //Debug.Log($"Planet clicked: {data.planetName} | Seed: {data.planetSeed} | Mineral: {data.mineralDensity}");
        // nanti di sini kamu bisa buka UI untuk enter planet / interact
    }

    void Start()
    {
        GeneratePlanetData(Game_SeedManager.Instance.currentGalaxySeed);
    }

    public void GeneratePlanetData(long seed)
    {
        // Random.InitState(seed);
        
        Unity.Mathematics.Random rand = new Unity.Mathematics.Random((uint)seed);
        planets.Clear();

        for (int i = 0; i < planetCount; i++)
        {
            // chaining seed dari universe
            //int planetSeed = Random.Range(int.MinValue, int.MaxValue);
            long planetSeed = SeedUtil.SubSeed(seed, i);

            PlanetInformation planet = new PlanetInformation
            {
                planetName = "Planet_" + i,
                planetSeed = planetSeed,
                position = RandomPointInSphere(rand, PLANET_RADIUS),
                size = rand.NextFloat(50f, 500f),
                atmosphereThickness = rand.NextFloat(0.5f, 1.5f),
                mineralDensity = 0.001f,
                mineralMaxRichness = rand.NextInt(0, 5001) // hanya debug, disarankan mengganti sebagai config
            };

            planets.Add(planet);
			SpawnPlanetObject(planet);
			//Debug.Log("index " + i + ": " + planet.position);
            // bisa langsung generate isi planet di sini
            // GeneratePlanetSurface(planet);
        }
    }

    Vector3 RandomPointInSphere(Unity.Mathematics.Random rand, float radius)
    {
        float u = rand.NextFloat();
        float v = rand.NextFloat();
        float theta = u * 2.0f * Mathf.PI;
        float phi = Mathf.Acos(2.0f * v - 1.0f);
        float r = radius * Mathf.Pow(rand.NextFloat(), 1.0f / 3.0f);

        float sinPhi = Mathf.Sin(phi);
        float x = r * sinPhi * Mathf.Cos(theta);
        float y = r * sinPhi * Mathf.Sin(theta);
        float z = r * Mathf.Cos(phi);

        return new Vector3(x, y, z);
    }

    void GeneratePlanetSurface(PlanetInformation planet)
    {
        // Random.InitState(planet.planetSeed);

        int mineralClusterCount = Mathf.RoundToInt(planet.mineralDensity * 100);
        for (int i = 0; i < mineralClusterCount; i++)
        {
            // Vector3 localPos = new Vector3(
            //     Random.Range(-planet.size, planet.size),
            //     Random.Range(-planet.size, planet.size),
            //     Random.Range(-planet.size, planet.size)
            // );

            // float richness = Random.Range(10f, 1000f);

            // nanti ini bisa dipetakan jadi prefab mineral
            //Debug.Log($"Mineral at {localPos}, Richness: {richness}");
        }
    }

    void SpawnPlanetObject(PlanetInformation data)
    {
        GameObject planet = Instantiate(planetPrefab, Vector3.zero, Quaternion.identity, ParentPlanetGenerated);
        // GameObject planet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        planet.transform.position = data.position;
        // Transform planetTransform = planet.transform.Find("MeshPlanet");
        planet.transform.localScale = Vector3.one * data.size;
        // planet.transform.localScale = Vector3.one * 200f;
        planet.name = data.planetName;
        // planet.transform.SetParent(ParentPlanetGenerated);

        planet.AddComponent<Game_PlanetFullInformation>();
        planet.GetComponent<Game_PlanetFullInformation>().Init(data);
        
        // planet.GetComponentsInChildren<Game_PlanetLandingZone>().InitLDZ(data.size, 300f);
		// Warna planet berdasarkan seed (biar unik)
        // Random.InitState(data.planetSeed);
        // Color c = new Color(Random.value, Random.value, Random.value);
        // planet.GetComponent<Renderer>().material.color = c;

        // Tambahkan komponen PlanetBehaviour
        //planet.AddComponent<PlanetBehaviour>().Init(data);
    }

}
