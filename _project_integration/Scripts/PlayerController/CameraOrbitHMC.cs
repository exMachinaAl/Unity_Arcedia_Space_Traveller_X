using UnityEngine;

public class CameraOrbitHMC : MonoBehaviour
{
    [Header("Target")]
    public Transform target; // player

    [Header("Orbit Settings")]
    public float mouseSensitivity = 25f;
    public Vector2 pitchLimits = new Vector2(-20f, 70f);

    [Header("Distance & Zoom")]
    public float distance = 6f;
    public float zoomSpeed = 2f;
    public float minDistance = 2f;
    public float maxDistance = 10f;

    [Header("Offset Settings")]
    public float verticalOffset = 0.2f; // posisi tengah kamera relatif terhadap player
    public float zoomVerticalCompensate = 0.003f; // seberapa tinggi kamera naik saat zoom out

    [Header("Smoothness")]
    public float followSmoothHorizontal = 30f;
    public float followSmoothVertical = 20f; // lebih lambat untuk redam jitter

    private float yaw;
    private float pitch;

    private Vector3 currentTargetPos;

    void Start()
    {
        if (target == null)
        {
			//custom code for multi scene and regis
			if (SceneRegistry.I.player) {
				target = SceneRegistry.I.player;
			} else {
				Debug.LogWarning("Camera target not assigned!");
				return;	
			}
        }

        // Mulai dengan posisi target sekarang
        currentTargetPos = target.position;
    }

    void LateUpdate()
    {
        if (target == null) return;

        HandleRotation();
        HandleZoom();
        HandleFollow();
    }

    void HandleRotation()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, pitchLimits.x, pitchLimits.y);
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance -= scroll * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
    }

    void HandleFollow()
    {
        // Hitung posisi target ideal kamera
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        float dynamicVertical = verticalOffset + (distance - minDistance) * zoomVerticalCompensate;

        Vector3 desiredTargetPos = target.position + Vector3.up * dynamicVertical;

        // Smooth horizontal dan vertical berbeda
        currentTargetPos.x = Mathf.Lerp(currentTargetPos.x, desiredTargetPos.x, Time.deltaTime * followSmoothHorizontal);
        currentTargetPos.z = Mathf.Lerp(currentTargetPos.z, desiredTargetPos.z, Time.deltaTime * followSmoothHorizontal);
        currentTargetPos.y = Mathf.Lerp(currentTargetPos.y, desiredTargetPos.y, Time.deltaTime * followSmoothVertical);

        // Tentukan posisi kamera di belakang target
        Vector3 desiredCamPos = currentTargetPos - rotation * Vector3.forward * distance;

        transform.position = desiredCamPos;
        transform.LookAt(currentTargetPos);
    }
}
