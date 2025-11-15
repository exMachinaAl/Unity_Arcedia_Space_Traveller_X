using UnityEngine;
using UnityEngine.SceneManagement;


public class FlightControllerV1 : MonoBehaviour
{

    [Header("Movement")]
    public float forwardSpeed = 40f;
    public float acceleration = 10f;

    [Header("Auto Stabilizer")]
    public bool enableStabilizer = true;
    public float stabilizerStrength = 2f;

    [Header("Rotation")]

    public float rotationSpeed = 5f;   // Kecepatan respon rotasi
    public float rollSpeed = 100f;     // Barrel roll 360°
    public float mouseSensitivity = 0.1f;

    [Header("Physics")]
    public Rigidbody rb;

    [Header("Crosshair Targeting")]
    public Camera shipCam;
    public RectTransform crosshairUI;
    public float aimDistance = 200f;

    private Vector3 smoothDir;
    private float currentRoll = 0f;        // assign dari inspector
    public float speed = 20f;
    // public float rotationSpeed = 50f;
    // [Header("Flight Banking")]
    // public float maxBankAngle = 35f; // derajat miring sayap


    private Vector3 smoothDirection; // smoothing untuk arah

    private bool isControlled = false;
    // private Rigidbody rb;

    public Transform exitPoint;          // tempat player keluar dari pesawat

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // rb.isKinematic = true; // kapal tidak bergerak sebelum dinaiki
        rb.useGravity = true; // kapal tidak bergerak sebelum dinaiki
    }

    void Update()
    {
        if (!isControlled) return;

        HandleCrosshairDirection();
        HandleExitShip();

        // float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // // Gerakkan pesawat
        Vector3 move = transform.forward * v * speed;
        rb.velocity = move;


        // HandleAiming();
        // HandleMovement();
        // HandleRoll();
        // if (Input.GetKey(KeyCode.Q))
        // {
        //     rb.angularVelocity = Vector3.up * -rotationSpeed * Mathf.Deg2Rad;
        // }
        // else if (Input.GetKey(KeyCode.E))
        // {
        //     rb.angularVelocity = Vector3.up * rotationSpeed * Mathf.Deg2Rad;
        // }
        // else
        // {
        //     rb.angularVelocity = Vector3.zero;
        // }

        // Rotasi pesawat
        // float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        // transform.Rotate(0, mouseX, 0);
    }

    void HandleExitShip()
    {
        if (isControlled && Input.GetKeyDown(KeyCode.R))
        {
            // gameObject.GetComponent<SpaceshipControllerV1>().DisableControl();
            FlightControllerV1 nich = this;
            Scene aktifScene = SceneManager.GetActiveScene();
            Transform ExitPos = transform.Find("Exit_Point");

            Manager_Player.Instance.ExitShip(nich, ExitPos, aktifScene);
        }
    }

    void HandleCrosshairDirection()
    {
        Vector3 mousePos = Input.mousePosition;
        // crosshairUI.position = mousePos;

        Ray ray = shipCam.ScreenPointToRay(mousePos);
        Plane plane = new Plane(transform.forward, transform.position + transform.forward * 100f);

        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 dir = (hitPoint - transform.position).normalized;

            // smoothing untuk arah agar tidak getar
            smoothDirection = Vector3.Lerp(smoothDirection, dir, Time.deltaTime * rotationSpeed);
            Quaternion targetRot = Quaternion.LookRotation(smoothDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }
    
    // =============================================================
    // 1. ARAH PESAWAT BERDASARKAN MOUSE
    // =============================================================
    void HandleAiming()
    {
        Vector3 mousePos = Input.mousePosition;
        // crosshairUI.position = mousePos;

        Ray ray = shipCam.ScreenPointToRay(mousePos);
        Plane plane = new Plane(transform.forward, transform.position + transform.forward * aimDistance);

        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 dir = (hitPoint - transform.position).normalized;

            // Smoothing biar tidak getar
            smoothDir = Vector3.Lerp(smoothDir, dir, Time.deltaTime * rotationSpeed);

            // Rotasi menuju arah crosshair
            Quaternion targetRot = Quaternion.LookRotation(smoothDir, transform.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
        }
    }

    // =============================================================
    // 2. GERAK MAJU DENGAN ACCELERATION
    // =============================================================
    void HandleMovement()
    {
        float targetSpeed = forwardSpeed;
        float currentSpeed = rb.velocity.magnitude;

        float newSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * acceleration);
        rb.velocity = transform.forward * newSpeed;
    }

    // =============================================================
    // 3. BARREL ROLL (360° bebas)
    // =============================================================
    void HandleRoll()
    {
        float h = Input.GetAxis("Horizontal");

        // Tekan A/D buat miring atau barrel roll
        currentRoll += -h * rollSpeed * Time.deltaTime;

        // Buat rotasi roll
        Quaternion rollRot = Quaternion.AngleAxis(currentRoll, transform.forward);

        // Terapkan roll ke rotasi yang sudah ditargetkan di aiming
        transform.rotation = rollRot * transform.rotation;

        if (enableStabilizer && Input.GetAxis("Horizontal") == 0)
        {
            // Arah "atas" pesawat kembali menuju Vector3.up
            Vector3 planeUp = transform.up;
            Vector3 stableUp = Vector3.up;

            Quaternion toStable = Quaternion.FromToRotation(planeUp, stableUp);
            Quaternion newRot = Quaternion.Slerp(Quaternion.identity, toStable, Time.deltaTime * stabilizerStrength);

            transform.rotation = newRot * transform.rotation;
        }

    }

    // Dipanggil dari PlayerManager saat player masuk pesawat
    public void EnableControl()
    {
        isControlled = true;
        // rb.isKinematic = false;
        rb.useGravity = false;

        if (shipCam != null)
            shipCam.gameObject.SetActive(true);

        gameObject.tag = "Player";
        gameObject.transform.Find("ShipModel").gameObject.tag = "Player";
    }

    // Dipanggil dari PlayerManager saat player keluar
    public void DisableControl()
    {
        isControlled = false;
        // rb.isKinematic = true;
        rb.useGravity = true;

        if (shipCam != null)
            shipCam.gameObject.SetActive(false);

        gameObject.tag = "Untagged";
        gameObject.transform.Find("ShipModel").gameObject.tag = "Untagged";
    }

    // Export data jika kamu perlu simpan pesawat
    public ShipData ExportData()
    {
        return new ShipData
        {
            position = transform.position,
            rotationEuler = transform.eulerAngles
        };
    }
}

[System.Serializable]
public class ShipData
{
    public Vector3 position;
    public Vector3 rotationEuler;
}
