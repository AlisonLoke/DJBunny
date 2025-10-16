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
    public enum TutorialStage { Start,Rotate,Placement,End}
    public TutorialStage currentStage = TutorialStage.Start;    
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
        Debug.Log($"LevelManager.Instance.Tutorial = {LevelManager.Instance.Tutorial}");
        Debug.Log($"Scene Build Index = {SceneManager.GetActiveScene().buildIndex}");
        if (LevelManager.Instance.Tutorial && SceneManager.GetActiveScene().buildIndex == 3)
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
            Debug.Log($"tutorialUI active state = {tutorialUI.activeSelf}");
            tutorialUI.SetActive(false);
        }
    }
   
    public void SwitchToRotateTutorial()
    {
        
        if (currentStage != TutorialStage.Start) return; // prevent skipping
        currentStage = TutorialStage.Rotate;
        StartCoroutine(SwitchTutorial());
    }
    public void SwitchToPlacementTutorial()
    {
        if (currentStage != TutorialStage.Rotate) return; // only valid after rotation
        currentStage = TutorialStage.Placement;
        StartCoroutine(SwitchAnimationDelay());
    
        StartCoroutine(SwitchTutorial());
        //StartCoroutine(SwitchText("Place The Block On The Bunny"));
    }
    public void SwitchToTutorialEnd()
    {
        //StartCoroutine(SwitchText("Use All The Blocks To Connect The Bunnies In A Single Line!"));
        if (currentStage != TutorialStage.Placement) return; // only valid after placement

        currentStage = TutorialStage.End;
        StartCoroutine(SwitchTutorial());
    }

    private IEnumerator SwitchAnimationDelay()
    {
        if(mouseAnimator == null) { yield break; }  
        yield return new WaitForSeconds(1f);
        mouseAnimator.SetTrigger("WithArrow");
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
        if(textAnimator == null) {yield break; }
        yield return new WaitForSeconds(1f);
        textAnimator.SetTrigger("Hide");
        yield return new WaitForSeconds(2f);

        ShowNextTutorialImage();
    }

 

}
