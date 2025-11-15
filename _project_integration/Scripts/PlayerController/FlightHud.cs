using UnityEngine;
using TMPro;

public class FlightHUD : MonoBehaviour
{
    public Transform plane;
    public Rigidbody rb;

    public RectTransform horizonLine;
    public TextMeshProUGUI speedText;

    void Update()
    {
        // Roll pesawat → horizon miring
        float roll = plane.eulerAngles.z;
        if (roll > 180) roll -= 360;
        horizonLine.localRotation = Quaternion.Euler(0, 0, -roll);

        // Speed indikator
        float speed = rb.velocity.magnitude;
        speedText.text = $"{speed:0} m/s";
    }
}
