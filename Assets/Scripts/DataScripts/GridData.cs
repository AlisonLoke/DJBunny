using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;



[CreateAssetMenu(fileName = "GridData", menuName = "Scriptable Objects/GridData")]
public class GridData : ScriptableObject
{

    public int x = 6;
    public int y = 6;
    
    [Header("Close Grid Space")]
    public List<Vector2Int> closedGridSpace;

    [Header("Start and Finish Cell")]
    public Vector2Int startCellCoordinates = Vector2Int.zero;
    public Vector2Int finishCellCoordinates = Vector2Int.zero;
    [Header("Double Start and Finish Cell")]
    public Vector2Int doubleStartCellCoordinates = Vector2Int.zero;
    public Vector2Int doubleFinishCellCoordinates = Vector2Int.zero;
    public bool enableDoubleConnectCell = false;    


    public List<Vector2Int> GetClosedGridSpaceCoordinates()
    {
        return closedGridSpace;
    }
    public Vector2Int GetStartCellCoordinates()
    {
        return startCellCoordinates;
    }

    public Vector2Int GetEndCellCoordinates()
    {
        return finishCellCoordinates;
    }
}
