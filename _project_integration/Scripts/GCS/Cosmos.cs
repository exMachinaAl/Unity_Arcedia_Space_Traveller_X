using UnityEngine;

public class Cosmos : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        // Gerak kiri-kanan (X) dan depan-belakang (Z)
        float moveX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * speed * Time.deltaTime;

        // Gerak naik-turun (Y) dengan tombol R (naik) dan F (turun)
        float moveY = 0f;
        if (Input.GetKey(KeyCode.R))
        {
            moveY = speed * Time.deltaTime;  // naik
        }
        else if (Input.GetKey(KeyCode.F))
        {
            moveY = -speed * Time.deltaTime; // turun
        }

        // Terapkan semua gerakan
        transform.Translate(moveX, moveY, moveZ);
    }
}