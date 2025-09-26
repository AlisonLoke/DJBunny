using UnityEngine;
using UnityEngine.SceneManagement;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;
    [Header("Atmos")]
    [SerializeField] private AK.Wwise.Event startAtmos;
    [SerializeField] private AK.Wwise.Event playCurrentAtmos;

    [Header("BlockSFX")]
    public AK.Wwise.Event PlayPickUp = null;
    public AK.Wwise.Event PlayDrop = null;
    public AK.Wwise.Event PlayRotation = null;

    public AK.Wwise.Event PlayCompletion = null;


    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
           
            startAtmos.Post(gameObject);
            SceneManager.sceneLoaded += OnSceneLoad;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

    }
    //private void Start()
    //{
    //    playCurrentAtmos.Post(gameObject);
    //}
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }
    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        playCurrentAtmos.Post(gameObject);
    }
}
