using UnityEngine;

public class SceneRegistry : MonoBehaviour
{
    public static SceneRegistry I;
    public Transform player; // assign (or set at runtime)

    void Awake() {
        if (I == null) { I = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }
}