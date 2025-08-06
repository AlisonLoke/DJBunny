using UnityEditorInternal;
using UnityEngine;
using TMPro;


public class MovesManager : MonoBehaviour
{
    public static MovesManager instance;
    [SerializeField] private GameObject movesCounterUI;
    //public int maxMoves = 5;
    private int currentMoves = 0;
    public TextMeshProUGUI movesText;
    public event System.Action onOutOfMoves;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    private void Update()
    {
        movesText.text = $"{GetRemainingMoves()}";
    }
    public void ResetMoves()
    {
        currentMoves = 0;
    }

    public void TrackMoves()
    {
        if(LevelManager.Instance != null && !LevelManager.Instance.useMoveLimit)
        {
            return;
        }
        currentMoves++;

        if (currentMoves >= LevelManager.Instance.maxMoves)
        {
            Debug.LogWarning("No more moves left");
            onOutOfMoves?.Invoke();
        }
    }

    public int GetRemainingMoves()
    {
        return Mathf.Max(0, LevelManager.Instance.maxMoves - currentMoves);
    }
}
