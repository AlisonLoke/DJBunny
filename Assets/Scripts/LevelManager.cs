using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public GridData gridData;
    [Header("LevelSettings")]
    public bool Tutorial = false;
    public bool useMoveLimit = false;
    public int maxMoves = 5;
    [SerializeField] private bool isLastPuzzle = false;
    [SerializeField] private bool isLastLevel = false;  
    // needs to be called after ConnectionSystem.Awake()
   
    [SerializeField] private float CutSceneTransitionTime = 1f;
    private EndCell[] allEndCells;
    public GridData GetGridData => gridData;

    private void Start()
    {
        Instance = this;
        //ConnectionManager.instance.onValidPathCompleted += CheckIfLevelComplete;
        ConnectionManager.instance.onAllConnectionsComplete += HandleLevelComplete;

        if (useMoveLimit && isLastLevel)
        {

            MovesManager.instance.onOutOfMoves += TriggerLose;
        }

        allEndCells = FindObjectsByType<EndCell>(FindObjectsSortMode.None);

        if (!useMoveLimit )
        {
            return;
        }
    }

    private void OnDestroy()
    {
        //ConnectionManager.instance.onValidPathCompleted -= CheckIfLevelComplete;
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
    //private void CheckIfLevelComplete(int endCellsOnPathCount)
    //{
    //    if (endCellsOnPathCount >= allEndCells.Length)
    //    {
    //        StartCoroutine(TriggerWin());
    //    }
    //    else
    //    {
    //        ShowNotAllBlocksUsedAffordance();
    //    }
    //}

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
       
        Debug.Log("GameOver! You ran out of moves");
        StartCoroutine(LoadFailScene());
    }

    private IEnumerator LoadFailScene()
    {
        SceneTransition.Instance.StartCutSceneSceneTransition();
        yield return new WaitForSeconds(CutSceneTransitionTime);
        SceneManager.LoadScene("FailDanceCutScene_Level05");
    }
}
