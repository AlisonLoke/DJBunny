using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    public static PathFinder instance;
    [SerializeField] private GridData gridData;
    [SerializeField] private GridVisual gridVisual;

    private GridCell startCell;
    private GridCell finishCell;
    private List<EndCell> pathEndCells = new List<EndCell>();

    private void Awake()
    {
        instance = this;

        // Find the start and end cells based on the GridData coordinates
        Debug.Log("PathFinder Start was called!");
        gridVisual.OnGridGenerated += FindStartAndFinishCell;
        gridVisual.OnGridGenerated += LogStartFinishCellStstus;
    }

    //private void Start()
    //{
    //    // Find the start and end cells based on the GridData coordinates
    //    Debug.Log("PathFinder Start was called!");
    //    gridVisual.OnGridGenerated += FindStartAndFinishCell;
    //    gridVisual.OnGridGenerated += LogStartFinishCellStstus;
    //}

    public void LogStartFinishCellStstus()
    {
        Debug.Log($"PathFinder - Start cell: {(startCell != null ? $"({startCell.x}, {startCell.y})" : "null")}");
        Debug.Log($"PathFinder - Finish cell: {(finishCell != null ? $"({finishCell.x}, {finishCell.y})" : "null")}");
        Debug.Log($"GridData coordinates - Start: {gridData.startCellCoordinates}, Finish: {gridData.finishCellCoordinates}");
    }
    private void FindStartAndFinishCell()
    {
        // Instead of searching through all cells, let's get the references directly from GridVisual
        GridCell[] allCells = gridVisual.cells;
        Debug.Log($"Finding start/finish among {allCells.Length} cells");
        
        // Get the coordinates from GridData
        Vector2Int startCoords = gridData.startCellCoordinates;
        Vector2Int endCoords = gridData.finishCellCoordinates;
        
        
        Debug.Log($"Looking for start at ({startCoords.x}, {startCoords.y}) and finish at ({endCoords.x}, {endCoords.y})");
        // Use the already marked cells from GridVisual if possible
        foreach (GridCell cell in allCells)
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

        if (startCell == null || finishCell == null)
        {
            Debug.LogError("Could not find start or end cell!");

        }
    }

   public bool IsStartCell(GridCell thisCell)
    {
        if (thisCell == null || startCell == null) return false;

        bool result = (thisCell != null && startCell != null && thisCell.x == startCell.x && thisCell.y == startCell.y);
        Debug.Log($"Checking if cell ({thisCell?.x}, {thisCell?.y}) is start cell ({startCell?.x}, {startCell?.y}): {result}");
        return result;
    }


    public bool IsFinishCell(GridCell thisCell)
    {
        if (thisCell == null || finishCell == null) return false;

        bool result = (thisCell != null && finishCell != null && thisCell.x == finishCell.x && thisCell.y == finishCell.y);
        Debug.Log($"Checking if cell ({thisCell?.x}, {thisCell?.y}) is finish cell ({finishCell?.x}, {finishCell?.y}): {result}");
        return result;
    }
}
