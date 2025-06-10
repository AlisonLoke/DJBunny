using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    // needs to be called after ConnectionSystem.Awake()
    private EndCell[] allEndCells;

    private void Start()
    {
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
            TriggerWin();
        }
        else
        {
            ShowNotAllBlocksUsedAffordance();
        }
    }

    private void TriggerWin()
    {
        Debug.Log("Triggering Win ");
        // do whatever a win would do
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void ShowNotAllBlocksUsedAffordance()
    {
        // show something to the player to let them know that they need to use all blocks
    }
}
