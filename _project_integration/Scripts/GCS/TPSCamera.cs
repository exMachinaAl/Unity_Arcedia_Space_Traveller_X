using UnityEngine;

public class TPSCamera : MonoBehaviour
{
    public Transform target;     // player
    public float distance = 5.0f;
    public float sensitivity = 5.0f;

    float yaw = 0f;
    float pitch = 0f;

    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        pitch = Mathf.Clamp(pitch, -30f, 60f); // batasi rotasi atas/bawah

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 position = target.position - rotation * Vector3.forward * distance;

        transform.position = position;
        transform.LookAt(target);

        if (Input.GetMouseButtonDown(0)) // klik kiri
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log("Kena: " + hit.collider.name);
            }
        }
    }
}
