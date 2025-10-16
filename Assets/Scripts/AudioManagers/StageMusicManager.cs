using System.Collections.Generic;
using UnityEngine;

public class StageMusicManager : MonoBehaviour
{

    [Header("This level's music setup")]
    public AK.Wwise.Event nextStageMusic;
    public AK.Wwise.Event StopMusicEvent = null;

    //    private void Start()
    //    {
    //        if (MusicManager.instance != null)
    //        {
    //            MusicManager.instance.SetLevelMusic(nextStageMusic,StopMusicEvent);
    //        }
    //        else
    //        {
    //            //Debug.LogWarning("AudioManager instance not found.");
    //        }
    //    }
    //}
}
