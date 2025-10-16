using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    public TextMeshProUGUI DialogueText;
    public TextMeshProUGUI nameTag;
    private GameObject currentCharacterImage;
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
        SFXManager.instance.PlayOpenDialogue();
        animator.SetBool("IsOpen", true);
        index = 0;
        ShowCharacterImage();
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
        SFXManager.instance.PlayButtonUI();
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
            ShowCharacterImage();
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
    private void ShowCharacterImage()
    {
        // Hide the currently active one
        if (currentCharacterImage != null)
            currentCharacterImage.SetActive(false);

        // Get the image for this dialogue line
        GetNextImage();
    }

    private void GetNextImage()
    {
        GameObject nextImage = dialogueLines[index].characterImage;

        if (nextImage != null)
        {
            nextImage.SetActive(true);
            currentCharacterImage = nextImage; // Track it so we can hide it later
        }
        else
        {
            currentCharacterImage = null;
        }
    }
}
