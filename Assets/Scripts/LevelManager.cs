using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// FULLY IN CONTROL OF MUSIC
/// To make a new track start playing on scene load, make sure IsNewLevel = true then set StartLevelMusic to desired track.
/// To make a new track start playing on Connection Path Completed, make sure IsLastPuzzle = true then set EndLevelMusic to desired track.
/// </summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public GridData gridData;

    [Header("LevelSettings")]
    public bool TriggerDialogueOnStart = false;
    public bool Tutorial = false;
    public bool useMoveLimit = false;
    public int maxMoves = 5;
    public bool isLastPuzzle = false;
    public bool isFailCutScene = false;
    public bool isWinCutScene = false;
    public bool IsNewLevel = false;
    [SerializeField] private bool isLastLevel = false;
    [SerializeField] private GameObject tryAgainCanvas;
    [Header("Level Music")]
    public AK.Wwise.Event StartLevelMusic;
    public AK.Wwise.Event EndLevelMusic = null;
    // needs to be called after ConnectionSystem.Awake()

    [SerializeField] private float CutSceneTransitionTime = 1f;
    private EndCell[] allEndCells;
    public GridData GetGridData => gridData;

    public delegate void OnLevelRestartHandler();
    public event OnLevelRestartHandler OnLevelRestart;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (MusicManager.instance != null && IsNewLevel)
        {
            Debug.Log("Resetting Music Manager for new level.");
            MusicManager.instance.ResetForNewLevel(StartLevelMusic, EndLevelMusic);
        }

        //Instance = this; // WHY DID I DO THAT?
        //ConnectionManager.instance.onValidPathCompleted += CheckIfLevelComplete;
        if (ConnectionManager.instance == null)
        {
            return;
        }
        ConnectionManager.instance.onAllConnectionsComplete += HandleLevelComplete;

        if (useMoveLimit)
        {

            MovesManager.instance.onOutOfMoves += TriggerLose;
        }

        allEndCells = FindObjectsByType<EndCell>(FindObjectsSortMode.None);

        if (!useMoveLimit && tryAgainCanvas == null)
        {
            return;
        }
     


    }

    private void OnDestroy()
    {
        //ConnectionManager.instance.onValidPathCompleted -= CheckIfLevelComplete;
        if (ConnectionManager.instance == null)
        {
            return;
        }
        ConnectionManager.instance.onAllConnectionsComplete -= HandleLevelComplete;
        if (useMoveLimit)
        {

            MovesManager.instance.onOutOfMoves -= TriggerLose;
        }
    }

    // the one who triggers (invoke) the event is called the publisher
    // the one who "subscribes" / AddListener to the event is called the subscriber
    // below is the Subscriber's "handling" of the event => when the event gets called/triggered, all subscribers' handlers get called
    private void HandleLevelComplete()
    {
        StartCoroutine(TriggerPuzzleWin());
    }


    private IEnumerator TriggerPuzzleWin()
    {
        Debug.Log("Triggering Win ");
        // do whatever a win would do

        if (isLastPuzzle)
        {
            SceneTransition.Instance.StartPuzzleToCutScene();
            yield return new WaitForSeconds(1f);
            GetNextCutScene();
        }
        else
        {
            yield return new WaitForSeconds(1.5f);
            SceneTransition.Instance.StartPuzzleTransition();
            yield return new WaitForSeconds(4f);
            GetNextSceneinBuildIndex();
        }


    }

    //TODO: Set of win sequence so that it gets full end track 
    private void ShowNotAllBlocksUsedAffordance()
    {
        // show something to the player to let them know that they need to use all blocks
    }

    //Get next scene build
    public void GetNextSceneinBuildIndex()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void GetNextCutScene()
    {
        StartCoroutine(StartCutSceneTransition(SceneManager.GetActiveScene().buildIndex + 1));
    }

    private IEnumerator StartCutSceneTransition(int levelIndex)
    {
        SceneTransition.Instance.StartCutSceneSceneTransition();
        yield return new WaitForSeconds(CutSceneTransitionTime);
        SceneManager.LoadScene(levelIndex);
    }
    private void TriggerLose()
    {
        if (isLastLevel)
        {
            Debug.Log("GameOver! You ran out of moves");
            StartCoroutine(LoadFailScene());
        }
        else
        {
            StartTryAgainTransition();
        }



    }

    private void StartTryAgainTransition()
    {
        OnLevelRestart?.Invoke();
        SceneTransition.Instance.StartTryAgainTransition();
        //MusicManager.instance.ResetForNewLevel(StartLevelMusic, EndLevelMusic);
        StartCoroutine(TryAgainDelay(1f));
    }
    private IEnumerator TryAgainDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }
    public void RestartCurrentLevel()
    {
        OnLevelRestart?.Invoke();
        OnLevelRestart = null;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }
    public void RestartLastLevel()
    {
        SceneManager.LoadScene("CS_Level05_st01");
    }
    private IEnumerator LoadFailScene()
    {
        SceneTransition.Instance.StartCutSceneSceneTransition();
        yield return new WaitForSeconds(CutSceneTransitionTime);
        SceneManager.LoadScene("FailDanceCutScene_Level05");
    }

    public void OnPlayerFail()
    {
        //Level Manager takes you back to Main Menu 
        SceneManager.LoadScene("ReplayMenu");
    }
    public void OnPlayerWin()
    {
        SceneManager.LoadScene("Credits");
    }
}
