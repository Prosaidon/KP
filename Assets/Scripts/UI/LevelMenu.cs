using System.Collections;
using System.Collections.Generic;
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

    public void OpenLevel(int LevelId)
    {
        string LevelName = "Level" + LevelId;
        SceneManager.LoadScene(LevelName);
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
