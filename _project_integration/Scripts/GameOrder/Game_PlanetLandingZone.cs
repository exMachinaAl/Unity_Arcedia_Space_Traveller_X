using UnityEngine;

public class Game_PlanetLandingZone : MonoBehaviour
{
    private Game_PlanetFullInformation planetInfoComponent;
    // public float surfaceTriggerRadius = 0.65f;

    void Start()
    {
        SphereCollider atmosphereCollider = GetComponent<SphereCollider>();
        atmosphereCollider.isTrigger = true;
        // atmosphereCollider.radius = surfaceTriggerRadius;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Vector3 playerPos = other.transform.position;


        // Debug.Log("Player memasuki planet " + planetInfoComponent.planetInfo.planetName);
        Manager_Landing.Instance.EnteringPlanetSurfaceActiveFlags(transform.parent);
    }

    // private void OnTriggerExit(Collider other)
    // {
    //     if (!other.CompareTag("Player")) return;

    //     Debug.Log("Player meninggalkan atmosfer planet " + planetInfoComponent.planetInfo.planetName);

    //     Manager_Landing.Instance.ExitPlanetAtmosphere(planetInfoComponent);
    // }
}
