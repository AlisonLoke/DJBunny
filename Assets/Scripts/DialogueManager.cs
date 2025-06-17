using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;
    private int index;

    private void Start()
    {
        textComponent.text = string.Empty;
        StartDialogue();
    }
    private void Update()
    {
       if (Mouse.current.leftButton.wasPressedThisFrame) // isPressed fires every frame while button is held not press and released so use wasPressedThisFrame
        {
            if(textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];  
            }
        }
    }
    private void StartDialogue()
    {
       index = 0;
        StartCoroutine(TypeLine());
    }
    private IEnumerator TypeLine()
    {
        foreach(char character in lines[index].ToCharArray())
        {
            textComponent.text += character;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    private void NextLine()
    {
        if(index < lines.Length - 1 )
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine (TypeLine());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
