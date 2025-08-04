using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

  
    private GameObject currentAudioObject;
    public AK.Wwise.Event currentLevelMusicEvent = null;
    public AK.Wwise.Event StartLevelMusic = null;
    public AK.Wwise.Event PlayMusic = null;





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

        if (instance != null)
        {
            SetLevelMusic(StartLevelMusic);
        }
        else
        {
            Debug.LogWarning("AudioManager instance not found.");
        }
    }
    public void SetLevelMusic(AK.Wwise.Event levelMusic)
    {
        if(currentLevelMusicEvent != null)
        {
            Debug.Log("Stopping previous level music.");
            currentLevelMusicEvent.Stop(gameObject);
        }
        currentLevelMusicEvent= levelMusic;
        currentLevelMusicEvent?.Post(gameObject);
      

    }

    public void PlayInstruments(AK.Wwise.Event instruments)
    {
        if (instruments == null) return;
        instruments.Post(gameObject);
    }




   
    public void StopCurrentLevelMusic()
    {
        if (currentLevelMusicEvent != null)
        {
            Debug.Log("Stopping current level music.");
            currentLevelMusicEvent.Stop(gameObject);
            currentLevelMusicEvent = null; // Optional: clears reference
        }
    }




}
