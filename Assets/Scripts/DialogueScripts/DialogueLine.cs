using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DialogueLine 
{
    public string characterName;
    public GameObject characterImage;

    [TextArea(2,5)]
    public string line;

    public UnityEvent onLineStart;
}
