using UnityEngine;
using UnityEngine.SceneManagement;

public class CutSceneManager : MonoBehaviour
{
    [SerializeField] private float sceneLegth;
    [SerializeField] private string sceneName;

    private void Update()
    {
        sceneLegth -= Time.deltaTime;
        if(sceneLegth <= 0)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
