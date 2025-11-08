using UnityEngine;

public class CameraFollowV2 : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 2, -10);
    public float followSpeed = 8f;
    public float zoomSpeed = 5f;
    public float minZoom = -20f;
    public float maxZoom = -4f;
    public float rotationSpeed = 100f;

    private float yaw;   // rotasi horizontal
    private float pitch; // rotasi vertikal

    void Start()
    {
        if (target)
        {
            Vector3 angles = transform.eulerAngles;
            yaw = angles.y;
            pitch = angles.x;
        }

        // kunci cursor (opsional)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (!target) return;

        // ============ 1. Zoom ==============
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        offset.z = Mathf.Clamp(offset.z + scroll * zoomSpeed, minZoom, maxZoom);

        // ============ 2. Rotasi kamera (pakai input mouse) ============
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yaw += mouseX * rotationSpeed * Time.deltaTime;
        pitch -= mouseY * rotationSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -40f, 80f); // biar gak kelewat miring

        // ============ 3. Hitung posisi kamera =============
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredPosition = target.position + rotation * offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up * 1.5f); // lihat ke target (tengah badan)
    }
}
