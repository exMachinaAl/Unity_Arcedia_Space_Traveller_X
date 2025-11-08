using UnityEngine;
using System.Collections.Generic;

public class Game_FloatingOrigin : MonoBehaviour
{
    [Header("Assign Player Root Transform")]
    public Transform player;

    [Header("Jarak batas sebelum dunia digeser")]
    public float threshold = 500f;

    public static Vector3 globalOffset = Vector3.zero;
    private static List<Transform> sceneObjects = new List<Transform>();

    void Start()
    {
        // Catat semua transform di scene saat start
        sceneObjects.Clear();
        foreach (var obj in FindObjectsOfType<Transform>())
        {
            // Jangan geser player root
            if (obj != player)
                sceneObjects.Add(obj);
        }
    }

    void LateUpdate()
    {
        if (!player) return;

        Vector3 playerPos = player.position;

        // Jika player menjauh lebih dari threshold
        if (playerPos.magnitude > threshold)
        {
			float y = playerPos.y;
            // Geser semua objek kecuali player
            foreach (var obj in sceneObjects)
                obj.position -= playerPos;

            // Catat offset dunia (global)
            globalOffset += playerPos;

            // Reset player ke pusat
            player.position = Vector3.zero;
			//player.position = new Vector3(0, y, 0);

            // Sinkronisasi physics agar tidak “nabrak bayangan”
            Physics.SyncTransforms();

            Debug.Log($"[FloatingOrigin] Dunia digeser, offset global: {globalOffset}");
        }
    }

    void OnDrawGizmos()
    {
        if (!player) return;

        // Radius floating
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(player.position, threshold);

        // Posisi global player (untuk debug)
        Vector3 globalPos = player.position + globalOffset;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(globalPos, Vector3.one * 10);
    }
}
