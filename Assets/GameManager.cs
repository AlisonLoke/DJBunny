using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public void PlayGame()
    {
        SFXManager.instance.PlayButtonUI();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void RestartLastLevel()
    {
        SceneManager.LoadScene("CS_Level05_st01");
    }
    public void LoadCreditsMenu()
    {
        SceneManager.LoadScene("Credits");
    }
    public void OnApplicationQuit()
    {
        Debug.Log("QUIT");
        Application.Quit();
    }

}
