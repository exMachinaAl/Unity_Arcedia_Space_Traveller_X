using UnityEngine;

public class Manager_Audio : MonoBehaviour
{
    public static Manager_Audio Instance;
    public AudioSource audioSource;
    // public Transform player; // Untuk mendeteksi jarak dengan pemain
    public float maxDistance = 10f; // Jarak maksimal untuk pengaturan volume

    private void Awake()
    {
        // Jika instance sudah ada, hancurkan objek baru yang dibuat
        if (Manager_Audio.Instance != null && Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Update()
    {
        // AdjustVolumeBasedOnDistance();
    }

    // Mengatur volume berdasarkan jarak antara pemain dan AudioSource
    // public void AdjustVolumeBasedOnDistance()
    // {
    //     if (audioSource != null && player != null)
    //     {
    //         float distance = Vector3.Distance(player.position, audioSource.transform.position);
    //         audioSource.volume = Mathf.Clamp01(1 - (distance / maxDistance));
    //     }
    // }

    // Method untuk memainkan suara dengan volume otomatis sesuai jarak
    public void PlayAudioClip(AudioSource audioSrc, AudioClip clip)
    {
        audioSource = audioSrc;

        audioSource.clip = clip;
        audioSource.Play();
    }
}
