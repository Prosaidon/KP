using UnityEngine;

public class UI : MonoBehaviour
{
    // Fungsi ini akan dipanggil untuk keluar dari permainan atau menghentikan mode permainan di editor
    public void Keluar()
    {
        // Set default level to 1 in PlayerPrefs
        PlayerPrefs.SetInt("UnlockedLevel", 1);

        // Remove user data from PlayerPrefs
        PlayerPrefs.DeleteKey("UserName");
        PlayerPrefs.DeleteKey("UserEmail");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
