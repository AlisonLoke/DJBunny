using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    public static PathFinder instance;
    [SerializeField] private GridData gridData;
    [SerializeField] private GridVisual gridVisual;

    private GridCell startCell;
    private GridCell finishCell;
    private GridCell doubleStartCell;
    private GridCell doubleFinishCell;
    private List<EndCell> pathEndCells = new List<EndCell>();


    private void Awake()
    {
        instance = this;

        // Find the start and end cells based on the GridData coordinates
        //Debug.Log("PathFinder Start was called!");
        gridVisual.OnGridGenerated += FindAllStartAndFinishCell;
        //gridVisual.OnGridGenerated += LogStartFinishCellStstus;
    }

    private void OnDestroy()
    {
        gridVisual.OnGridGenerated -= FindAllStartAndFinishCell;
    }

    private void FindAllStartAndFinishCell()
    {
        // Instead of searching through all cells, let's get the references directly from GridVisual
        GridCell[] allCells = gridVisual.cells;
        Debug.Log($"Finding start/finish among {allCells.Length} cells");

        // Get the coordinates from GridData
        Vector2Int startCoords = gridData.startCellCoordinates;
        Vector2Int endCoords = gridData.finishCellCoordinates;
        Vector2Int doubleStartCoords = gridData.doubleStartCellCoordinates;
        Vector2Int doubleFinishCoords = gridData.doubleFinishCellCoordinates;


        Debug.Log($"Looking for start at ({startCoords.x}, {startCoords.y}) and finish at ({endCoords.x}, {endCoords.y})");
        // Use the already marked cells from GridVisual if possible
        foreach (GridCell cell in allCells)
        {
            FindPrimaryConnectCells(ref startCoords, ref endCoords, cell);

            FindDoubleConnectCells(ref doubleStartCoords, ref doubleFinishCoords, cell);
        }

        if (startCell == null || finishCell == null && (doubleStartCell == null || doubleFinishCell == null))
        {
            Debug.LogError("Could not find start or end cell!");

        }
    }

    private void FindPrimaryConnectCells(ref Vector2Int startCoords, ref Vector2Int endCoords, GridCell cell)
    {

        // Check if this cell has a connect cell image active (indicating start/end)
        if (cell.x == startCoords.x && cell.y == startCoords.y)
        {
            startCell = cell;
            Debug.Log("Found start cell at: " + cell.x + ", " + cell.y);
        }

        if (cell.x == endCoords.x && cell.y == endCoords.y)
        {
            finishCell = cell;
            Debug.Log("Found end cell at: " + cell.x + ", " + cell.y);
        }
    }

    private void FindDoubleConnectCells(ref Vector2Int doubleStartCoords, ref Vector2Int doubleFinishCoords, GridCell cell)
    {
        if (gridData.enableDoubleConnectCell)
        {
            if (cell.x == doubleStartCoords.x && cell.y == doubleStartCoords.y)
            {
                doubleStartCell = cell;
                Debug.Log("Found double start cell at: " + cell.x + ", " + cell.y);
            }

            if (cell.x == doubleFinishCoords.x && cell.y == doubleFinishCoords.y)
            {
                doubleFinishCell = cell;
                Debug.Log("Found double finish cell at: " + cell.x + ", " + cell.y);

            }
        }
    }

    public bool IsStartCell(GridCell thisCell)
    {
        if (thisCell == null) return false;

        bool isPrimaryStart = IsPrimaryStartCell(thisCell);

        bool isDoubleStart = IsDoubleStartCell(thisCell);

        bool result = isPrimaryStart || isDoubleStart;

        Debug.Log($"Checking if cell ({thisCell?.x}, {thisCell?.y}) is start cell: {result}");
        return result;
    }
    public bool IsFinishCell(GridCell thisCell)
    {
        if (thisCell == null) return false;

        bool isPrimaryFinish = IsPrimaryFinishCell(thisCell);

        bool isDoubleFinish = IsDoubelFinishCell(thisCell);

        bool result = isPrimaryFinish || isDoubleFinish;

        Debug.Log($"Checking if cell ({thisCell?.x}, {thisCell?.y}) is finish cell: {result}");
        return result;
    }
   
    private bool IsPrimaryStartCell(GridCell thisCell)
    {
        return startCell != null && thisCell.x == startCell.x && thisCell.y == startCell.y;
    }

    private bool IsPrimaryFinishCell(GridCell thisCell)
    {
        return finishCell != null && thisCell.x == finishCell.x && thisCell.y == finishCell.y;
    }
  
    private bool IsDoubleStartCell(GridCell thisCell)
    {
        if (thisCell == null || doubleStartCell == null) return false;
        if (!gridData.enableDoubleConnectCell) return false;
        return doubleStartCell != null && thisCell.x == doubleStartCell.x && thisCell.y == doubleStartCell.y;
    }
    private bool IsDoubelFinishCell(GridCell thisCell)
    {
        if (thisCell == null || doubleFinishCell == null) return false;
        if (!gridData.enableDoubleConnectCell) return false;
        return doubleFinishCell != null && thisCell.x == doubleFinishCell.x && thisCell.y == doubleFinishCell.y;
    }




}
