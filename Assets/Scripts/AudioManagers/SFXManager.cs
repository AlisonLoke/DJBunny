using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;
    public AK.Wwise.Event PlayCompletion = null;
    [Header("Atmos")]
    [SerializeField] private AK.Wwise.Event startAtmos;
    [SerializeField] private AK.Wwise.Event playCurrentAtmos;

    [Header("BlockSFX")]
    public AK.Wwise.Event PlayPickUp = null;
    public AK.Wwise.Event PlayDrop = null;
    public AK.Wwise.Event PlayRotation = null;

    [Header("DialogueSFX")]
    [SerializeField] private AK.Wwise.Event playDialogueSfx;
    [SerializeField] private AK.Wwise.Event stopDialogueSfx;
    public bool allowDialogueSFX = true;


    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
           
            startAtmos.Post(gameObject);
            SceneManager.sceneLoaded += OnSceneLoad;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

    }
  
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }
    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (playCurrentAtmos != null)
        {
            playCurrentAtmos.Post(gameObject);
            Debug.Log($"[SFXManager] Scene '{scene.name}'Playing Atmos Event: {playCurrentAtmos.Name}");
        }
        else
        {
            Debug.LogWarning($"[SFXManager] Scene '{scene.name}' No Atmos event assigned!");
        }
    }

    public void TriggerDialogueSFX()
    {
        if (!allowDialogueSFX)
        {
            return;
        }

        playDialogueSfx.Post(gameObject);
    }
    public void StopDialogueSFXAtEnd()
    {
        stopDialogueSfx.Post(gameObject);
    }

    public void DisableDialogueSFX() => allowDialogueSFX = false;
    public void EnableDialogueSFX() => allowDialogueSFX=true;
}

