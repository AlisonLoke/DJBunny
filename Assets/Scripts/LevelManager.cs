using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
   public GridData gridData;
    // needs to be called after ConnectionSystem.Awake()
    private EndCell[] allEndCells;
    public GridData GetGridData => gridData;    

    private void Start()
    {
        Instance = this;
        ConnectionSystem.instance.onValidPathCompleted += CheckIfLevelComplete;

        allEndCells = FindObjectsByType<EndCell>(FindObjectsSortMode.None);

      
    }

    private void OnDestroy()
    {
        ConnectionSystem.instance.onValidPathCompleted -= CheckIfLevelComplete;
    }

    // the one who triggers (invoke) the event is called the publisher
    // the one who "subscribes" / AddListener to the event is called the subscriber
    // below is the Subscriber's "handling" of the event => when the event gets called/triggered, all subscribers' handlers get called
    private void CheckIfLevelComplete(int endCellsOnPathCount)
    {
        if (endCellsOnPathCount >= allEndCells.Length)
        {
            StartCoroutine(TriggerWin());
        }
        else
        {
            ShowNotAllBlocksUsedAffordance();
        }
    }

    private IEnumerator TriggerWin()
    {
        Debug.Log("Triggering Win ");
        // do whatever a win would do
        yield return new WaitForSeconds(4f); 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    //TODO: Set of win sequence so that it gets full end track 
    private void ShowNotAllBlocksUsedAffordance()
    {
        // show something to the player to let them know that they need to use all blocks
    }
}
