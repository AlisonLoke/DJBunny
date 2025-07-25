using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    private List<GameObject> instruments = new List<GameObject>();
    //private Queue<AK.Wwise.Event> musicQueue = new Queue<AK.Wwise.Event>();
    //private bool IsMusicPlaying = false;
    private List<AK.Wwise.Event> activeInstrumentLayers = new();
    private GameObject currentAudioObject;
    public AK.Wwise.Event LevelMusic = null;
    public AK.Wwise.Event PlayMusic = null;
   
 



    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Debug.Log("Starting to play music");
        PlayMusic?.Post(gameObject);

    }


    public void SetLevelMusic(AK.Wwise.Event levelMusic)
    {
        Debug.Log("Setting Level Music");
        levelMusic?.Post(gameObject);
       
    }

    public void PlayInstruments(AK.Wwise.Event instruments)
    {
        if (instruments == null) return;
        instruments.Post(gameObject);
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

    //public void StopMusic()
    //{
    //    if (currentAudioObject != null)
    //    {
    //        AkUnitySoundEngine.StopAll(currentAudioObject);
    //        Destroy(currentAudioObject);
    //        instruments.Remove(currentAudioObject);
    //        currentAudioObject = null;
    //    }
    //}




}
