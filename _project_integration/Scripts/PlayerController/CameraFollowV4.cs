using UnityEngine;

public class CameraFollowV4 : MonoBehaviour
{
    public Transform player;
    public Transform cam;       // camera transform
    [SerializeField] bool pCamControl = true;

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
        // Cursor.lockState = CursorLockMode.Locked; // 11/23/25
        // Cursor.visible = false;


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

        switch (Game_SaveSystem.Instance.GetPlayerMode())
        {
            case PlayerMode.Human:
                {
                    pCamControl = FoundRootPInCamStruct<PlayerControllerV4>(transform).isControlled;
                    break;
            }
            case PlayerMode.Flight:
                {
                    pCamControl = FoundRootPInCamStruct<FlightControllerV1>(transform).isControlled;
                    break;
            }
            default: {
                    Debug.LogWarning($"switch error for Camera follow V4 controller");
                    break;
            }
        }
        // Debug.LogError($"what if pCamControl = {pCamControl}");
        if (!pCamControl) return;

        // Mouse input
        CameraFollowingMouse();
    }

    private T FoundRootPInCamStruct<T>(Transform trn) where T : Component
    {
        Transform rpnt = trailParent(trn);
        return rpnt.GetComponent<T>();
    }
    private Transform trailParent(Transform p)
    {
        Transform ParentI = p.transform;
        while (ParentI.parent != null)
        {
            ParentI = ParentI.parent;
        }
        return ParentI;
    }

    public void CameraFollowingMouse()
    {
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
