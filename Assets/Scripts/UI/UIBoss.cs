using UnityEngine;
using UnityEngine.SceneManagement;

public class UIBoss : MonoBehaviour
{
    [Header ("Game Over")]
    [SerializeField] private GameObject gameOverScreen;
    //[SerializeField] private AudioClip gameOverSound;

    [Header("Pause")]
    [SerializeField] private GameObject pauseScreen;

    private UIManager uiManager; // Tambahkan referensi ke UIManager

    private void Awake()
    {
        gameOverScreen.SetActive(false);
        pauseScreen.SetActive(false);

        // Temukan UIManager pada scene dan tetapkan sebagai referensi
        uiManager = FindObjectOfType<UIManager>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //If pause screen already active unpause and viceversa
            PauseGame(!pauseScreen.activeInHierarchy);
        }
    }

    #region Game Over
    //Activate game over screen
    public void GameOver()
    {
        gameOverScreen.SetActive(true);
        //SoundManager.instance.PlaySound(gameOverSound);
    }

    //Restart level
    /*public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Panggil fungsi Restart dari UIManager
    }
    */
    //Main Menu
    public void MainMenu()
    {
        SceneManager.LoadScene(0); // Panggil fungsi MainMenu dari UIManager
    }

    //Quit game/exit play mode if in Editor
    public void Quit()
    {
        Application.Quit(); // Panggil fungsi Quit dari UIManager
    }
    #endregion

    #region Pause
    public void PauseGame(bool status)
    {
        //If status == true pause | if status == false unpause
        pauseScreen.SetActive(status);

        //When pause status is true change timescale to 0 (time stops)
        //when it's false change it back to 1 (time goes by normally)
        if (status)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }
    #endregion
    public void level1()
    {
        SceneManager.LoadScene("Level1");
    }
}
