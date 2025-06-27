using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionSystem : MonoBehaviour
{
    public static ConnectionSystem instance;
    public List<EndCell> connectedEndCell = new List<EndCell>();
    public List<RectTransform> placedBlocks;
    public List<EndCell> endCells;

    [HideInInspector] public BlockSystem blockSystem;
    public static int currentLevelIndex = 1;
    public GridCell currentGridCell;
    private EndCell startConnectedEndCell;
    private EndCell finishConnectedEndCell;
    //stores a single path.
    private List<EndCell> currentPath = new List<EndCell>();
    //Contains multiple list of endCell object paths. A collection of paths
    private List<List<EndCell>> allPaths = new List<List<EndCell>>();
    public List<BlockUI> allBlockUIs = new List<BlockUI>();
    private List<BlockUI> previouslyPulsedBlocks = new List<BlockUI>();
    //private BlockUI blockUI;
    //private Image cellImage;
    private Tween currentLineFadeTween;

    public event System.Action<int> onValidPathCompleted;

    private const float PreviewPathPulseLength = 0.15f;
    private const float PathCompletePulseLength = 0.5f;

    [SerializeField] private UILineRenderer lineRenderer;
    [SerializeField] private RectTransform canvas;
    [SerializeField] private PathFinder pathFinder;

    [Tooltip("Disable for levels where not all blocks are required to complete the level")]
    [SerializeField] private bool requireAllBlocksUsed = true;

    private void Awake()
    {
        blockSystem = GetComponentInParent<BlockSystem>();
        //cellImage = GetComponent<Image>();
        //currentGridCell = blockSystem.SnapClosestGridCell(transform.position);
        instance = this;


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

        lineRenderer.thickness = 5f;
        lineRenderer.raycastTarget = false;

    }


    public void CheckConnectionsForAllEndCells()
    {

        currentPath.Clear();
        startConnectedEndCell = null;
        finishConnectedEndCell = null;
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
                endCell.ConnectedToStartAndFinish(endCell.currentGridCell);
                Debug.Log($"Found EndCell on start cell {endCell.name} at: ({endCell.currentGridCell.x}, {endCell.currentGridCell.y})");
            }

            if (pathFinder != null && pathFinder.IsFinishCell(endCell.currentGridCell))
            {
                finishConnectedEndCell = endCell;
                endCell.ConnectedToStartAndFinish(endCell.currentGridCell);
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
                //EndCell[] endCellsInBlock = placedBlockSystem.endCells;
                //allEndCells.AddRange(endCellsInBlock);

                foreach (EndCell endCell in placedBlockSystem.endCells)
                {
                    // Skip self
                    if (endCell == fromEndCell)
                        continue;

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





    private void FindLongestPath()
    {
        // Clear any previous paths
        ClearBlockPulses();
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

        if (allPaths.Count == 0)
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

        if (requireAllBlocksUsed && !AreAllBlockUsed(currentPath))
        {
            Debug.Log("Not all blocks are used in this Path");
            ClearConnectedLine();
            ClearBlockPulses();
            return;
        }
        Debug.Log("PATH COMPLETE");
        //UpdateConnectionLine();
        //load win scene
        //SceneManager.LoadScene("WinCutScene");
        //blockUI = startConnectedEndCell?.GetComponentInParent<BlockUI>();



        onValidPathCompleted?.Invoke(currentPath.Count);//invoke fancy name for trigger event gets triggered



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

        StartCoroutine(PulseCompletePath());
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
    public void PreviewCurrentPath()
    {
        currentPath.Clear();
        allPaths.Clear();
        if (startConnectedEndCell == null && finishConnectedEndCell == null)
        {
            return;
        }

        List<EndCell> workingPath = new List<EndCell>();

        EndCell cellToTrackPathFrom = !startConnectedEndCell ? finishConnectedEndCell : startConnectedEndCell;
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

        StartCoroutine(PulseCurrentPath(blockUIPath, Color.cyan, PreviewPathPulseLength));

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
                    UpdateConnectionLine();
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

        Debug.Log("Current path cells:");

        List<BlockUI> blockUisOriginal = GetAllBlockUIInPath();

        List<BlockUI> blockUisInPath = RemoveBlockUIDuplicatesInPath(blockUisOriginal);

        List<RectTransform> rectTransformsInPath = GetBlockUIRectTransformInPathSorted(blockUisInPath);

        List<Vector2> linePoints = GetWorldPosFromBlockTransforms(rectTransformsInPath);

        Debug.Log($"Setting {linePoints.Count} points to line renderer");
        if (linePoints.Count >= 2)
        {
            lineRenderer.points = linePoints.ToArray();
            lineRenderer.SetAllDirty(); // Force redraw

            //Move line renderer to render on top
            //lineRenderer.transform.SetAsLastSibling();
            Debug.Log($"Line renderer updated with {linePoints.Count} points");
        }
        else
        {
            Debug.LogWarning("Not enough valid points to draw line");
            ClearConnectedLine();
        }
    }

    /// <summary>
    /// Sorts RectTransforms in correct path order
    /// Start on one of the EndCells marked with onlyConnectToStartFinish
    /// Go to next Rect Transform in same Block UI
    /// Once Block UI out of Rect Transforms, look at next Block UI
    /// And so on
    /// When reaching last BlockUI, start with the EndCell not marked as onlyConnectToStartFinish
    /// End line on the Finish cell (EndCell.onlyConnectToStartFinish == true)
    /// </summary>
    private List<RectTransform> GetBlockUIRectTransformInPathSorted(List<BlockUI> blockUisInPath)
    {
        List<RectTransform> rectTransformsInPath = new();
        RectTransform lastCellInPreviousBlock = null;
        for (int index = 0; index < blockUisInPath.Count; index++)
        {
            BlockUI blockUi = blockUisInPath[index];
            List<RectTransform> rectTransformsInBlock = blockUi.GetBlockCellImagesRectTransforms();

            //TODO: refator method out
            // Handle First/Last block "faster" check
            bool isFirstBlock = index == 0;
            bool isLastBlock = index == blockUisInPath.Count - 1;
            EndCell endCell = rectTransformsInBlock[0].GetComponent<EndCell>();
            if (isFirstBlock && !endCell.onlyConnectToStartFinish
                || isLastBlock && endCell.onlyConnectToStartFinish)
            {
                rectTransformsInBlock.Reverse();
            }

            if (lastCellInPreviousBlock != null)
            {
                float distanceToFirstCellInBlock = Vector2.Distance(rectTransformsInBlock[0].transform.position, lastCellInPreviousBlock.transform.position);
                float distanceToLastCellInBlock = Vector2.Distance(rectTransformsInBlock[rectTransformsInBlock.Count - 1].transform.position, lastCellInPreviousBlock.transform.position);

                if (distanceToFirstCellInBlock > distanceToLastCellInBlock)
                {
                    rectTransformsInBlock.Reverse();
                }
            }

            rectTransformsInPath.AddRange(rectTransformsInBlock);
            lastCellInPreviousBlock = rectTransformsInBlock[rectTransformsInBlock.Count - 1];
        }

        return rectTransformsInPath;
    }

    private List<Vector2> GetWorldPosFromBlockTransforms(List<RectTransform> rectTransformsInPath)
    {
        List<Vector2> linePoints = new List<Vector2>();
        foreach (RectTransform rectTransform in rectTransformsInPath)
        {
            //Get world position of block's center
            Vector2 worldPos = rectTransform.TransformPoint(rectTransform.rect.center);


            // Convert world position to local position relative to the line renderer
            // null camera for Screen Space - Overlay
            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(lineRenderer.rectTransform, worldPos, null, out localPos);

            Debug.Log($"  - Cell at local position {localPos} (world: {worldPos})");
            linePoints.Add(localPos);
        }

        return linePoints;
    }

    private List<BlockUI> GetAllBlockUIInPath()
    {
        // Get all block Uis in current path
        //picks up all the blocks it sees
        List<BlockUI> blockUisOriginal = new();
        foreach (EndCell endCell in currentPath)
        {
            blockUisOriginal.Add(endCell.GetBlockUi());
        }

        return blockUisOriginal;
    }

    private List<BlockUI> RemoveBlockUIDuplicatesInPath(List<BlockUI> blockUisOriginal)
    {
        // remove duplicate BlockUis
        //if grabbed a blockui before that skip it
        HashSet<BlockUI> blockUisSeen = new();
        List<BlockUI> blockUisInPath = new();
        foreach (BlockUI blockUI in blockUisOriginal)
        {
            if (!blockUisSeen.Add(blockUI))
            {
                continue;
            }

            blockUisInPath.Add(blockUI);
        }

        return blockUisInPath;
    }
    private void FadeOutPathLine(float duration = 1.0f)
    {

        //a is alpha, naming follows rgba
        if (lineRenderer == null || lineRenderer.color.a <= 0f) return;

        currentLineFadeTween?.Kill();
        // DoTween.Alpha gradually changes alpha colour 
        currentLineFadeTween = DOTween.ToAlpha(GetLineRendererColour, SetLineRendererColour, 0f, duration).SetEase(Ease.InOutSine);


    }

    private Color GetLineRendererColour()
    {
        return lineRenderer.color;
    }
    private void SetLineRendererColour(Color newColour)
    {
        lineRenderer.color = newColour;
        lineRenderer.SetAllDirty(); //refreshes ui so new colour can be applied
    }
    private void ClearConnectedLine()
    {
        lineRenderer.points = new Vector2[0];
        lineRenderer.SetAllDirty();
    }


}
