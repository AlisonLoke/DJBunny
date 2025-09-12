using UnityEngine;

public class DialogueAnimationEvent : MonoBehaviour
{
    public void OnNodFinish()
    {
        DialogueManager.instance.animator.SetBool("IsOpen",true);

        DialogueManager.instance.InteractToNextLine();
    }

}
