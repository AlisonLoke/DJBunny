using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] private AK.Wwise.RTPC masterVolume = null;
    [SerializeField] private AK.Wwise.RTPC musicVolume = null;
    [SerializeField] private AK.Wwise.RTPC sfxVolume = null;
    [Header("Volume Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;


    void Start()
    {
        UpdateVolumes();
    }

    public void UpdateVolumes()
    {
        masterVolume.SetGlobalValue(masterSlider.value);
        musicVolume.SetGlobalValue(musicSlider.value);
        sfxVolume.SetGlobalValue(sfxSlider.value);
    }
}
