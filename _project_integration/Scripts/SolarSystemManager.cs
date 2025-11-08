using UnityEngine;

public class SolarSystemManager : MonoBehaviour
{
    [Range(0.1f, 10f)] public float timeScale = 1f;
    public bool pause = false;

    void Update()
    {
        if (pause) return;

        Time.timeScale = timeScale;
    }
}
