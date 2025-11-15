using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MineralData
{
    public int seed;
    public Vector3 position;
    public string type;
    public float richness;
}

[System.Serializable]
public class PlanetData
{
    public int seed;
    public string name;
    public int mineralCount;
    public Vector3 position;
    public List<MineralData> minerals;
	

	public MineralData GenerateMineral(int seed)
	{
		Random.InitState(seed);
		string[] types = { "Iron", "Gold", "Uranium", "Crystal", "Helium3" };

		return new MineralData
		{
			seed = seed,
			type = types[Random.Range(0, types.Length)],
			position = new Vector3(
				Random.Range(-5000f, 5000f),
				Random.Range(-5000f, 5000f),
				Random.Range(-5000f, 5000f)
			),
			richness = Random.Range(0.5f, 5f)
		};
	}
	
	
	public PlanetData GeneratePlanet(int planetSeed, Vector3 pos)
	{
		Random.InitState(planetSeed);

		PlanetData planet = new PlanetData();
		planet.seed = planetSeed;
		planet.name = "Planet_" + planetSeed;
		planet.position = pos;

		// Tentukan jumlah mineral total planet (misal 1 juta)
		planet.mineralCount = Random.Range(800000, 1200000);
		planet.minerals = new List<MineralData>();

		// Bikin beberapa contoh mineral untuk awal preview (bukan semua!)
		for (int i = 0; i < 10; i++)
		{
			int mSeed = SeedUtil.SubSeed(planetSeed, i);
			planet.minerals.Add(GenerateMineral(mSeed));
		}

		return planet;
	}

}