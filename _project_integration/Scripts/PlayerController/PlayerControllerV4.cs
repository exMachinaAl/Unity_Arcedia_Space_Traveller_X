using UnityEngine;

public class PlayerControllerV4 : MonoBehaviour
{
    public IPlayerInput input;
    public float walkSpeed = 4;
    public float runSpeed = 6;
    [SerializeField] private float currentSpeed = 0.1f;
    public Transform cameraPivot;
    private Transform cam;

    private CharacterController cc;
    private Rigidbody rb;
    private Animator animatorCtrl;
    public float gravity = -9.81f;

    public float interactRange = 6f;
    public LayerMask interactLayerMask;

    // public bool isControlled = true; // debug untuk animasi
    public bool isControlled = false;

    Vector3 velocity;
    float velocityMagnitude;
    Vector3 lastPos;

    void Start()
    {

        animatorCtrl = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CharacterController>();
        cam = cameraPivot.GetComponentInChildren<Camera>().transform;
        lastPos = transform.position;
        // Manager_Player.Instance.RegisterPlayer(gameObject);

        isControlled = true; // debug untuk animasi
    }

    void Update()
    {
        input ??= Manager_Controller.Instance.CInput;
        PlayerGravity();

        if (!isControlled) return;
        // GetComponentInChildren<CameraFollowV4>().

        AnimCtrl();
        ControlHandling();
        OnTriggerStay();
    }

    public void AnimCtrl()
    {
        if (input.Interact)
        {
            animatorCtrl.SetTrigger("pickingSmall");
        }

        if (input.Jump)
            animatorCtrl.SetTrigger("jump");

        currentSpeed = walkSpeed;
        if (input.SprintToggle)
        {
            currentSpeed = runSpeed;
        }
        // float speed = new Vector2(rb.velocity.x, rb.velocity.z).magnitude;
        // Vector3 displacement = transform.position - lastPos;
        // velocityMagnitude = displacement.magnitude / Time.deltaTime;
        // lastPos = transform.position;
        float velocityOfS = input.MoveAxis.magnitude * currentSpeed;
        Debug.Log($"speed : {velocityOfS}");
        // Debug.Log($"speed: {speed}");
        animatorCtrl.SetFloat("speed", velocityOfS);

        bool grounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
        animatorCtrl.SetBool("isGrounded", grounded);
    }

    public void EnableControl()
    {
        isControlled = true;
    }

    public void DisableControl()
    {
        isControlled = false;
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

        cc.Move(moveDir * currentSpeed * Time.deltaTime);
    }

    public void PlayerGravity()
    {
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
    public void OnTriggerEnter(Collider other)
    {
        Manager_Player.Instance.MOnTriggerEnter(other);
    }

    public void OnTriggerExit(Collider other)
    {
        Manager_Player.Instance.MOnTriggerExit(other);
    }

}
