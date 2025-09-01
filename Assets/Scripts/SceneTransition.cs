using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;
    [SerializeField] private Animator cutSceneTransition;

    private void Awake()
    {
        Instance = this;
    }
    public void StartCutSceneSceneTransition()
    {
        cutSceneTransition.SetTrigger("Start");
    }
}
