using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;
    [SerializeField] private GameObject tutorialUI;
    [SerializeField] Animator mouseAnimator;
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
}
