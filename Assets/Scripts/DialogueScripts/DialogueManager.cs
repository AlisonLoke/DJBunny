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
        foreach (char character in dialogueLines[index].line.ToCharArray())
        {
            DialogueText.text += character;
            yield return new WaitForSeconds(textSpeed);
        }
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

        }
    }
    private void NextLine()
    {
        if (index < dialogueLines.Length - 1)
        {
            index++;
            DialogueText.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            //gameObject.SetActive(false);
            animator.SetBool("IsOpen", false);
            InputBlocker.Instance.DisableBlockInput();
            //SceneManager.LoadScene("Lvl01_St01");
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1 );
            Debug.Log("current music has stopped");
            //MusicManager.instance.StopCurrentLevelMusic();
            LevelManager.Instance.GetNextCutScene();

        }
    }
}
