using UnityEngine;

public class FPSCamera : MonoBehaviour
{
    public float sensitivity = 300f;
    public Transform playerBody;

    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // kunci cursor di tengah
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // rotasi kamera atas/bawah
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        // rotasi badan player kiri/kanan
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
