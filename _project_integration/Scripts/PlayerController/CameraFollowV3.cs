using UnityEngine;

public class CameraFollowV3 : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;
    public Transform cam;
    public Vector3 offset = new Vector3(0, 2, -8);

    [Header("Camera Movement")]
    public float followSpeed = 8f;
    public float mouseSensitivity = 3f;
    public float rotationSmoothness = 5f;

    [Header("Zoom Settings")]
    public float zoomSpeed = 5f;
    public float minZoom = -20f;
    public float maxZoom = -4f;

    [Header("Rotation Limits")]
    public float maxYaw = 60f;   // batas kiri-kanan
    public float maxPitch = 40f; // batas atas-bawah

    [Header("Auto Center")]
    public float idleTimeBeforeReset = 2f;
    public float autoCenterSpeed = 2f;

    private float yaw;
    private float pitch;
    private float idleTimer;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
    }

    void LateUpdate()
    {
        if (!target) return;

        // --- ZOOM ---
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        offset.z = Mathf.Clamp(offset.z + scroll * zoomSpeed, minZoom, maxZoom);

        // --- ROTASI DENGAN BATAS ---
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        if (Mathf.Abs(mouseX) > 0.01f || Mathf.Abs(mouseY) > 0.01f)
        {
            yaw += mouseX * mouseSensitivity;
            pitch -= mouseY * mouseSensitivity;

            yaw = Mathf.Clamp(yaw, -maxYaw, maxYaw);
            pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);

            idleTimer = 0f; // reset idle
        }
        else
        {
            idleTimer += Time.deltaTime;

            // --- AUTO CENTER ---
            if (idleTimer > idleTimeBeforeReset)
            {
                yaw = Mathf.Lerp(yaw, 0f, Time.deltaTime * autoCenterSpeed);
                pitch = Mathf.Lerp(pitch, 0f, Time.deltaTime * autoCenterSpeed);
            }
        }

        // rotasi kamera berdasarkan offset rotasi lokal
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredPosition = target.position + rotation * offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        transform.LookAt(target.position);
    }
}
