using UnityEngine;

public class CrosshairFollowMouseV1 : MonoBehaviour
{
    RectTransform rect;
    void Start() => rect = GetComponent<RectTransform>();
    void Update() => rect.position = Input.mousePosition;
}
