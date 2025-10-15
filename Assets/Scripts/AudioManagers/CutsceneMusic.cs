using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneMusic : MonoBehaviour
{
    public static CutsceneMusic instance;
    public AK.Wwise.Event PlayMusic = null;
    public AK.Wwise.Event startBackgroundMusic = null;

    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Debug.Log("Starting to play music");
        PlayMusic?.Post(gameObject);

    }
    private void Start()
    {
        startBackgroundMusic?.Post(gameObject);
    }
}
