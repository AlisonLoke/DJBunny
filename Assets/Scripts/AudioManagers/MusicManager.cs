using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Only the first MusicManager loaded remains in active. Future ones are destroyed.
/// You only need one in the scene if you want to "Play" from there.
/// Note: which music plays when is FULLY controlled by LevelManager.
/// </summary>
public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;


    private GameObject currentAudioObject;
    //stops all music
    public AK.Wwise.Event StopLevelMusic = null;
    //public AK.Wwise.Event currentLevelMusicEvent = null;
    //public AK.Wwise.Event StartLevelMusic = null;
    //end level triggers puzzle complete stinger
    //public AK.Wwise.Event EndLevelMusic = null;
    public AK.Wwise.Event PlayMusic = null;
    public AK.Wwise.Event StartMenuMusic = null;

    [SerializeField] private bool isStartMenu = false;




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
        if (isStartMenu)
        {
            StartMenuMusic?.Post(gameObject);
        }
    }

    public void PlayLevelMusic(AK.Wwise.Event targetLevelMusic)
    {
        targetLevelMusic.Post(gameObject);
    }

    public void PlayInstruments(AK.Wwise.Event instruments)
    {
        if (instruments == null) return;
        instruments.Post(gameObject);
    }






    public void StopCurrentLevelMusic()
    {

        if (StopLevelMusic != null)
        {
            Debug.Log("Stopping current Level Music");
            StopLevelMusic.Post(gameObject);
        }
    }

    public void ResetForNewLevel(AK.Wwise.Event levelMusic, AK.Wwise.Event stopEvent)
    {
        Debug.Log("Resetting Music Manager for new level.");

     

        PlayLevelMusic(levelMusic);

    }


}
