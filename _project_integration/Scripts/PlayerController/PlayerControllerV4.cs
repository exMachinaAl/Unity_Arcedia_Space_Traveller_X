using UnityEngine;

public class PlayerControllerV4 : MonoBehaviour
{
    public float speed = 6f;
    public Transform cameraPivot;
    private Transform cam;

    private CharacterController cc;
    public float gravity = -9.81f;
    
    public float interactRange = 6f;
    public LayerMask interactLayerMask;

    Vector3 velocity;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        cam = cameraPivot.GetComponentInChildren<Camera>().transform;
        // Manager_Player.Instance.RegisterPlayer(gameObject);
    }

    void Update()
    {
        ControlHandling();
        OnTriggerStay();
    }

    private void ControlHandling()
    {

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 moveDir = Vector3.zero;

        if (cameraPivot)
        {
            Vector3 forward = cameraPivot.forward;
            Vector3 right = cameraPivot.right;

            forward.y = 0;
            right.y = 0;

            forward.Normalize();
            right.Normalize();

            moveDir = forward * v + right * h;
        }

        if (moveDir.magnitude > 0.1f)
        {
            // player menghadap arah gerak (tapi hanya yaw)
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * 10f);
        }

        cc.Move(moveDir * speed * Time.deltaTime);

        velocity.y = 0f;
        
        if (Manager_Player.Instance.InWorld == PlayerInThe.Ground)
        {
            velocity.y += gravity * Time.deltaTime;
        }

        cc.Move(velocity * Time.deltaTime);
    }

    private void OnTriggerStay()
    {
        Ray ray = new Ray(cam.position, cam.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactLayerMask))
        {
            FlightControllerV1 ship = hit.collider.GetComponentInParent<FlightControllerV1>();

            Debug.Log("hit something smotththh");
            if (ship != null)
            {
                Debug.Log("Press F to enter the ship");
                if (Input.GetKeyDown(KeyCode.F))
                {
                    // Interact with ship
                    Manager_Player.Instance.EnterShip(ship);
                }
            }
        }
    }
}
