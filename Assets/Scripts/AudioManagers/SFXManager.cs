using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;
    [Header("BlockSFX")]
    public AK.Wwise.Event PlayPickUp = null;
    public AK.Wwise.Event PlayDrop = null;
    public AK.Wwise.Event PlayRotation = null;
    public AK.Wwise.Event PlayBlink = null;
    public AK.Wwise.Event PlayCompletion = null;


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
  

    }
}
