using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectionSystem : MonoBehaviour
{
    public static ConnectionSystem instance;

    public List<RectTransform> placedBlocks;
    public List<EndCell> endCells;
    public static int currentLevelIndex = 1;

    private EndCell startConnectedEndCell;
    private EndCell finishConnectedEndCell;
    //stores a single path.
    private List<EndCell> currentPath = new List<EndCell>();
    //Contains multiple list of endCell object paths. A collection of paths
    private List<List<EndCell>> allPaths = new List<List<EndCell>>();

    [SerializeField] private UILineRenderer lineRenderer;
    [SerializeField] private RectTransform canvas;
    [SerializeField] private PathFinder pathFinder;

    private void Awake()
    {
        instance = this;
        //Debug.Log("ConnectionSystem Awake called");
    }
    private void Start()
    {
        // Make sure line renderer is properly initialized
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<UILineRenderer>();
            if (lineRenderer == null)
            {
                Debug.LogError("Line renderer not assigned to ConnectionSystem!");
            }
        }

        lineRenderer.color = Color.yellow;
        lineRenderer.thickness = 5f;
        lineRenderer.raycastTarget = false;

    }

    //private void Update()
    //{
    //    UpdateConnectionLine();
    //}
    public void CheckConnectionsForAllEndCells()
    {
        currentPath.Clear();
        ClearConnectedLine();

        Debug.Log($"Checking connections for {endCells.Count} end cells");

        // Check each end cell's position and connections
        foreach (EndCell endCell in endCells)
        {
            // Make sure we have current grid cell info - THIS IS CRITICAL
            endCell.currentGridCell = endCell.blockSystem.SnapClosestGridCell(endCell.transform.position);

            if (endCell.currentGridCell == null)
            {
                Debug.Log($"EndCell {endCell.name} has no grid cell");
                continue;
            }

            //Debug.Log($"EndCell at grid position: {endCell.currentGridCell.x}, {endCell.currentGridCell.y}");

            // Check for connections to other EndCells
            endCell.CheckForEndCells();

            // Check if this EndCell is on a start or finish cell
            if (pathFinder != null && pathFinder.IsStartCell(endCell.currentGridCell))
            {
                startConnectedEndCell = endCell;
                endCell.MakeCellYellow(endCell.currentGridCell);




                Debug.Log($"Found EndCell on start cell {endCell.name} at: ({endCell.currentGridCell.x}, {endCell.currentGridCell.y})");
            }

            if (pathFinder != null && pathFinder.IsFinishCell(endCell.currentGridCell))
            {
                finishConnectedEndCell = endCell;
                endCell.MakeCellYellow(endCell.currentGridCell);
                Debug.Log($"Found EndCell on finish cell {endCell.name} at: ({endCell.currentGridCell.x}, {endCell.currentGridCell.y})");
            }
        }

        // Find path if we have both start and finish cells
        Debug.Log($"After checking all cells - Start cell found: {startConnectedEndCell != null}, Finish cell found: {finishConnectedEndCell != null}");
        if (startConnectedEndCell != null && finishConnectedEndCell != null)
        {
            Debug.Log("Both start and finish cells found, finding path...");
            FindLongestPath();
        }
        else
        {
            Debug.Log("Missing either start or finish cell, clearing path");
            currentPath.Clear();
            ClearConnectedLine();
        }
    }

    private void FindLongestPath()
    {
        // Clear any previous paths
        allPaths.Clear();
        currentPath.Clear();
        Debug.Log($"Finding path from {startConnectedEndCell.name} to {finishConnectedEndCell.name}");

        // Create a new path to work with
        List<EndCell> workingPath = new List<EndCell>();

        // Start recursive search
        FindAllPaths(startConnectedEndCell, finishConnectedEndCell, workingPath);
        Debug.Log($"Found {allPaths.Count} possible paths");

        //Filter out Paths that use only one side of a sister pair
        int beforeFilter = allPaths.Count;
        allPaths = allPaths.Where(IsPathValid).ToList();
        int removed = beforeFilter - allPaths.Count;

        Debug.Log($"Removed {removed} invalid partial-block paths. Remaining: {allPaths.Count}");

        if(allPaths.Count == 0)
        {
            allPaths.Clear();
            currentPath.Clear();
            Debug.Log("No valid path found. Don't trigger win scene");
            return;
        }

        // Find the longest path
        foreach (List<EndCell> path in allPaths)
        {
            if (path.Count > currentPath.Count)
            {
                currentPath = new List<EndCell>(path);
            }
        }

        Debug.Log($"Selected longest path with {currentPath.Count} cells");
        foreach (EndCell cell in currentPath)
        {
            Debug.Log($"Path point: {cell.name} at position {cell.transform.position}");
        }
        Debug.Log("PATH COMPLETE");
        UpdateConnectionLine();
        //load win scene
        //SceneManager.LoadScene("WinCutScene");

    }



    private void FindAllPaths(EndCell currentCell, EndCell targetCell, List<EndCell> path)
    {
        // Add current cell to path

        if (path.Contains(currentCell)) { return; }
        path.Add(currentCell);

        // If we reached the target, we found a complete path
        if (currentCell == targetCell)
        {
            // Save a copy of this path to our list of paths
            allPaths.Add(new List<EndCell>(path));
            return;// no need to keep going
        }
        // Check the sister (if it exists and isn't already visited)
        if (currentCell.sisterEndCell != null)
        {
           
            if (!path.Contains(currentCell.sisterEndCell)) 
            {
                FindAllPaths(currentCell.sisterEndCell, targetCell, new List<EndCell>(path));
                //Returning means that a sister end cell must be checked before further connections are checked
                //This prevents a scenario where just part of a block in used in a path
                return;
            }
        }

        //  Try all connected cells (if any)
        if (currentCell.connectedEndCell != null)
        {
            foreach (EndCell connected in currentCell.connectedEndCell)
            {
                FindAllPaths(connected, targetCell, new List<EndCell>(path));
            }
        }
    }
   
    private bool IsPathValid (List<EndCell> path)
    {
        if (path.Count < 3) return false; // Must go through at least one middle cell
        foreach (EndCell cell in path)
        {
            if(cell.sisterEndCell != null && !path.Contains(cell.sisterEndCell))
            {
                //sister exists but is not in the path so it is invalid
                return false;
               
            }
        }
        return true;
    }
   
    private void UpdateConnectionLine()
    {
        // If we don't have a path, don't draw anything
        if (currentPath.Count < 2)
        {
            ClearConnectedLine();
            return;
        }

        // Draw line connecting all cells in the path
        DrawPathLine();
    }



    private void DrawPathLine()
    {
        Debug.Log($"DrawPathLine called with {currentPath.Count} points");
        if (currentPath.Count < 2)
        {
            Debug.LogWarning("Not enough points to draw a line");
            ClearConnectedLine();
            return;
        }

        List<Vector2> linePoints = new List<Vector2>();
        Debug.Log("Current path cells:");
        foreach (EndCell thisCell in currentPath)
        {
            if (thisCell == null)
            {
                Debug.LogError("Null EndCell in path!");
                continue;
            }

            // Get the RectTransform and its position in UI space
            RectTransform rectTransform = thisCell.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                Vector2 anchoredPos = rectTransform.position - (canvas.position / (canvas.localScale.x / 1.85f));
                //Vector2 anchoredPos = rectTransform.position;
                Debug.Log($"  - Cell at {anchoredPos}");
                linePoints.Add(anchoredPos);
            }
            else
            {
                Debug.LogError($"EndCell {thisCell.name} has no RectTransform!");
            }
        }

        Debug.Log($"Setting {linePoints.Count} points to line renderer");
        if (linePoints.Count >= 2)
        {
            lineRenderer.points = linePoints.ToArray();
            lineRenderer.SetAllDirty(); // Force redraw
            Debug.Log($"Line renderer updated with {linePoints.Count} points");
        }
        else
        {
            Debug.LogWarning("Not enough valid points to draw line");
            ClearConnectedLine();
        }
    }

    private void ClearConnectedLine()
    {
        lineRenderer.points = new Vector2[0];
        lineRenderer.SetAllDirty();
    }

}
