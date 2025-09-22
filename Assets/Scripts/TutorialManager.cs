using System.Collections;
using TMPro;
using UnityEngine;
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
        SwitchText("RotateTheBlock");
    }
    public void SwitchToPlacementTutorial()
    {
        StartCoroutine(SwitchAnimationDelay());
    }

    private IEnumerator SwitchAnimationDelay()
    {
        yield return new WaitForSeconds(1f);
        mouseAnimator.SetTrigger("WithArrow");
    }

    public void SwitchText(string newText)
    {
        tutorialText.text = newText;
        textAnimator.SetTrigger("Hide");

    }
}
