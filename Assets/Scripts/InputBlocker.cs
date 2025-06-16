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
        inputBlocker.enabled = true;
    }

    public void DisableBlockInput()
    {
        inputBlocker.enabled = false;
    }

}
