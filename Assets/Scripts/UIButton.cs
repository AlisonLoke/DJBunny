using UnityEngine;

public class UIButton : MonoBehaviour
{
    [SerializeField] private Animator buttonAnimator;


    public void ButtonAnimator()
    {
        buttonAnimator.SetTrigger("OnClick");
    }
}
