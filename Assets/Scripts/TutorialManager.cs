using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;
    [SerializeField] private GameObject tutorialUI;
    [SerializeField] Animator mouseAnimator;
    [SerializeField] private Animator textAnimator;
    [SerializeField] private TextMeshProUGUI tutorialText;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        if (LevelManager.Instance.Tutorial && SceneManager.GetActiveScene().buildIndex == 2)
        {
            tutorialUI.SetActive(true);
        }
        else
        {
            tutorialUI.SetActive(false);
        }
    }
    public void SwitchToRotateTutorial()
    {
        mouseAnimator.SetTrigger("RightClick");
        StartCoroutine(SwitchText("Rotate The Block"));
    }
    public void SwitchToPlacementTutorial()
    {
        StartCoroutine(SwitchAnimationDelay());
    
        StartCoroutine(SwitchText("Place The Block On The Bunny"));
    }
    public void SwitchToTutorialEnd()
    {
        StartCoroutine(SwitchText("Use All The Blocks To Connect The Bunnies In A Single Line!"));
    }

    private IEnumerator SwitchAnimationDelay()
    {
        yield return new WaitForSeconds(1f);
        mouseAnimator.SetTrigger("WithArrow");
    }

    private IEnumerator SwitchText(string newText)
    {
        yield return new WaitForSeconds(1f);
        textAnimator.SetTrigger("Hide");
        yield return new WaitForSeconds(1f);
        tutorialText.text = newText;

    }

}
