using UnityEngine;
using UnityEngine.UI;

public class InputBlocker : MonoBehaviour
{
    public static InputBlocker Instance;    
    [SerializeField] private Image inputBlocker;

    private void Awake()
    {
        Instance = this;
    }
    public void EnableBlockInput()
    {
        if (inputBlocker == null) { Debug.LogError("Forgot to assign an image to input blocker"); return; }
        inputBlocker.enabled = true;
    }

    public void DisableBlockInput()
    {
        if (inputBlocker == null) { Debug.LogError("Forgot to assign an image to input blocker"); return; }
        inputBlocker.enabled = false;
    }

    public bool IsBlocking()
    {
        return inputBlocker != null && inputBlocker.enabled;
    }

}
