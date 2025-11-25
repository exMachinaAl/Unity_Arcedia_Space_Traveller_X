using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game_PlanetFullInformation : MonoBehaviour
{
    [System.Serializable]
    public class MineralResult
    {
        public string id;
        public float amount;
    }

    public Transform planetTransform;
    private Transform PlanetVisual;
    // public float planetRadius = 200f;
    public float atmosphereExtra = 350f;
    public float enterAdderSurface = 70f;
    public PlanetInformation planetInfo;
    // public List<RNG_PlanetData.MineralExData> mineralResources = new List<RNG_PlanetData.MineralExData>();
    public List<MineralResult> mineralResources = new List<MineralResult>();
    private List<MineralExData> planetDataConfig;
    public Transform AtmosphereTrigger;
    public Transform SurfaceTrigger;

    public
    void Start()
    {
        // planetDataConfig = Root_GameStartManager.Instance.planetDataConfigGen.mineralResources;
        // InitRandomResourceData();
        planetTransform = transform;
    }

    public void Init(PlanetInformation oltah)
    {
        // PlanetVisual = transform.Find("MeshPlanet");
        // // transform.find("MeshPlanet").localScale = Vector3.one * (oltah.radius * 2);

        // PlanetVisual.localScale = Vector3.one * (oltah.radius);
        // float planetScale = PlanetVisual.localScale.x; // diasumsikan uniform
        // float realPlanetSize = oltah.radius * planetScale;
        // // float realPlanetSize = planetRadius * planetScale;

        // AtmosphereTrigger.GetComponent<SphereCollider>().radius = realPlanetSize + atmosphereExtra;
        // SurfaceTrigger.GetComponent<SphereCollider>().radius = realPlanetSize;

        PlanetVisual = transform.Find("MeshPlanet");

        // Planet visual scale = diameter
        PlanetVisual.localScale = Vector3.one * (oltah.radius * 2f);

        // Karena scale sudah benar, collider radius = radius
        AtmosphereTrigger.GetComponent<SphereCollider>().radius = oltah.radius + atmosphereExtra + enterAdderSurface;
        SurfaceTrigger.GetComponent<SphereCollider>().radius = oltah.radius + enterAdderSurface;


        planetInfo = oltah;
        planetDataConfig = Root_GameStartManager.Instance.planetDataConfigGen.mineralResources;
        // Debug.Log(planetDataConfig[0].ToString());
        InitRandomResourceData();

        gameObject.GetComponentInChildren<Game_PlanetEnteringAtmosphere>().InitLDZ(this);
    }

    void InitRandomResourceData()
    {

        if (planetInfo == null || planetInfo.planetSeed == 0)
        {
            Debug.LogError("Error null tidak init tepat waktu.");
            Debug.LogError("Planet seed is zero! Cannot generate minerals.");
            return;

        }

        Unity.Mathematics.Random rng = new Unity.Mathematics.Random((uint)planetInfo.planetSeed);

        List<MineralResult> results = new List<MineralResult>();
        List<float> tempAmounts = new List<float>();

        // Step A — Tentukan mineral muncul atau tidak
        foreach (var cfg in planetDataConfig)
        {
            if (rng.NextFloat(0f, 1f) <= cfg.spawnChance)
            {
                // Step B — Hitung jumlah awal (tanpa normalisasi)
                // float variation = Random.Range(
                //     1f - cfg.variability,
                //     1f + cfg.variability
                // );
                float variation = rng.NextFloat(
                    1f - cfg.variability,
                    1f + cfg.variability
                );

                float rawAmount = cfg.richness * variation;

                results.Add(new MineralResult { id = cfg.mineralId });
                tempAmounts.Add(rawAmount);
            }
            else
            {
                // Tidak muncul → skip
            }
        }

        // Kalau tidak ada mineral sama sekali (kasus langka)
        if (results.Count == 0)
        {
            Debug.Log("vry unlucky planet, no minerals spawned.");
            mineralResources.Clear();
            return;
        }

        // Step C — Normalisasi ke totalMineral
        float sum = 0f;
        foreach (float v in tempAmounts) sum += v;

        float scale = planetInfo.mineralMaxRichness / sum;

        for (int i = 0; i < results.Count; i++)
        {
            results[i].amount = tempAmounts[i] * scale;
        }

        for (int i = 0; i < results.Count; i++)
        {
            mineralResources.Add(results[i]);
        }
        Debug.Log("all good and set it in mineral");
    }
}
