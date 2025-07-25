using System.Collections.Generic;
using UnityEngine;

public class LevelMusicManager : MonoBehaviour
{

    [Header("This level's music setup")]
    public AK.Wwise.Event levelMusic;


    private void Start()
    {
        if (MusicManager.instance != null)
        {
            MusicManager.instance.SetLevelMusic(levelMusic);
        }
        else
        {
            Debug.LogWarning("AudioManager instance not found.");
        }
    }
}
