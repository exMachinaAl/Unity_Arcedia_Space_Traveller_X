using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class UniverseGenerator : MonoBehaviour
{
    public int universeSeed = 123456; // seed utama
    public int planetCount = 10;
	public PlanetData data; // reference ke data planet

    [System.Serializable]
    public class PlanetData
    {
        public string planetName;
        public int planetSeed;
        public Vector3 position;
        public float size;
        public float mineralDensity;
    }

    public List<PlanetData> planets = new List<PlanetData>();
	
	
	

    // dipanggil setelah dibuat oleh generator
    public void Init(PlanetData planetData)
    {
        data = planetData;
    }
	
	private void OnMouseDown()
    {
        Debug.Log($"Planet clicked: {data.planetName} | Seed: {data.planetSeed} | Mineral: {data.mineralDensity}");
        // nanti di sini kamu bisa buka UI untuk enter planet / interact
    }

    void Start()
    {
        GenerateUniverse(universeSeed);
    }

    public void GenerateUniverse(int seed)
    {
        Random.InitState(seed);
        planets.Clear();

        for (int i = 0; i < planetCount; i++)
        {
            // chaining seed dari universe
            int planetSeed = Random.Range(int.MinValue, int.MaxValue);

            PlanetData planet = new PlanetData
            {
                planetName = "Planet_" + i,
                planetSeed = planetSeed,
                position = Random.onUnitSphere * Random.Range(100f, 1500f),
                size = Random.Range(50f, 500f),
                mineralDensity = Random.Range(0.1f, 1f)
            };

            planets.Add(planet);
			SpawnPlanetObject(planet);
			//Debug.Log("index " + i + ": " + planet.position);
            // bisa langsung generate isi planet di sini
            GeneratePlanetSurface(planet);
        }
    }

    void GeneratePlanetSurface(PlanetData planet)
    {
        Random.InitState(planet.planetSeed);

        int mineralClusterCount = Mathf.RoundToInt(planet.mineralDensity * 100);
        for (int i = 0; i < mineralClusterCount; i++)
        {
            Vector3 localPos = new Vector3(
                Random.Range(-planet.size, planet.size),
                Random.Range(-planet.size, planet.size),
                Random.Range(-planet.size, planet.size)
            );

            float richness = Random.Range(10f, 1000f);

            // nanti ini bisa dipetakan jadi prefab mineral
            //Debug.Log($"Mineral at {localPos}, Richness: {richness}");
        }
    }
	
	void SpawnPlanetObject(PlanetData data)
	{
		GameObject planet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		planet.transform.position = data.position;
		planet.transform.localScale = Vector3.one * 200f;
		planet.name = data.planetName;

		// Warna planet berdasarkan seed (biar unik)
		Random.InitState(data.planetSeed);
		Color c = new Color(Random.value, Random.value, Random.value);
		planet.GetComponent<Renderer>().material.color = c;

		// Tambahkan komponen PlanetBehaviour
		//planet.AddComponent<PlanetBehaviour>().Init(data);
	}

}
