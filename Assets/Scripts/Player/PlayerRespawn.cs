using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
   // [SerializeField] private AudioClip checkpoint;
    private Transform currentCheckpoint;
    private Health playerHealth;
    private UIManager uIManager;

    private void Awake()
    {
        playerHealth = GetComponent<Health>();
        uIManager = FindObjectOfType<UIManager>();
    }

    public void CheckRespawn()
    {
        if (currentCheckpoint == null) 
        {
            uIManager.GameOver();
            return;
        }
            playerHealth.Respawn();
            transform.position = currentCheckpoint.position;

            //Camera.main.GetComponent<camera>().MoveToNewRoom(currentCheckpoint.parent); // Mengirim transformasi checkpoint ke kamera
    }

    private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Checkpoint") // Periksa tag dengan CompareTag
            {
                currentCheckpoint = collision.transform;
                //SoundManager.instance.PlaySound(checkpoint);
                collision.GetComponent<Collider2D>().enabled = false;
                //collision.GetComponent<Animator>().SetTrigger("activate");

            }
        }
}