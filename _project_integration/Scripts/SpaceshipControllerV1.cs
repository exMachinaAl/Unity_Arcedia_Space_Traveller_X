using UnityEngine;
using UnityEngine.UI;

public class SpaceshipControllerV1 : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 25f;
    public float rotationSpeed = 3f;
    public float acceleration = 10f;
    public float deceleration = 10f;

    [Header("Auto Fly Mode")]
    public bool autoFly = false;

    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 100f;
    public float fireCooldown = 0.2f;

    [Header("References")]
    public Camera mainCamera;
    public RectTransform crosshairUI;
    public Text autoFlyStatusText;		


    private float currentSpeed = 0f;
    private Vector3 targetDirection;
    private float nextFireTime = 0f;

    private Vector3 smoothDirection; // smoothing untuk arah

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        if (autoFlyStatusText)
            autoFlyStatusText.text = "Auto Fly: OFF";
		
    }



    void Update()
    {
        HandleInput();
        HandleCrosshairDirection();
        HandleShooting();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            autoFly = !autoFly;
            if (autoFlyStatusText)
                autoFlyStatusText.text = "Auto Fly: " + (autoFly ? "ON" : "OFF");
        }

        if (!autoFly)
        {
            if (Input.GetKey(KeyCode.W))
                currentSpeed = Mathf.Lerp(currentSpeed, moveSpeed, acceleration * Time.deltaTime);
            else if (Input.GetKey(KeyCode.S))
                currentSpeed = Mathf.Lerp(currentSpeed, -moveSpeed * 0.5f, deceleration * Time.deltaTime);
            else
                currentSpeed = Mathf.Lerp(currentSpeed, 0f, deceleration * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, moveSpeed, acceleration * Time.deltaTime);
        }
    }

    void HandleCrosshairDirection()
    {
        Vector3 mousePos = Input.mousePosition;
        crosshairUI.position = mousePos;

        Ray ray = mainCamera.ScreenPointToRay(mousePos);
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

    void HandleMovement()
    {
        transform.position += transform.forward * currentSpeed * Time.fixedDeltaTime;
    }

    void HandleShooting()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireCooldown;
            Shoot();
        }
    }

	void Shoot()
	{
		if (bulletPrefab == null || firePoint == null || mainCamera == null)
		{
			Debug.LogWarning("Missing reference: bulletPrefab, firePoint, or mainCamera");
			return;
		}

		// Tentukan arah bidik dari crosshair di layar ke dunia
		Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		Vector3 targetPoint;

		// Jika ray dari crosshair mengenai sesuatu (musuh, dinding, dll)
		if (Physics.Raycast(ray, out hit, 1000f))
		{
			targetPoint = hit.point;
		}
		else
		{
			// Jika tidak kena apa-apa, targetnya adalah titik jauh ke depan
			targetPoint = ray.GetPoint(1000f);
		}

		// Hitung arah peluru dari FirePoint menuju target
		Vector3 direction = (targetPoint - firePoint.position).normalized;

		// Spawn peluru
		GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));

		// Terapkan gaya atau kecepatan
		Rigidbody rb = bullet.GetComponent<Rigidbody>();
		if (rb != null)
		{
			rb.velocity = direction * bulletSpeed;
		}

		Destroy(bullet, 3f);
	}



}
