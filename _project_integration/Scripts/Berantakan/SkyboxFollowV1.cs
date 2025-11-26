using UnityEngine;

public class SkyboxFollowV1 : MonoBehaviour
{
    public Transform player;

    void LateUpdate()
    {
        if (player)
            transform.rotation = player.rotation; // Kamera langit selalu ikut posisi player
    }
}
