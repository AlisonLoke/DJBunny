using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;
    [SerializeField] private Animator cutSceneTransition;
    [SerializeField] private Animator puzzleTransition;
    [SerializeField] private Animator lastPuzzleTransition;
  

    private void Awake()
    {
     
        Instance = this;
    
    }

    public void StartCutSceneSceneTransition()
    {
        cutSceneTransition.SetTrigger("Start");
    }
    public void StartPuzzleTransition()
    {
        puzzleTransition.SetTrigger("PuzzleTransitionStart");
      

    }
    public void StartPuzzleToCutScene()
    {
        cutSceneTransition.SetTrigger("LastPuzzleFadeOut");
    }

}
