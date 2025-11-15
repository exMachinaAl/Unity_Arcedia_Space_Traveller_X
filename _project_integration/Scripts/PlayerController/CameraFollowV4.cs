using UnityEngine;

public class CameraFollowV4 : MonoBehaviour
{
    public Transform player;
    public Transform cam;       // camera transform

    public float distance = 6f;
    public float minDistance = 3f;
    public float maxDistance = 12f;

    public float mouseSensitivity = 2f;
    public float pitchMin = -30f;
    public float pitchMax = 75f;

    private float yaw;
    private float pitch;
    // public float interactRange = 6f;
    // public LayerMask interactLayerMask;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        
        // registeringPlayer();
    }
    // void Update()
    // {
    //     if (player == null) return;

    //     OnTriggerStay();
    // }

    // public void registeringPlayer()
    // {
    //     Manager_Player.Instance.RegisterPlayer(gameObject);
    // }   

    void LateUpdate()
    {
        if (!player) return;

        // Mouse input
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        // Apply rotation to the pivot (this object)
        transform.rotation = Quaternion.Euler(pitch, yaw, 0);

        // Handle zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance = Mathf.Clamp(distance - scroll * 5f, minDistance, maxDistance);

        // Set camera position behind the pivot
        cam.localPosition = new Vector3(0, 0, -distance);
    }
}
