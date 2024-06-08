using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            UnlockNewLevel();
        }
    }

    void UnlockNewLevel()
    {
        // Jika level saat ini lebih besar atau sama dengan level yang terbuka, buka level berikutnya
        if (SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ReachedIndex"))
        {
            PlayerPrefs.SetInt("ReachedIndex", SceneManager.GetActiveScene().buildIndex + 1);
            PlayerPrefs.SetInt("UnlockedLevel", PlayerPrefs.GetInt("UnlokedLevel",1) + 1);
            PlayerPrefs.Save();
        }
    }*/
}
