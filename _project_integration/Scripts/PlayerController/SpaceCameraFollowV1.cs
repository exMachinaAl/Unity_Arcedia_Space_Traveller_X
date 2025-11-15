using UnityEngine;

public class SpaceCameraFollowV1 : MonoBehaviour
{
    public Transform target;
    public float followSpeed = 5f;

    [Header("Orbit Settings")]
    public float distance = 8f;
    public float mouseSensitivity = 2f;

    private float yaw;
    private float pitch;

    void LateUpdate()
    {
        if (!target) return;

        // Ambil input mouse
        float mx = Input.GetAxis("Mouse X") * mouseSensitivity;
        float my = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mx;
        pitch -= my;
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        // Kamera orbit menggunakan transform rotation target
        Quaternion rotOffset = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = rotOffset * (Vector3.back * distance);

        Vector3 desiredPosition = target.position + target.rotation * offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * followSpeed);

        transform.rotation = Quaternion.LookRotation(
            target.position - transform.position,
            target.up
        );
    }
}
