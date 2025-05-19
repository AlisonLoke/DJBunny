using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private List <AK.Wwise.Event> instruments = new List<AK.Wwise.Event>();
    public AK.Wwise.Event drumSoundTest = null;


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



    public GameObject PlayMusic(AK.Wwise.Event instrument)
    {
        if(instrument == null)
        {
            Debug.LogWarning("clip is null in PlayMusic");
            return null; 
        }

        GameObject newAudioObj = new GameObject("Audio_" + instrument);
        newAudioObj.transform.SetParent(this.transform);


        //AudioSource newSource = newAudioObj.AddComponent<AudioSource>();
        //newSource.clip = clip;
        //newSource.loop = true; 
        instrument.Post(newAudioObj);

        instruments.Add(instrument);

        Debug.Log("Playing layered sound: " + instrument);
        return newAudioObj;
    }

    //public void StopMusic()
    //{
    //    foreach (AudioSource source in audioSources)
    //    {
    //        if (source != null)
    //        {
    //            source.Stop();
    //            Destroy(source.gameObject);
    //        }
    //    }

    //    audioSources.Clear();
    //}
}
