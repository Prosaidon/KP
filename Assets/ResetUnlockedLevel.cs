using UnityEngine;

public class ResetUnlockedLevel : MonoBehaviour
{
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("UnlockedLevel", 1); // Setel ulang level yang terbuka ke 1
        PlayerPrefs.Save();
    }
}
