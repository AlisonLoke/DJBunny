using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    private List<GameObject> instruments = new List<GameObject>();
    //private Queue<AK.Wwise.Event> musicQueue = new Queue<AK.Wwise.Event>();
    //private bool IsMusicPlaying = false;
    private GameObject currentAudioObject;
    public AK.Wwise.Event Lvl2_1 = null;
    public AK.Wwise.Event PlayMusic = null;
   
    [Header("BlockSFX")]
    public AK.Wwise.Event PlayPickUp = null;
    public AK.Wwise.Event PlayDrop = null;
    public AK.Wwise.Event PlayRotation = null;
    public AK.Wwise.Event PlayBlink = null;
    public AK.Wwise.Event PlayCompletion = null;



    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        Debug.Log("Starting to play music");
        PlayMusic.Post(gameObject);

    }

    private void Start()
    {
        Debug.Log("Loading Lvl2 music");
        Lvl2_1.Post(gameObject);

    }



    public GameObject Play(AK.Wwise.Event instrument)
    {
        if (instrument == null) return null;
        //StopMusic();

        //currentAudioObject = new GameObject("Audio_" + instrument.Name);
        //currentAudioObject.transform.SetParent(this.transform);
        //instruments.Add(currentAudioObject);

        instrument.Post(gameObject);

        return currentAudioObject;
    }

    public void StopMusic()
    {
        if (currentAudioObject != null)
        {
            AkUnitySoundEngine.StopAll(currentAudioObject);
            Destroy(currentAudioObject);
            instruments.Remove(currentAudioObject);
            currentAudioObject = null;
        }
    }




}
