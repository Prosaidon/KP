
using UnityEngine;
using UnityEngine.SceneManagement;

public class MulaiManager : MonoBehaviour
{
    // Start is called before the first frame update
    public void Level1()
    {
        Debug.Log ("Level1");
        SceneManager.LoadScene("Level1");
    }
    public void Boss()
    {
        SceneManager.LoadScene("Boss");
    }
    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
