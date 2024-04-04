using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header ("Game Over")]
    [SerializeField] private GameObject gameOverScreen;
    //[SerializeField] private AudioClip gameOverSound;

    [Header("Pause")]
    [SerializeField] private GameObject pauseScreen;

    [Header("Win")]
    [SerializeField] private GameObject winScreen;

    public static int enemyCount = 3;
    AudioManager audioManager;


    private void Awake()
    {
        gameOverScreen.SetActive(false);
        pauseScreen.SetActive(false);
        winScreen.SetActive(false);
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    private void Start()
    {
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !gameOverScreen.activeInHierarchy && !winScreen.activeInHierarchy) // Tambahkan kondisi untuk memeriksa layar kalah atau menang sedang aktif atau tidak
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
        audioManager.PlaySFX(audioManager.GameOver);
        audioManager.PauseBackgroundMusic();
        Time.timeScale = 0;    
    }

    //Restart level
    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(2);
    }

    //Main Menu
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 0;
    }

    //Quit game/exit play mode if in Editor
    public void Quit()
    {
        Application.Quit(); //Quits the game (only works in build)

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; //Exits play mode (will only be executed in the editor)
#endif
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
        {
            Time.timeScale = 0;
            audioManager.PauseBackgroundMusic();
        }else{
            Time.timeScale = 1;
            audioManager.ResumeBackgroundMusic();
        }
    }
    /*public void SoundVolume()
    {
        SoundManager.instance.ChangeSoundVolume(0.2f);
    }
    public void MusicVolume()
    {
        SoundManager.instance.ChangeMusicVolume(0.2f);
    }*/
    #endregion

    #region Win
    public void EnemyKilled()
    {
         enemyCount--; // Mengurangi jumlah musuh yang tersisa
         //Time.timeScale = 0;
        if (enemyCount <= 0)
        {
            // Panggil UIManager untuk menampilkan layar kemenangan
            UIManager uiManager = FindObjectOfType<UIManager>();
            if (uiManager != null)
            {
                uiManager.ShowWinScreen();
            }
        }
    }
    
    public void ShowWinScreen()
    {
    
            winScreen.SetActive(true);
            audioManager.PlaySFX(audioManager.Win);
            audioManager.PauseBackgroundMusic();
            Time.timeScale = 0;
        
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    public void QuitGame()
    {
        Application.Quit();
        
    }
    #endregion

    public void Boss()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Boss");
    }
}