using UnityEngine;

public class RocketLaunch : MonoBehaviour
{
    public float startDelay = 2f;
    public float speed = 1f;
    public float acceleration = 0.5f;

    private bool launching = false;

    void Start()
    {
        Invoke(nameof(StartLaunch), startDelay);
    }

    void StartLaunch()
    {
        launching = true;
    }

    void Update()
    {
        if (!launching) return;

        speed += acceleration * Time.deltaTime;
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }
}
