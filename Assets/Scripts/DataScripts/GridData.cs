using UnityEngine;

[CreateAssetMenu(fileName = "GridData", menuName = "Scriptable Objects/GridData")]
public class GridData : ScriptableObject
{

    public int x = 6;
    public int y = 6;

    [SerializeField] private GridCell startCell;
    [SerializeField] private GridCell endCell;
    public Vector2Int startCellCoordinates = Vector2Int.zero;
    public Vector2Int finishCellCoordinates = Vector2Int.zero;


    public Vector2Int GetStartCellCoordinates()
    {
        return startCellCoordinates;
    }

    public Vector2Int GetEndCellCoordinates()
    {
        return finishCellCoordinates;
    }
}
