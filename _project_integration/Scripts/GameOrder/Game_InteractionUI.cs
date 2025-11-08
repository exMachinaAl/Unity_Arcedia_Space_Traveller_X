using UnityEngine;
using TMPro;

public class Game_InteractionUI : MonoBehaviour
{
    public static Game_InteractionUI Instance;

    public TextMeshPro textMesh;
    private Transform target;

    void Awake()
    {
        Instance = this;
    }

    public void ShowText(string message, Transform targetTransform)
    {
        target = targetTransform;
        textMesh.text = message;
        textMesh.gameObject.SetActive(true);
    }

    public void HideText()
    {
        textMesh.gameObject.SetActive(false);
        target = null;
    }

    void LateUpdate()
    {
        if (target == null) return;

        textMesh.transform.position = target.position + Vector3.up * 2f;
        textMesh.transform.LookAt(Camera.main.transform);
        textMesh.transform.Rotate(0, 180, 0); // because Text faces backward by default
    }
}
