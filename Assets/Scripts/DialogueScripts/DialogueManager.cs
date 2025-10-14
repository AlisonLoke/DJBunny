using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    public TextMeshProUGUI DialogueText;
    public TextMeshProUGUI nameTag;
    public float textSpeed = 0.03f;
    public DialogueLine[] dialogueLines;
    public Animator animator;
    private int index;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        DialogueText.text = string.Empty;

    }

    public void StartDialogue()
    {
        animator.SetBool("IsOpen", true);
        index = 0;
        StartCoroutine(TypeLine());
    }
    private IEnumerator TypeLine()
    {
        nameTag.text = dialogueLines[index].characterName;
        SFXManager.instance.TriggerDialogueSFX(/*character*/);
        foreach (char character in dialogueLines[index].line.ToCharArray())
        {
            DialogueText.text += character;
            yield return new WaitForSeconds(textSpeed);
        }
        SFXManager.instance.StopDialogueSFXAtEnd();
    }
    public void InteractToNextLine()
    {
        if (DialogueText.text == dialogueLines[index].line)
        {
            NextLine();
        }
        else
        {
            StopAllCoroutines();
            DialogueText.text = dialogueLines[index].line;
            SFXManager.instance.StopDialogueSFXAtEnd();

        }
    }
    private void NextLine()
    {
        if (index < dialogueLines.Length - 1)
        {
            index++;
            DialogueText.text = string.Empty;
            dialogueLines[index].onLineStart?.Invoke();
            StartCoroutine(TypeLine());
        }
        else
        {

            animator.SetBool("IsOpen", false);
            InputBlocker.Instance.DisableBlockInput();

            Debug.Log("current music has stopped");
            if (LevelManager.Instance.isFailCutScene)
            {
                LevelManager.Instance.OnPlayerFail();
            }
            else if (LevelManager.Instance.isWinCutScene)
            {
                LevelManager.Instance.OnPlayerWin();
            }
            else
            {

                LevelManager.Instance.GetNextCutScene();
            }

        }
    }

    public void OnNodStart()
    {
        animator.SetBool("IsOpen", false);
    }
}
