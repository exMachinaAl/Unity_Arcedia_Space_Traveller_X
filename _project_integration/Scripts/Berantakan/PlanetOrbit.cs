using UnityEngine;

public class PlanetOrbit : MonoBehaviour
{
    [Header("Orbit Settings")]
    public Transform orbitCenter; // misal Sun
    public float orbitSpeed = 10f; // derajat per detik
    public float orbitRadius = 10f; // jarak ke matahari

    [Header("Rotation Settings")]
    public float selfRotationSpeed = 30f; // rotasi di sumbu sendiri

    [Header("Debug Orbit Path")]
    public bool showOrbitGizmo = true;
    public Color orbitColor = Color.white;

    private float currentAngle;

    void Update()
    {
        if (orbitCenter != null)
        {
            // Hitung posisi orbit planet
            currentAngle += orbitSpeed * Time.deltaTime;
            float rad = currentAngle * Mathf.Deg2Rad;

            // Vector3 offset = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * orbitRadius;
			
			Vector3 offset = new Vector3(Mathf.Cos(rad) * orbitRadius, 0, Mathf.Sin(rad) * orbitRadius * 0.8f);

            transform.position = orbitCenter.position + offset;
        }

        // Rotasi di sumbu sendiri
        transform.Rotate(Vector3.up, selfRotationSpeed * Time.deltaTime, Space.Self);
    }

    void OnDrawGizmos()
    {
        if (showOrbitGizmo && orbitCenter)
        {
            Gizmos.color = orbitColor;
            Gizmos.DrawWireSphere(orbitCenter.position, orbitRadius);
        }
    }
}
