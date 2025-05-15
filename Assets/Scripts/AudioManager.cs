using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private AudioSource audioSource;
    public AudioClip testClip;


    private void Awake()
    {
   

        instance = this;
  

        audioSource = gameObject.GetComponent<AudioSource>();
    }
    private void Start()
    {
        TestPlayAudio();
    }

    public void TestPlayAudio()
    {
        if (testClip != null)
        {
            audioSource.clip = testClip;
            audioSource.Play();
            Debug.Log($"Test sound playing: {testClip.name}, Is Playing: {audioSource.isPlaying}");
        }
        else
        {
            Debug.LogError("Test clip not assigned!");
        }
    }
    public void PlayMusic(AudioClip clip)
    {
        Debug.Log("Trying to play sound: " + (clip != null ? clip.name : "null"));
        Debug.Log("AudioSource is playing? " + audioSource.isPlaying);
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
            Debug.Log("Playing sound: " + clip.name);
        }
        else
        {
            Debug.LogWarning("Clip is null in PlaySound()");
        }
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }
}
