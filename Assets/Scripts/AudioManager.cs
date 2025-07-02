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
    //public void QueueMusic(AK.Wwise.Event instrument)
    //{
    //    if (instrument == null) return;

    //    musicQueue.Enqueue(instrument);

    //    if (!IsMusicPlaying)
    //        StartCoroutine(ProcessMusicQueue());
    //}

    //private IEnumerator ProcessMusicQueue()
    //{
    //    while (musicQueue.Count > 0)
    //    {
    //        IsMusicPlaying = true;

    //        AK.Wwise.Event currentInstrument = musicQueue.Dequeue();
    //        GameObject audioObj = new GameObject("Audio_" + currentInstrument);
    //        audioObj.transform.SetParent(this.transform);

    //        instruments.Add(audioObj);

    //        Debug.Log("Playing: " + currentInstrument);

    //        // Use a callback to detect end
    //        uint playingId = currentInstrument.Post(audioObj, (uint)AkCallbackType.AK_EndOfEvent, OnMusicEnd);

    //        // Wait until the callback marks the music done
    //        while (IsMusicPlaying)
    //            yield return null;

    //        Destroy(audioObj);
    //    }
    //}
    //private void OnMusicEnd(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info)
    //{
    //    IsMusicPlaying = false;
    //}



    //public void StopMusic()
    //{
    //    foreach (GameObject instrument in instruments)
    //    {
    //        if (instrument != null)
    //        {
    //            AkUnitySoundEngine.StopAll(instrument);
    //            Destroy(instrument);

    //        }
    //    }

    //    instruments.Clear();
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
