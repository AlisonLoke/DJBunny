using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private List <AudioSource> audioSources = new List<AudioSource>();
 


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



    public GameObject PlayMusic(AudioClip clip)
    {
        if(clip == null)
        {
            Debug.LogWarning("clip is null in PlayMusic");
            return null; 
        }

        GameObject newAudioObj = new GameObject("Audio_" + clip.name);
        newAudioObj.transform.SetParent(this.transform);


        AudioSource newSource = newAudioObj.AddComponent<AudioSource>();
        newSource.clip = clip;
        newSource.loop = true; 
        newSource.Play();

        audioSources.Add(newSource);

        Debug.Log("Playing layered sound: " + clip.name);
        return newAudioObj;
    }

    public void StopMusic()
    {
        foreach (AudioSource source in audioSources)
        {
            if (source != null)
            {
                source.Stop();
                Destroy(source.gameObject);
            }
        }

        audioSources.Clear();
    }
}
