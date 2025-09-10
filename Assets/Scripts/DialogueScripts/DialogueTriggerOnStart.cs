using UnityEngine;

public class DialogueTriggerOnStart : MonoBehaviour
{
    [SerializeField] private DialogueLine[] dialogueLines;
   
    void Start()
    {
        DialogueManager.instance.dialogueLines = dialogueLines;
        DialogueManager.instance.StartDialogue();
        InputBlocker.Instance.EnableBlockInput();
    }
}

//public void TriggerNextLine()
//    {
//        DialogueManager.instance.dialogueLines = dialogueLines;
//        DialogueManager.instance.InteractToNextLine();
//    }
//}
