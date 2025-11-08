using UnityEngine;

public class PlanetSpawner : MonoBehaviour
{
    public GameObject[] planetPrefabs;   // daftar prefab planet
    public int planetCount = 10;         // berapa planet yang akan di-spawn
    public float minDistance = 100f;     // jarak minimum dari player
    public float maxDistance = 500f;     // jarak maksimum dari player
    public Transform player;             // referensi posisi player

    void Start()
    {
        SpawnPlanets();
    }

    void SpawnPlanets()
    {
        for (int i = 0; i < planetCount; i++)
        {
            Vector3 randomDir = Random.onUnitSphere; // arah acak 3D
            float distance = Random.Range(minDistance, maxDistance);
            Vector3 spawnPos = player.position + randomDir * distance;

            int randomIndex = Random.Range(0, planetPrefabs.Length);
            GameObject planet = Instantiate(planetPrefabs[randomIndex], spawnPos, Quaternion.identity);

            planet.transform.localScale *= Random.Range(0.5f, 2.5f); // variasi ukuran
        }
    }
}
