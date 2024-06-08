using UnityEngine;

public class UI : MonoBehaviour
{
    // Fungsi ini akan dipanggil untuk keluar dari permainan atau menghentikan mode permainan di editor
    public void Keluar()
    {
        PlayerPrefs.SetInt("UnlockedLevel", 1); 
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
