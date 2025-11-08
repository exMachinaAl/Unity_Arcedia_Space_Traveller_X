using UnityEngine;
using System.Collections.Generic;

public class Game_FloatingOriginV2 : MonoBehaviour
{
    [Header("Assign Player Root Transform")]
    public Transform player;

    [Header("Jarak batas sebelum dunia digeser")]
    public float threshold = 500f;

    public static Vector3 globalOffset = Vector3.zero;

    // Daftar objek dunia yang akan ikut geser (opsional)
    private static List<Transform> sceneObjects = new();

    void Start()
    {
        RefreshSceneObjects();
    }

    void LateUpdate()
    {
        if (!player) return;

        // Pastikan tidak trigger terus-menerus
        if (player.position.sqrMagnitude > threshold * threshold)
        {
            // Simpan posisi Y player agar tidak nembus tanah
            float y = player.position.y;

            Vector3 shift = new Vector3(player.position.x, 0, player.position.z);

            // Geser semua objek kecuali player
            foreach (var obj in sceneObjects)
            {
                if (obj != null && obj != player)
                    obj.position -= shift;
            }

            // Catat offset dunia
            globalOffset += shift;

            // Reset player ke tengah, tapi pertahankan Y
            player.position = new Vector3(0, y, 0);

            // Sinkronisasi physics
            Physics.SyncTransforms();

            Debug.Log($"[Game_FloatingOriginV2] Dunia digeser, offset global: {globalOffset}");
        }
    }

    // Tambahkan fungsi untuk refresh otomatis (misal saat chunk spawn)
    public static void RegisterObject(Transform t)
    {
        if (!sceneObjects.Contains(t))
            sceneObjects.Add(t);
    }

    public static void UnregisterObject(Transform t)
    {
        sceneObjects.Remove(t);
    }

    public void RefreshSceneObjects()
    {
        sceneObjects.Clear();
        foreach (var obj in FindObjectsOfType<Transform>())
        {
            if (obj != player && obj.parent == null) // hanya root scene object
                sceneObjects.Add(obj);
        }
    }

    void OnDrawGizmos()
    {
        if (!player) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Vector3.zero, threshold);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(10, 10, 10));
    }
}
