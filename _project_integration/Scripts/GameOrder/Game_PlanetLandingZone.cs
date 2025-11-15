using UnityEngine;

public class Game_PlanetLandingZone : MonoBehaviour
{
    // public int planetID;
    // public int surfaceSeed;
    public float planetRadius = 200f;
    // public float atmosphereThickness = 300f;

    private SphereCollider atmosphereCollider;
    private Game_PlanetFullInformation planetInfoComponent;


    // public InitLDZ(float planetRadius, float atmosphereThickness)
    public void InitLDZ(Game_PlanetFullInformation planetInfoComponent)
    {
        atmosphereCollider = GetComponent<SphereCollider>();
        atmosphereCollider.isTrigger = true;

        atmosphereCollider.radius = 0.75f;
        // Set otomatis radius collider
        // atmosphereCollider.radius = planetInfoComponent.planetInfo.atmosphereThickness; // agar kek nentuin maks gravity nya
        // atmosphereCollider.radius = planetInfoComponent.planetInfo.size + planetInfoComponent.planetInfo.atmosphereThickness;
    }
    // private void Awake()
    // {
    //     atmosphereCollider = GetComponent<SphereCollider>();
    //     atmosphereCollider.isTrigger = true;

    //     // Set otomatis radius collider
    //     atmosphereCollider.radius = planetRadius + atmosphereThickness;
    // }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log("Player memasuki atmosfer planet " + planetInfoComponent.planetInfo.planetName);

        // Kirim sinyal ke manager bahwa planet ini siap landing
        Manager_Landing.Instance.EnterPlanetAtmosphere(planetInfoComponent);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log("Player meninggalkan atmosfer planet " + planetInfoComponent.planetInfo.planetName);

        Manager_Landing.Instance.ExitPlanetAtmosphere(planetInfoComponent);
    }
}
