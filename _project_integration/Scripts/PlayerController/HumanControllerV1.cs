using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HumanControllerV1 : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public Transform cameraTransform;

    [Header("Gravity")]
    public float gravity = -20f;
    public float groundCheckOffset = 0.3f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        // Cek apakah grounded
        isGrounded = controller.isGrounded;

        // Ambil input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = new Vector3(horizontal, 0f, vertical).normalized;

        // Basis arah kamera (XZ plane)
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0f;
        camRight.Normalize();

        // Arah gerak relatif kamera
        Vector3 moveDir = camForward * inputDir.z + camRight * inputDir.x;
        moveDir.Normalize();

        // Jika bergerak → rotasi
        if (moveDir.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Terapkan gerak horizontal
        Vector3 move = moveDir * moveSpeed * Time.deltaTime;
        controller.Move(move);

        // Terapkan gravitasi manual
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // biar tetap nempel di tanah
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        // Gerak vertikal (gravitasi)
        controller.Move(velocity * Time.deltaTime);
    }
}
