using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionSystem : MonoBehaviour
{
    //public static ConnectionSystem instance;
    public List<EndCell> connectedEndCell = new List<EndCell>();
    public List<RectTransform> placedBlocks;
    public List<EndCell> endCells;

    [HideInInspector] public BlockSystem blockSystem;
    public static int currentLevelIndex = 1;
    public GridCell currentGridCell;
    [SerializeField] private ConnectCellType connectCellType = ConnectCellType.Primary;
    public ConnectCellType ConnectionType => connectCellType;

    //Primary Connection variables
    private EndCell startConnectedEndCell;
    private EndCell finishConnectedEndCell;
    //stores a single path.
    private List<EndCell> currentPath = new List<EndCell>();

    public bool pathIsComplete = false;



    //Path Management
    //Contains multiple list of endCell object paths. A collection of paths
    private List<List<EndCell>> allPaths = new List<List<EndCell>>();
    public List<BlockUI> allBlockUIs = new List<BlockUI>();
    private List<BlockUI> previouslyPulsedBlocks = new List<BlockUI>();


    //private Tween currentLineFadeTween;

    public event System.Action<int> onValidPathCompleted;

    private const float PreviewPathPulseLength = 0.15f;
    private const float PathCompletePulseLength = 0.5f;

    //[SerializeField] private UILineRenderer lineRenderer;

    [SerializeField] private RectTransform canvas;
    [SerializeField] private PathFinder pathFinder;
    [SerializeField] private GridData gridData; //ref the gridata for dual connect feature

    //[Tooltip("Disable for levels where not all blocks are required to complete the level")]
    //[SerializeField] private bool requireAllBlocksUsed = true;

    private void Awake()
    {
        blockSystem = GetComponentInParent<BlockSystem>();
        //cellImage = GetComponent<Image>();
        //currentGridCell = blockSystem.SnapClosestGridCell(transform.position);
        //instance = this;


    }
 


    public void CheckConnectionsForAllEndCells()
    {
        if (pathIsComplete)
        {
            Debug.Log("Path is already completed.Skipping further checks");
            return;
        }
        ResetStartAndFinishConnections();

        Debug.Log($"Checking connections for {endCells.Count} end cells");

        // Check each end cell's position and connections
        foreach (EndCell endCell in endCells)
        {
            // Make sure we have current grid cell info - THIS IS CRITICAL
            endCell.currentGridCell = endCell.blockSystem.SnapClosestGridCell(endCell.transform.position);

            if (IsEndCellOffGrid(endCell)) continue;
            //Debug.Log($"EndCell at grid position: {endCell.currentGridCell.x}, {endCell.currentGridCell.y}");

            // Check for connections to other EndCells
            endCell.CheckForEndCells();

            CheckIfEndCellIsOnStartCell(endCell);
            CheckIfEndCellIsOnFinishCell(endCell);
        }

        EndCellFoundStartAndFinishCell();

    }
    public void ResetCompletedPath()
    {
        pathIsComplete = false;

        currentPath.Clear();
        ClearConnectedLine();
        ClearBlockPulses();
    }
    private void ResetStartAndFinishConnections()
    {
        currentPath.Clear();
        startConnectedEndCell = null;
        finishConnectedEndCell = null;
        ClearConnectedLine();
    }
    private bool IsEndCellOffGrid(EndCell endCell)
    {
        if (endCell.currentGridCell == null)
        {
            Debug.Log($"EndCell {endCell.name} has no grid cell");
            return true;
        }
        return false;
    }
    private void CheckIfEndCellIsOnStartCell(EndCell endCell)
    {
        // Check if this EndCell is on a start or finish cell
        if (pathFinder != null && pathFinder.IsStartCell(endCell.currentGridCell))
        {

            //Debug.Log($"EndCell {endCell.name} has ConnectCellType: {endCell.connectCellType}");
            if (connectCellType != endCell.currentGridCell.connectCellType)
            {
                return;
            }

            startConnectedEndCell = endCell;
            //connectedStartCells.Add(endCell);
            endCell.ConnectedToStartAndFinish(endCell.currentGridCell);
            Debug.Log($"Found EndCell on start cell {endCell.name} at: ({endCell.currentGridCell.x}, {endCell.currentGridCell.y})");
        }
    }
    private void CheckIfEndCellIsOnFinishCell(EndCell endCell)
    {
        if (pathFinder != null && pathFinder.IsFinishCell(endCell.currentGridCell))
        {
            Debug.Log($"EndCell {endCell.name} has ConnectCellType: {endCell.currentGridCell.connectCellType}");
            if (connectCellType != endCell.currentGridCell.connectCellType)
            {
                return;
            }

            finishConnectedEndCell = endCell;
            //connectedFinishCells.Add(endCell);
            endCell.ConnectedToStartAndFinish(endCell.currentGridCell);
            Debug.Log($"Found EndCell on finish cell {endCell.name} at: ({endCell.currentGridCell.x}, {endCell.currentGridCell.y})");
        }
    }

    private void EndCellFoundStartAndFinishCell()
    {
        if (pathIsComplete)
        {
            Debug.Log("Path is already completed.Skipping further checks");
            return;
        }

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
            ClearBlockPulses();
        }
    }



    public GridCell FindAdjacentEndCells(EndCell fromEndCell, Image blockCellImage, int x, int y)
    {


        foreach (RectTransform placedBlockRect in placedBlocks)
        {
            BlockSystem placedBlockSystem = placedBlockRect.GetComponent<BlockSystem>();
            if (placedBlockSystem != null && placedBlockSystem != fromEndCell.blockSystem) // Skip the current block
            {


                foreach (EndCell endCell in placedBlockSystem.endCells)
                {
                    // Skip self
                    if (endCell == fromEndCell)
                        continue;
                    if (endCell.blockSystem.connectCellType != fromEndCell.blockSystem.connectCellType)
                    {
                        continue;
                    }
                    // Skip if it's a restricted end cell
                    if (endCell.onlyConnectToStartFinish)
                        continue;
                    // Check if this end cell is in the target grid position
                    GridCell endCellGridCell = placedBlockSystem.SnapClosestGridCell(endCell.transform.position);
                    if (endCellGridCell == null || endCellGridCell.x != x || endCellGridCell.y != y)
                        continue;



                    // Connect the two end cells if not already connected
                    if (!endCell.connectedEndCell.Contains(fromEndCell))
                    {
                        endCell.connectedEndCell.Add(fromEndCell);
                    }



                    if (!fromEndCell.connectedEndCell.Contains(endCell))
                    {
                        fromEndCell.connectedEndCell.Add(endCell);
                    }

                    return endCellGridCell;
                }
            }
        }

        return null;
    }


    //TODO: Define paths as primary or secondary through the what connect cell type the endcells are on
    //TODO: Feed that data into the linerenderer so that it draws in defined paths
    //TODO: When two paths meet, prevent the linerenderer from merging


    private void FindLongestPath()
    {
        // Clear any previous paths

        ClearBlockPulses();
        allPaths.Clear();
        currentPath.Clear();
        Debug.Log($"Finding path from {startConnectedEndCell.name} to {finishConnectedEndCell.name}");

        // Create a new path to work with
        CreateWorkingPathAndStartSearchForPossiblePaths();

        //Filter out Paths that use only one side of a sister pair
        FilterOutInvalidPaths();

        if (allPaths.Count == 0)
        {
            NoValidPathFound();
            return;
        }

        // Select the longest path
        SelectLongestPath();

        //Debug method to Log longest path
        foreach (EndCell cell in currentPath)
        {
            Debug.Log($"Path point: {cell.name} at position {cell.transform.position}");
        }

        //if (!CheckAllBlockUsed(currentPath))
        //{
        //    return;
        //}



        Debug.Log("PATH COMPLETE");
        onValidPathCompleted?.Invoke(currentPath.Count);//invoke fancy name for trigger event gets triggered

        if (currentPath.Count == 0) return;
        //lastSuccessfulPath = new List<EndCell>(currentPath);
        PathComplete();

    }



 

    private void NoValidPathFound()
    {
        allPaths.Clear();

        currentPath.Clear();
        Debug.Log("No valid path found. Don't trigger win scene");
    }

    private void PathComplete()
    {
        List<BlockUI> blockUIPath = new List<BlockUI>();
        foreach (EndCell endCell in currentPath)
        {
            BlockUI ui = endCell.GetComponentInParent<BlockUI>();
            if (ui != null && !blockUIPath.Contains(ui))
            {
                blockUIPath.Add(ui);
            }
        }
        pathIsComplete = true;
       
    }

    private void SelectLongestPath()
    {
        //Find longest primary and Longest Secondary
        foreach (List<EndCell> path in allPaths)
        {
            if (path.Count > currentPath.Count)
            {
                currentPath = new List<EndCell>(path);
            }
        }

        Debug.Log($"Selected longest path with {currentPath.Count} cells");
    }

    private void FilterOutInvalidPaths()
    {
        int beforeFilter = allPaths.Count;
        allPaths = allPaths.Where(IsPathValid).ToList();
        int removed = beforeFilter - allPaths.Count;

        Debug.Log($"Removed {removed} invalid partial-block paths. Remaining: {allPaths.Count}");
    }

    private void CreateWorkingPathAndStartSearchForPossiblePaths()
    {
        List<EndCell> workingPath = new List<EndCell>();

        // Start recursive search
        FindAllPaths(startConnectedEndCell, finishConnectedEndCell, workingPath);
        Debug.Log($"Found {allPaths.Count} possible paths");
    }
    private bool AreAllBlockUsed(List<EndCell> path)
    {
        List<BlockUI> allBlocks = FindObjectsByType<BlockUI>(FindObjectsSortMode.None)
       .Where(b => b.gameObject.activeInHierarchy)
       .ToList();

        List<BlockUI> usedBlocks = new List<BlockUI>();

        foreach (EndCell endCell in path)
        {
            BlockUI blockUI = endCell.GetComponentInParent<BlockUI>();

            if (blockUI != null && !usedBlocks.Contains(blockUI))
            {
                usedBlocks.Add(blockUI);
            }
        }
        bool allUsed = usedBlocks.Count == allBlocks.Count;

        Debug.Log("Used blocks: " + usedBlocks.Count + ", Total placed blocks: " + allBlocks.Count + "  All used: " + allUsed);

        return allUsed;
    }

    public List<EndCell> GetCurrentPath()
    {
        return currentPath;
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

    private bool IsPathValid(List<EndCell> path)
    {
        if (path.Count < 3) return false; // Must go through at least one middle cell
        foreach (EndCell cell in path)
        {
            if (cell.sisterEndCell != null && !path.Contains(cell.sisterEndCell))
            {
                //sister exists but is not in the path so it is invalid
                return false;

            }
        }
        return true;
    }

    public void PreviewCurrentPath()
    {
        currentPath.Clear();
        allPaths.Clear();

        EndCell typeSpecificStartCell = FindStartCellOfType(this.connectCellType);
        EndCell typeSpecificFinishCell = FindFinishCellOfType(this.connectCellType);

        if (typeSpecificStartCell == null && typeSpecificFinishCell == null)
        {
            Debug.LogWarning($"No start or finish cells found for connection type {connectCellType}");
            return;
        }

     

        List<EndCell> workingPath = new List<EndCell>();

        EndCell cellToTrackPathFrom = typeSpecificStartCell != null ? typeSpecificStartCell : typeSpecificFinishCell;
        /* ternary operator - same as
         * if (startConnectedEndCell == null)
         * {
         *      cellToTrackPathFrom = finishConnectedEndCell;
         * }
         */

        TrackCurrentPath(cellToTrackPathFrom, workingPath);

        // Grab the longest reachable partial path
        foreach (List<EndCell> path in allPaths)
        {
            if (path.Count > currentPath.Count)
            {
                currentPath = new List<EndCell>(path);
            }
        }

        if (currentPath.Count == 0) return;

        List<BlockUI> blockUIPath = new List<BlockUI>();
        foreach (EndCell endCell in currentPath)
        {
            BlockUI ui = endCell.GetComponentInParent<BlockUI>();
            if (ui != null && !blockUIPath.Contains(ui))
            {
                blockUIPath.Add(ui);
            }
        }
        UpdateMusicLayers(blockUIPath);
        UpdateConnectionLine();
        StartCoroutine(PulseCurrentPath(blockUIPath, Color.cyan, PreviewPathPulseLength));

    }
    private void UpdateMusicLayers(List<BlockUI> activeBlocks)
    {
        foreach (BlockUI block in activeBlocks)
        {
            BlockData blockData = block.GetComponent<BlockSystem>().blockData;
            if (blockData != null && blockData.PlayInstrument != null)
            {

                MusicManager.instance.PlayInstruments(blockData.PlayInstrument);
            }
        }
    }
    private EndCell FindStartCellOfType(ConnectCellType targetType)
    {
        // Look through all endCells for one that matches the type and is a start cell
        foreach (EndCell endCell in endCells)
        {

            if (endCell.onlyConnectToStartFinish && PathFinder.instance.IsStartCell(endCell.currentGridCell) && endCell.currentGridCell.connectCellType == targetType)
            {

                return endCell;
            }
        }
        return null;
    }

    private EndCell FindFinishCellOfType(ConnectCellType targetType)
    {
        // Look through all endCells for one that matches the type and is a start cell
        foreach (EndCell endCell in endCells)
        {

            if (endCell.onlyConnectToStartFinish && PathFinder.instance.IsFinishCell(endCell.currentGridCell) && endCell.currentGridCell.connectCellType == targetType) // You'll need to implement this logic
            {
                return endCell;
            }
        }
        return null;
    }
    private IEnumerator PulseCurrentPath(List<BlockUI> blockUIs, Color pulseColour, float delayBetweenBlocks)
    {
        InputBlocker.Instance.EnableBlockInput();

        for (int i = 0; i < blockUIs.Count; i++)
        {
            BlockUI block = blockUIs[i];
            if (block == null) continue;
            List<EndCell> endCells = block.GetComponentsInChildren<EndCell>().ToList();

            //adding all endcells in specific block ui to current path
            foreach (EndCell endCell in endCells)
            {
                if (!currentPath.Contains(endCell))
                {
                    currentPath.Add(endCell);
                }
            }
            //Draw the line

            //make it visible again after fading out
            //lineRenderer.color = new Color(lineRenderer.color.r, lineRenderer.color.g, lineRenderer.color.b, 1f);
            //lineRenderer.SetAllDirty();

            List<Image> blockImages = block.GetBlockCellImages();
            for (int j = 0; j < blockImages.Count; j++)
            {
                Image img = blockImages[j];
                if (img == null) continue;

                Color originalColor = img.color;
                img.DOColor(pulseColour, 0.25f).OnComplete(() =>
                {
                    img.DOColor(originalColor, 0.25f);
                });
            }

            yield return new WaitForSeconds(delayBetweenBlocks);
            //FadeOutPathLine(0.4f);
        }

        InputBlocker.Instance.DisableBlockInput();
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(HightlightCurrentPath(blockUIs, Color.yellow, PreviewPathPulseLength));

    }

    private void TrackCurrentPath(EndCell currentCell, List<EndCell> path)
    {
        if (path.Contains(currentCell)) return;

        path.Add(currentCell);

        // Save this reachable path as a potential partial path
        allPaths.Add(new List<EndCell>(path));

        // First, follow sister (if not visited)
        if (currentCell.sisterEndCell != null && !path.Contains(currentCell.sisterEndCell))
        {
            TrackCurrentPath(currentCell.sisterEndCell, new List<EndCell>(path));
            return; // prioritize full block connections
        }

        if (currentCell.connectedEndCell != null)
        {
            foreach (EndCell connected in currentCell.connectedEndCell)
            {
                TrackCurrentPath(connected, new List<EndCell>(path));
            }
        }
    }

    private IEnumerator HightlightCurrentPath(List<BlockUI> blockUIs, Color highlightColour, float delayBetweenBlocks)
    {
        currentPath.Clear();
        allPaths.Clear();
        for (int i = 0; i < blockUIs.Count; i++)
        {

            BlockUI blockUI = blockUIs[i];
            if (blockUI == null) continue;



            //visual highlight
            List<Image> blockImages = blockUI.GetBlockCellImages();
            foreach (Image image in blockImages)
            {
                if (image != null)
                {

                    image.color = highlightColour;
                }
            }
            yield return new WaitForSeconds(delayBetweenBlocks);
        }


    }
    private IEnumerator PulseCompletePath()
    {
        currentPath.Clear();
        allPaths.Clear();
        if (startConnectedEndCell == null || finishConnectedEndCell == null)
        {
            yield break;
        }

        List<EndCell> workingPath = new List<EndCell>();

        FindAllPaths(startConnectedEndCell, finishConnectedEndCell, workingPath);



        // Grab the longest reachable partial path
        foreach (List<EndCell> path in allPaths)
        {
            if (path.Count > currentPath.Count)
            {
                currentPath = new List<EndCell>(path);
            }
        }

        if (currentPath.Count == 0) yield break;

        List<BlockUI> blockUIPath = new List<BlockUI>();
        foreach (EndCell endCell in currentPath)
        {
            BlockUI ui = endCell.GetComponentInParent<BlockUI>();
            if (ui != null && !blockUIPath.Contains(ui))
            {
                blockUIPath.Add(ui);
            }
        }

        yield return new WaitForSeconds(blockUIPath.Count * PreviewPathPulseLength + 0.01f);

        ClearBlockPulses();
        StartCoroutine(PulseCompletePath(blockUIPath, Color.green, PathCompletePulseLength));
    }

    private IEnumerator PulseCompletePath(List<BlockUI> blockUIs, Color pulseColour, float delayBetweenBlocks)
    {
        InputBlocker.Instance.EnableBlockInput();

        for (int i = 0; i < blockUIs.Count; i++)
        {
            BlockUI block = blockUIs[i];
            if (block == null) continue;

            List<Image> blockImages = block.GetBlockCellImages();

            for (int j = 0; j < blockImages.Count; j++)
            {
                Image img = blockImages[j];
                if (img == null) continue;
                Color originalColor = img.color;
                img.DOColor(pulseColour, 0.25f);
            }
            previouslyPulsedBlocks.Add(block);

            yield return new WaitForSeconds(delayBetweenBlocks);
        }

        InputBlocker.Instance.DisableBlockInput();
    }


    public void ClearBlockPulses()
    {
        foreach (BlockUI blockUI in previouslyPulsedBlocks)
        {
            if (blockUI == null) continue;

            blockUI.ResetToOriginalColours();
        }
        previouslyPulsedBlocks.Clear();
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

            ConnectionManager.instance.HandlePathCleared(connectCellType);
            return;
        }
        RectTransform targetLineRendererRect = ConnectionManager.instance.GetLineRendererByType(connectCellType)?.rectTransform;

        if (targetLineRendererRect == null)
        {
            Debug.LogWarning("Line renderer not found for connection type");
            return;
        }


        List<Vector2> linePoints = GetLinePointsFromCurrentPath( targetLineRendererRect);


        Debug.Log($"Setting {linePoints.Count} points to line renderer");
        if (linePoints.Count >= 2)
        {
            //SetLinePoints(linePoints, connectCellType);
            ConnectionManager.instance.HandlePathUpdate(connectCellType, linePoints);
        }
        else
        {
            //Debug.LogWarning("Not enough valid points to draw line");
            //ClearConnectedLine();
            ConnectionManager.instance.HandlePathCleared(connectCellType);
        }
    }
    /// <summary>
    /// Converts currentPath (from pathfinding) into line renderer points.
    /// Expands each EndCell into all cells within its block, following the path direction.
    /// MUST start and end on EndCells with onlyConnectToStartFinish = true
    /// </summary>
    private List<Vector2> GetLinePointsFromCurrentPath(RectTransform lineRendererRect)
    {
        List<Vector2> linePoints = new List<Vector2>();

        if (currentPath.Count < 2) { return linePoints; }
        // Verify the path starts and ends on connect cells
        EndCell firstEndCell = currentPath[0];
        EndCell lastEndCell = currentPath[currentPath.Count - 1];

        if (!firstEndCell.onlyConnectToStartFinish)
        {
            Debug.LogWarning($"Path doesn't start on a connect cell! Starting on {firstEndCell.name}");
        }

        if (!lastEndCell.onlyConnectToStartFinish)
        {
            Debug.LogWarning($"Path doesn't end on a connect cell! Ending on {lastEndCell.name}");
        }

        BlockUI previousBlock = null;


        for (int i = 0; i < currentPath.Count; i++)
        {
            EndCell currentEndCell = currentPath[i];
            BlockUI currentBlock = currentEndCell.GetBlockUi();

            // When encountering a new block, add all its cells
            if (currentBlock != previousBlock)
            {
                List<RectTransform> blockCells = currentBlock.GetBlockCellImagesRectTransforms();

                // Find which EndCells from this block are in the path
                List<EndCell> blockEndCellsInPath = GetBlockEndCellsInPath(currentBlock, i);


                // Determine direction: must go from first to last EndCell in path for this block
                bool shouldReverse = ShouldReverseBlockForConnectCells(blockCells, blockEndCellsInPath);

                if (shouldReverse)
                {
                    blockCells.Reverse();
                }

                // Add all cells in this block to the line
                foreach (RectTransform cellRect in blockCells)
                {
                    Vector2 localPos = ConvertToLineRendererSpace(cellRect, lineRendererRect);
                    linePoints.Add(localPos);
                }

                previousBlock = currentBlock;
            }
 
        }
        return linePoints;
    }

    /// <summary>
    /// Gets all EndCells from a specific block that appear in currentPath, in path order
    /// </summary>
    private List<EndCell> GetBlockEndCellsInPath(BlockUI block, int startIndex)
    {
        List<EndCell> endCellsInPath = new List<EndCell>();

        // Collect all EndCells from this block that are in the path
        for (int i = startIndex; i < currentPath.Count; i++)
        {
            EndCell endCell = currentPath[i];
            if (endCell.GetBlockUi() == block)
            {
                endCellsInPath.Add(endCell);
            }
            else
            {
                // Moved to a different block
                break;
            }
        }

        return endCellsInPath;
    }

    /// <summary>
    /// Determines if we should reverse the block's cell order.
    /// The block MUST be oriented so that:
    /// 1. If the first EndCell in path has onlyConnectToStartFinish=true, line starts there
    /// 2. If the last EndCell in path has onlyConnectToStartFinish=true, line ends there
    /// 3. Line flows from first to last EndCell in the path
    /// </summary>
    private bool ShouldReverseBlockForConnectCells(List<RectTransform> blockCells, List<EndCell> endCellsInPath)
    {
        if (blockCells.Count == 0 || endCellsInPath.Count == 0)
            return false;

        // Get the first and last EndCells from this block in the path
        EndCell firstEndCellInPath = endCellsInPath[0];
        EndCell lastEndCellInPath = endCellsInPath[endCellsInPath.Count - 1];

        // Find their indices in the block's cell list
        RectTransform firstRect = firstEndCellInPath.GetComponent<RectTransform>();
        RectTransform lastRect = lastEndCellInPath.GetComponent<RectTransform>();

        int firstIndex = blockCells.IndexOf(firstRect);
        int lastIndex = blockCells.IndexOf(lastRect);

        if (firstIndex == -1)
        {
            Debug.LogWarning($"First EndCell {firstEndCellInPath.name} not found in block cells");
            return false;
        }

        // If only one EndCell from this block is in the path
        if (firstEndCellInPath == lastEndCellInPath)
        {
            // Check if this EndCell is a connect cell
            if (firstEndCellInPath.onlyConnectToStartFinish)
            {
                // Determine if this is the start or end of the entire path
                bool isPathStart = (currentPath.IndexOf(firstEndCellInPath) == 0);
                bool isPathEnd = (currentPath.IndexOf(firstEndCellInPath) == currentPath.Count - 1);

                if (isPathStart)
                {
                    // This connect cell is the START - we want to begin from it
                    // If it's at the end of the block list, reverse
                    return firstIndex >= blockCells.Count / 2;
                }
                else if (isPathEnd)
                {
                    // This connect cell is the END - we want to finish on it
                    // If it's at the beginning of the block list, reverse
                    return firstIndex < blockCells.Count / 2;
                }
            }

            // Default fallback
            return firstIndex >= blockCells.Count / 2;
        }

        // Multiple EndCells in path - use entry and exit logic
        if (lastIndex == -1)
        {
            Debug.LogWarning($"Last EndCell {lastEndCellInPath.name} not found in block cells");
            return false;
        }

        // Path flows from first to last EndCell
        // If first comes AFTER last in the block's cell order, reverse
        bool shouldReverse = firstIndex > lastIndex;

        Debug.Log($"Block {endCellsInPath[0].GetBlockUi().name}: " +
                  $"first={firstEndCellInPath.name}(idx:{firstIndex}, connect:{firstEndCellInPath.onlyConnectToStartFinish}), " +
                  $"last={lastEndCellInPath.name}(idx:{lastIndex}, connect:{lastEndCellInPath.onlyConnectToStartFinish}), " +
                  $"shouldReverse={shouldReverse}");

        return shouldReverse;
    }





    private Vector2 ConvertToLineRendererSpace(RectTransform cellRect, RectTransform lineRendererRect)
    {
        Vector2 worldPos = cellRect.TransformPoint(cellRect.rect.center);
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            lineRendererRect,
            worldPos,
            null,
            out localPos
        );
        return localPos;
    }
   

    private void ClearConnectedLine()
    {
        //lineRenderer.points = new Vector2[0];
        //lineRenderer.SetAllDirty();
        ConnectionManager.instance.HandlePathCleared(connectCellType);
    }

    public void ShowPathComplete()
    {
        StartCoroutine(PulseCompletePath());
        if (LevelManager.Instance.isLastPuzzle)
        {
            //Play end music
            Debug.Log("ENDING LEVEL 01 MUSIC");
            LevelManager.Instance.EndLevelMusic.Post(gameObject);
        }

        SFXManager.instance.PlayCompletion.Post(gameObject);
    }

  

    public void MutePlacedBlocksForRestart()
    {
        foreach(RectTransform thisBlock in placedBlocks)
        {
            thisBlock.GetComponent<BlockSystem>().RestartLevel();
        }
    }


}




