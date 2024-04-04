using UnityEngine;

public class camera : MonoBehaviour
{
    [SerializeField] private Transform target; // Transformasi pemain yang ingin diikuti oleh kamera
    [SerializeField] private float smoothSpeed = 0.125f; // Kecepatan pergerakan kamera

    private Vector3 offset; // Jarak antara kamera dan pemain

    private void Start()
    {
        // Menghitung jarak antara kamera dan pemain saat inisialisasi
        if (target != null)
        {
            offset = transform.position - target.position;
        }
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            // Menghitung posisi target yang diikuti oleh kamera
            Vector3 desiredPosition = target.position + offset;

            // Menggunakan fungsi SmoothDamp untuk pergerakan yang halus
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }

    // Fungsi untuk mengatur pemain yang ingin diikuti oleh kamera
    public void SetTarget(Transform newTarget)
    {
        if (newTarget != null)
        {
            target = newTarget;
            offset = transform.position - target.position;
        }
        else
        {
            Debug.LogWarning("Target pemain tidak valid!");
        }
    }
}
