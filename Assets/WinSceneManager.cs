using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinSceneManager : MonoBehaviour
{
    public AK.Wwise.Event playFullTrack = null;
    private void Update()
    {
        playFullTrack.Post(gameObject);
    }

    void Start()
    {
        StartCoroutine(GoToNextLevel());
    }

    private IEnumerator GoToNextLevel()
    {
        yield return new WaitForSeconds(1f);

        ConnectionSystem.currentLevelIndex++;
        string nextLevel = "Level" + ConnectionSystem.currentLevelIndex.ToString("D2");
        SceneManager.LoadScene(nextLevel);
    }

}
