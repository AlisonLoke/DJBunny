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
    }

    private void Start()
    {
        // Find the start and end cells based on the GridData coordinates
        Debug.Log("FindStartAndFinishCell was called!");
        gridVisual.OnGridGenerated += FindStartAndFinishCell;
    }

    private void FindStartAndFinishCell()
    {
        // Instead of searching through all cells, let's get the references directly from GridVisual
        GridCell[] allCells = gridVisual.cells;

        // Get the coordinates from GridData
        Vector2Int startCoords = gridData.startCellCoordinates;
        Vector2Int endCoords = gridData.finishCellCoordinates;

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
}
