using UnityEngine;

public class CameraFollowV1 : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 2, -10);
    public float followSpeed = 8f;
    public float lookSpeed = 8f;
    public float zoomSpeed = 5f;
    public float minZoom = -20f;
    public float maxZoom = -4f;

    void LateUpdate()
    {
        if (!target) return;

        // scroll zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        offset.z = Mathf.Clamp(offset.z + scroll * zoomSpeed, minZoom, maxZoom);

        // pos camera smooth
        Vector3 desiredPosition = target.TransformPoint(offset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // rotasi camera ke target
        Quaternion lookRot = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, lookSpeed * Time.deltaTime);
    }
}
