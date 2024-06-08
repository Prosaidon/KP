using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour
{
    public Button[] buttons;
    public GameObject levelButton;

    private void Awake()
    {
        ButtonsToArray(); // Memanggil fungsi untuk mengisi array buttons
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        // Menonaktifkan semua tombol
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }

        // Mengaktifkan tombol sesuai level yang sudah terbuka
        for (int i = 0; i < unlockedLevel; i++)
        {
            if (i < buttons.Length)
            {
                buttons[i].interactable = true;
            }
        }
    }

    public void OpenLevel(int levelId)
    {
        string levelName = "Level" + levelId;
        // Cek apakah pemain sudah menyelesaikan level sebelumnya sebelum membuka level berikutnya
        int previousLevel = levelId - 1;
        if (previousLevel > 0 && previousLevel <= PlayerPrefs.GetInt("UnlockedLevel", 1))
        {
            SceneManager.LoadScene(levelName);
        }
        else if (previousLevel > 0)
        {
            Debug.Log("Anda harus menyelesaikan level sebelumnya terlebih dahulu!");
            // Mungkin tambahkan pesan kepada pemain bahwa mereka harus menyelesaikan level sebelumnya
        }
        else
        {
            SceneManager.LoadScene(levelName);
        }
    }

    void ButtonsToArray()
    {
        int childCount = levelButton.transform.childCount;
        buttons = new Button[childCount];
        for (int i = 0; i < childCount; i++)
        {
            buttons[i] = levelButton.transform.GetChild(i).gameObject.GetComponent<Button>();
        }
    }
}
