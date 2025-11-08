using UnityEngine;

public class BulletControllerV1 : MonoBehaviour
{
    public float speed = 100f;
    public float lifeTime = 3f;

    private Vector3 moveDirection;

    // Method untuk dipanggil dari luar
    public void Fire(Vector3 direction)
    {
        moveDirection = direction.normalized;
        transform.forward = moveDirection; // pastikan peluru menghadap arah
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Gerakkan peluru manual setiap frame
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    // Opsional: Deteksi tabrakan
    private void OnCollisionEnter(Collision collision)
    {
        // Bisa ditambah efek atau damage di sini
        Destroy(gameObject);
    }
}
