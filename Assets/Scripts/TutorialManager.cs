using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;
    [SerializeField] private GameObject tutorialUI;
    [SerializeField] Animator mouseAnimator;
    [SerializeField] private Animator textAnimator;
    [SerializeField] private List<GameObject> tutorialImage;
    private int currentImageIndex = 0;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        if (LevelManager.Instance.Tutorial && SceneManager.GetActiveScene().buildIndex == 2)
        {
            tutorialUI.SetActive(true);

            for (int i = 0; i < tutorialImage.Count; i++)
            {
                tutorialImage[i].SetActive(i == 0);
            }
            currentImageIndex = 0;
        }
        else
        {
            tutorialUI.SetActive(false);
        }
    }
    public void SwitchToRotateTutorial()
    {
        //mouseAnimator.SetTrigger("RightClick");
        //textAnimator.SetTrigger("Hide");
        //StartCoroutine(SwitchText("Rotate The Block"));
        StartCoroutine(SwitchTutorial());
    }
    public void SwitchToPlacementTutorial()
    {
        StartCoroutine(SwitchAnimationDelay());
        StartCoroutine(SwitchTutorial());
        //StartCoroutine(SwitchText("Place The Block On The Bunny"));
    }
    public void SwitchToTutorialEnd()
    {
        //StartCoroutine(SwitchText("Use All The Blocks To Connect The Bunnies In A Single Line!"));
        StartCoroutine(SwitchTutorial());
    }

    private IEnumerator SwitchAnimationDelay()
    {
        yield return new WaitForSeconds(1f);
        mouseAnimator.SetTrigger("WithArrow");
    }
    public void OnHideComplete()
    {
        ShowNextTutorialImage();
    }
    private void ShowNextTutorialImage()
    {
        Debug.Log($"Switching tutorial image. CurrentIndex = {currentImageIndex}");
        if (currentImageIndex < tutorialImage.Count)
        {
            Debug.Log($"Deactivating {tutorialImage[currentImageIndex].name}");
            tutorialImage[currentImageIndex].SetActive(false);
        }

        currentImageIndex++;

        if(currentImageIndex < tutorialImage.Count)
        {
            Debug.Log($"Activating {tutorialImage[currentImageIndex].name}");
            tutorialImage[currentImageIndex].SetActive(true);
          //trigger show animation on next image
            Animator animator = tutorialImage[currentImageIndex].GetComponent<Animator>();  
            if(animator != null)
            {
                animator.SetTrigger("Show");
            }
        }
        else
        {
            Debug.Log("End Tutorial");
            tutorialUI.SetActive(false); 
        }
    }
    private IEnumerator SwitchTutorial()
    {
        yield return new WaitForSeconds(1f);
        textAnimator.SetTrigger("Hide");
        yield return new WaitForSeconds(1f);

        ShowNextTutorialImage();
    }

}
