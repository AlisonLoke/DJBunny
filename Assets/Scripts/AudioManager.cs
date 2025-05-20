using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    //private List <AK.Wwise.Event> instruments = new List<AK.Wwise.Event>();
    private List<GameObject> instruments = new List<GameObject>();



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
        instrument.Post(newAudioObj); // post music to this game object

        instruments.Add(newAudioObj);

        Debug.Log("Playing layered sound: " + instrument);
        return newAudioObj;
    }

    public void StopMusic()
    {
        foreach (GameObject instrument in instruments)
        {
            if (instrument != null)
            {
                AkUnitySoundEngine.StopAll(instrument);
                Destroy(instrument);
                
            }
        }

        instruments.Clear();
    }
}
