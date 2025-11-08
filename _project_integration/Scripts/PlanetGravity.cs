using UnityEngine;

public class PlanetGravity : MonoBehaviour
{
    public float gravity = 9.81f;
    public Transform planetCenter;

    void FixedUpdate()
    {
        foreach (Rigidbody rb in FindObjectsOfType<Rigidbody>())
        {
            Vector3 dir = (planetCenter.position - rb.position).normalized;
            rb.AddForce(dir * gravity, ForceMode.Acceleration);
        }
    }
}
