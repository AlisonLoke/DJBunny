using System.Collections.Generic;
using UnityEngine;

public class StageMusicManager : MonoBehaviour
{

    [Header("This level's music setup")]
    public AK.Wwise.Event nextStageMusic;


    private void Start()
    {
        if (MusicManager.instance != null)
        {
            MusicManager.instance.SetLevelMusic(nextStageMusic);
        }
        else
        {
            //Debug.LogWarning("AudioManager instance not found.");
        }
    }
}
