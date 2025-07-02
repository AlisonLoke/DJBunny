using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    private List<GameObject> instruments = new List<GameObject>();
    private Queue<AK.Wwise.Event> musicQueue = new Queue<AK.Wwise.Event>();
    private bool IsMusicPlaying = false;
    private GameObject currentAudioObject;
    //public AK.Wwise.Event musicEvent;


    private void Awake()
    {
   
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    //private void Update()
    //{
    //    Debug.Log("MUSIC IS PLAYING RIGHT NOW");
    //    musicEvent.Post(gameObject);
    //}




    public GameObject PlayMusicNow(AK.Wwise.Event instrument)
    {
        if (instrument == null) return null;
        StopMusic();

        currentAudioObject = new GameObject("Audio_" + instrument.Name);
        currentAudioObject.transform.SetParent(this.transform);
        instruments.Add(currentAudioObject);

        instrument.Post(currentAudioObject);

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
