using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BlockSystem : MonoBehaviour, IPointerClickHandler
{
    private static BlockSystem selectedBlock = null;
    private Vector3 blockOriginPos;
    private GridCell gridCell;
    private GridVisual gridVisual;
    private float timeSinceLastRotation = Mathf.Infinity;
    private float timeSinceLastHover = Mathf.Infinity;
    private bool isSnappedToGrid = false;
    private bool isFollowingMouse = false;
    private GameObject audioObject;
    private Image cellImage;
    private BlockUI blockUI;
    private bool isHovering = false;
    private bool isAnimatingHover = false;
    private Vector2 originalAnchoredPos;
    public Quaternion originalRotation;
    public ConnectCellType connectCellType => connectType;


    [SerializeField] private ConnectCellType connectType;
    [SerializeField] private RectTransform blockParentRect;
    [SerializeField] private RectTransform[] blockRectransforms;
    [SerializeField] private BlockData blockData;
    [SerializeField] private float delayBetweenRotations = 1f;
    [SerializeField] private float delaybetweenhovers = 1f;

    public RectTransform gridParent;
    public EndCell[] endCells;


    private void Start()
    {
        cellImage = GetComponent<Image>();
        gridVisual = gridParent.GetComponent<GridVisual>();
        endCells = GetComponentsInChildren<EndCell>();
        blockUI = GetComponent<BlockUI>();
        originalAnchoredPos = blockParentRect.anchoredPosition;
        originalRotation = blockParentRect.rotation;
        blockOriginPos = blockParentRect.position;
        foreach (EndCell cell in endCells)
        {
            cell.Initialise();
        }

    }

    private void Update()
    {

        if (timeSinceLastRotation < delayBetweenRotations)
        {
            timeSinceLastRotation += Time.deltaTime;
            return;
        }
        if(timeSinceLastHover < delaybetweenhovers)
        {
            timeSinceLastHover += Time.deltaTime;   
            return; 
        }

        // Follow mouse if selected
        if (isFollowingMouse)
        {
            Vector2 pointerPosition = Mouse.current.position.ReadValue();
            blockParentRect.position = pointerPosition;
            HighlightCells();
        }

        if (!isFollowingMouse && !isSnappedToGrid)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            bool pointerOver = IsMouseOverEntireBlock(mousePos);

            if (pointerOver && !isHovering)
            {
                if(timeSinceLastHover < delaybetweenhovers)
                {
                    return;
                }
                isHovering = true;
                StartHoverAnimation();
                timeSinceLastHover = 0;
            }
            else if (!pointerOver && isHovering)
            {

                isHovering = false;
                EndHoverAnimation();
            }

        }

        if (!isFollowingMouse && !isSnappedToGrid && !isHovering && !isAnimatingHover)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            bool pointerOver = IsMouseOverEntireBlock(mousePos);
            bool blockOriginPos = Vector2.Distance(blockParentRect.anchoredPosition, originalAnchoredPos) > 0.1f;

            if (!pointerOver && blockOriginPos)
            {
                // Force return to original position if we're somehow stuck
                blockParentRect.anchoredPosition = originalAnchoredPos;
            }
        }
    }

    private bool IsMouseOverEntireBlock(Vector2 mousePos)
    {
        // Check if mouse is over the parent rect
        if (RectTransformUtility.RectangleContainsScreenPoint(blockParentRect, mousePos))
        {
            //Debug.Log("Mouse is over the block");
            return true;
        }

        // Check if mouse is over any of the child block rectangles
        foreach (RectTransform blockRect in blockRectransforms)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(blockRect, mousePos))
            {
                return true;
            }
        }

        return false;
    }

    private void StartHoverAnimation()
    {
        if (isAnimatingHover) { return; }

        isAnimatingHover = true;
        blockParentRect.DOKill();
        blockParentRect.DOAnchorPos(originalAnchoredPos + new Vector2(0, 20f), 0.1f).SetEase(Ease.OutQuad)
        .OnComplete(() =>
        {
            isAnimatingHover = false;
        });
    }


    private void EndHoverAnimation()
    {
        if (isAnimatingHover) return;
        isAnimatingHover = true;
        blockParentRect.DOKill();
        // Animate scale back to normal
        blockParentRect.DOAnchorPos(originalAnchoredPos, 0.1f).SetEase(Ease.OutQuad)
        .OnComplete(() =>
        {
            isAnimatingHover = false;
        });
    }
    private void ResetHoverAnimation()
    {
        blockParentRect.DOKill(); // stop ongoing animations
        blockParentRect.anchoredPosition = originalAnchoredPos;
        isHovering = false;
        isAnimatingHover = false;
    }


    private void ClearEndCells()
    {
        foreach (EndCell cell in endCells)
        {
            cell.ClearConnections();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        HandleLeftClickOnBlock(eventData);
        HandleRightClickOnBlock(eventData);
    }

    private void HandleLeftClickOnBlock(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }


        if (!isFollowingMouse)
        {
            BeginPickUp();
            //audioObject = AudioManager.instance.Play(blockData.PlayInstrument);
            //Play the mute here
            //audioObject = AudioManager.instance.Play(blockData.MuteInstrument);
        }
        else
        {
            DropBlock();
        }
    }

    private void HandleRightClickOnBlock(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
        {
            return;
        }

        if (isSnappedToGrid)
        {

            RemoveFromGrid();
            ResetBlockToOrigin();



            return;
        }

        if (isFollowingMouse)
        {
            if (timeSinceLastRotation < delayBetweenRotations)
            {
                return;
            }

            RotateBlock();
            timeSinceLastRotation = 0f;
        }
    }

    private void DropBlock()
    {
        isFollowingMouse = false;
        selectedBlock = null;
        SFXManager.instance.PlayDrop.Post(gameObject);
        if (IsAnyBlockOutsideGrid())
        {
            ResetBlockToOrigin();
            UnhighlightAllCells();
            //AudioManager.instance.StopMusic();
            return;
        }

        //blockOriginPos = blockParentRect.position;
        //originalRotation = blockParentRect.rotation;

        GridCell snapClosestGridCell = SnapClosestGridCell(blockParentRect.position);
        FinalBlockPlacement(snapClosestGridCell);


        ConnectionManager.instance.PreviewCurrentPath();
        ConnectionManager.instance.CheckPathsAreCompleted();

        Debug.Log("PLAYING MUSIC");


        //AudioManager.instance.QueueMusic(blockData.Instruments);
        //if (audioObject != null)
        //{
        //    AudioManager.instance.StopMusic();
        //    audioObject = null;
        //}

        //audioObject = AudioManager.instance.Play(blockData.PlayInstrument);
        MusicManager.instance.PlayInstruments(blockData.PlayInstrument);
    }


    private void BeginPickUp()
    {
        selectedBlock = this;
        isFollowingMouse = true;
        SFXManager.instance.PlayPickUp.Post(gameObject);

        if (blockUI != null)
        {
            blockUI.ResetToOriginalColours();
        }


        if (!isSnappedToGrid)
        {
            ResetHoverAnimation();
            //blockOriginPos = blockParentRect.position;
            //originalRotation = blockParentRect.rotation;
            return;
        }
        //audioObject = AudioManager.instance.Play(blockData.MuteInstrument);
        MusicManager.instance.PlayInstruments(blockData.MuteInstrument);
        RemoveFromGrid();
        ResetHoverAnimation();
        ConnectionManager.instance.ResetCompletedPaths();
    }


    private void RemoveFromGrid()
    {
        isSnappedToGrid = false;
        //audioObject = AudioManager.instance.Play(blockData.MuteInstrument);
        MusicManager.instance.PlayInstruments(blockData.MuteInstrument);
        SFXManager.instance.PlayDrop.Post(gameObject);
        UnMarkBlockCellAsOccupied();
        RemovePlaceBlockFromList(blockParentRect);
        RemoveBlockUIFromList();
        ConnectionManager.instance.CheckConnectionsForAllEndCells();
        ConnectionManager.instance.ResetCompletedPaths();


        foreach (EndCell thisCell in endCells)
        {
            thisCell.StopBlink();

        }

        ClearEndCells();

        //AudioManager.instance.StopMusic();
    }

    private void ResetBlockToOrigin()
    {
        blockParentRect.position = blockOriginPos;
        //blockParentRect.rotation = Quaternion.identity;
        blockParentRect.rotation = originalRotation;
        Debug.Log($"Resetting block to original rotation: {originalRotation.eulerAngles}");

        ConnectionManager.instance.ClearBlockPulses();
        ConnectionManager.instance.ResetCompletedPaths();
        //audioObject = AudioManager.instance.Play(blockData.MuteInstrument);
        MusicManager.instance.PlayInstruments(blockData.MuteInstrument);
        SFXManager.instance.PlayDrop.Post(gameObject);

        if (blockUI != null)
        {
            blockUI.ResetToOriginalColours();
        }

    }

    private bool IsAnyBlockOutsideGrid()
    {
        foreach (RectTransform thisBlock in blockRectransforms)
        {
            if (IsBlockOutsideGrid(thisBlock))
                return true;
        }
        return false;
    }

    //------------------------- BlockPlacement-------------------------------------------
    private void FinalBlockPlacement(GridCell snapClosestGridCell)
    {

        if (snapClosestGridCell != null)// if we found grid cell that is close
        {
            //Debug.Log(snapClosestGridCell.x + "," + snapClosestGridCell.y);
            MoveBlockToGrid(snapClosestGridCell);

        }

        DidNotFindAnyGridCell();
        BlockIsDraggedOutOfBounds();

    }


    private void MoveBlockToGrid(GridCell snapClosestGridCell)
    {
        gridCell = snapClosestGridCell.GetComponent<GridCell>();

        bool allBlocksCanPlace = AllBlockCellsCanPlace();

        FoundValidGridCell(snapClosestGridCell, allBlocksCanPlace);
    }


    private void FoundValidGridCell(GridCell snapClosestGridCell, bool allBlocksCanPlace)
    {
        if (gridCell != null && allBlocksCanPlace)
        {

            blockParentRect.position = snapClosestGridCell.GetComponent<RectTransform>().position;

            bool blockOnClosedCell = IsBlockOverClosedCell();
            if (blockOnClosedCell)
            {
                UnMarkBlockCellAsOccupied();
                foreach (EndCell endCell in endCells)
                {
                    endCell.StopBlink();
                }
                ConnectionManager.instance.ClearBlockPulses();

                blockParentRect.position = blockOriginPos;
                blockParentRect.rotation = originalRotation;
                isSnappedToGrid = false;
                UnhighlightAllCells();

                return;
            }

            isSnappedToGrid = true;
            MarkBlockCellsAsOccupied();
            AddPlaceBlockToList(blockParentRect);
            AddBlockUIToList();
            CheckForFirstEndCell();

            // Establish connections for each end cell
            UpdateEndCellGridPositions();

            foreach (EndCell endCell in endCells)
            {
                endCell.CheckForEndCells();
            }


            //Debug.Log(">> Triggering path check after placing block");
            //Check for any new connections if they are valid or complete
            ConnectionManager.instance.CheckConnectionsForAllEndCells();

        }
        else
        {
            //Debug.Log("Block placement failed - grid cell occupied or invalid");
            blockParentRect.position = blockOriginPos;// no grid cell occupied so return to origin pos
        }

    }


    private void CheckForFirstEndCell()
    {
        //if (!ConnectionSystem.instance.endCells.Contains(endCells[0]))
        //{
        //    //Debug.Log($"Adding {endCells.Length} EndCells to connection system");
        //    ConnectionSystem.instance.endCells.AddRange(endCells);
        //}
        ConnectionManager.instance.RegisterEndCells(endCells);
    }
    private void UpdateEndCellGridPositions()
    {
        foreach (EndCell cell in endCells)
        {
            cell.UpdateGridPosition();

        }
    }

    private void DidNotFindAnyGridCell()
    {
        if (gridCell == null)
        {

            blockParentRect.position = blockOriginPos;// no grid cell found so return origin pos
        }
    }



    private bool IsBlockOverClosedCell()
    {
        foreach (RectTransform blockRect in blockRectransforms)
        {
            GridCell currentCell = SnapClosestGridCell(blockRect.position);

            if (currentCell == null || LevelManager.Instance.gridData == null)
                continue;

            Vector2Int blockPos = new Vector2Int(currentCell.x, currentCell.y);
            bool isClosed = LevelManager.Instance.gridData.closedGridSpace.Contains(blockPos);

            if (isClosed)
            {
                Debug.LogWarning($"Block is on a closed cell at {blockPos}");
                return true;
            }
        }

        return false;
    }
    private void BlockIsDraggedOutOfBounds()
    {
        if (IsBlockOutsideGrid(blockParentRect))
        {
            blockParentRect.position = blockOriginPos;
            /*   blockParentRect.rotation = Quaternion.identity;*/ // rotate UI of block
            blockParentRect.rotation = originalRotation;
            UnMarkBlockCellAsOccupied();
        }
    }

    private bool AllBlockCellsCanPlace()
    {
        foreach (RectTransform thisBlock in blockRectransforms)
        {
            //GridCell currentCell = gridVisual.FindGridCellAtPosition(thisBlock.position);
            GridCell currentCell = SnapClosestGridCell(thisBlock.position);
            if (currentCell != null && !currentCell.CanPlaceBlock()) // if current cell has a block and cant place a block
            {
                return false;// cant place block here
            }
        }

        return true; // all spots are empty, you can place block here.
    }

    private void MarkBlockCellsAsOccupied()
    {
        //Debug.Log("Blocks are occupied");
        foreach (RectTransform thisBlock in blockRectransforms)
        {
            //GridCell occupiedCell = gridVisual.FindGridCellAtPosition(thisBlock.position);
            GridCell occupiedCell = SnapClosestGridCell(thisBlock.position);

            if (occupiedCell != null)
            {
                //Debug.Log($"Marking cell {occupiedCell.x}, {occupiedCell.y} as occupied");
                occupiedCell.GridCellOccupied();
            }
        }

    }
    private void UnMarkBlockCellAsOccupied()
    {
        foreach (RectTransform thisBlock in blockRectransforms)
        {
            //GridCell occupiedCell = gridVisual.FindGridCellAtPosition(thisBlock.position);
            GridCell occupiedCell = SnapClosestGridCell(thisBlock.position);


            if (occupiedCell != null)
            {
                //Debug.Log($"Marking cell {occupiedCell.x}, {occupiedCell.y} as occupied");
                occupiedCell.GridCellFree();
            }
        }
    }



    public void RotateBlock()
    {
        //bool isBlockOverlapping = IsBlockOverlapping();
        bool allBlockCellCanPlace = AllBlockCellsCanPlace();
        transform.Rotate(Vector3.forward, -90); // rotate UI of block

        //Adjust coordinates for new rotation orientation
        if (blockData != null)
        {
            blockData.RotationCoordinates();
        }
        else
        {
            Debug.LogError("BlockData reference is missing!");
            return;
        }
        SFXManager.instance.PlayRotation.Post(gameObject);
    }


    public GridCell SnapClosestGridCell(Vector3 thisPosition)
    {
        float minDistance = Mathf.Infinity;
        GridCell closestCell = null;

        foreach (Transform gridCell in gridParent)
        {
            float distance = Vector2.Distance(thisPosition, gridCell.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestCell = gridCell.GetComponent<GridCell>();
            }
        }
        return closestCell;
    }

    private bool IsBlockOutsideGrid(RectTransform thisTransform)
    {
        Vector4 gridBoundaries = gridVisual.GetGridBoundaries();

        //Debug.Log($"Grid Boundaries: Left({gridBoundaries.x}) Right({gridBoundaries.y}) Bottom({gridBoundaries.z}) Top({gridBoundaries.w})");
        // Convert block position to local space
        Vector2 blockPosition = thisTransform.position;
        //Debug.Log($"Block Position: ({blockPosition.x}, {blockPosition.y})");

        return blockPosition.x < gridBoundaries.x ||
               blockPosition.x > gridBoundaries.y ||
               blockPosition.y < gridBoundaries.z ||
               blockPosition.y > gridBoundaries.w;
    }

    public void AddPlaceBlockToList(RectTransform thisBlock)
    {
        //ConnectionSystem.instance.placedBlocks.Add(thisBlock);  
        ConnectionManager.instance.AddPlacedBlocks(thisBlock);

    }

    public void AddBlockUIToList()
    {
        BlockUI newBlockUI = GetComponent<BlockUI>();
        //if (newBlockUI != null)
        //{

        //    ConnectionSystem.instance.allBlockUIs.Add(newBlockUI);
        //}
        ConnectionManager.instance.AddAllBlockUI(newBlockUI);
    }
    public void RemoveBlockUIFromList()
    {
        BlockUI thisBlockUI = GetComponent<BlockUI>();
        //if (thisBlockUI != null && ConnectionSystem.instance.allBlockUIs.Contains(thisBlockUI))
        //{
        //    ConnectionSystem.instance.allBlockUIs.Remove(thisBlockUI);
        //}
        ConnectionManager.instance.RemoveAllBlockUI(thisBlockUI);
    }
    public void RemovePlaceBlockFromList(RectTransform thisBlock)
    {
        //if (!ConnectionSystem.instance.placedBlocks.Contains(thisBlock))
        //{
        //    return;
        //}
        ConnectionManager.instance.RemovePlacedBlock(thisBlock);

        foreach (EndCell cell in endCells)
        {
            //ConnectionSystem.instance.endCells.Remove(cell);
            ConnectionManager.instance.Remove(cell);
        }
    }


    public List<RectTransform> GetListOfAllEndCells()
    {

        List<RectTransform> allEndCells = new List<RectTransform>();
        EndCell[] endCellsList = GetComponentsInChildren<EndCell>();
        foreach (EndCell cell in endCellsList)
        {
            allEndCells.Add(cell.GetComponent<RectTransform>());

        }

        return allEndCells;
    }



    private void UnhighlightAllCells()
    {
        for (int i = 0; i < gridVisual.cells.Length; i++)
        {
            gridVisual.cells[i].UnHighlight();
        }
    }
    private void HighlightCells()
    {
        List<GridCell> highlightedCells = new List<GridCell>();

        if (AllBlockCellsCanPlace())
        {
            foreach (RectTransform thisBlock in blockRectransforms)
            {
                GridCell currentCell = SnapClosestGridCell(thisBlock.position);

                if (currentCell != null)
                {
                    highlightedCells.Add(currentCell);
                }
            }
        }

        for (int i = 0; i < gridVisual.cells.Length; i++)
        {
            if (highlightedCells.Contains(gridVisual.cells[i]))
            {
                gridVisual.cells[i].Highlight();
            }
            else
            {
                gridVisual.cells[i].UnHighlight();
            }
        }
    }
}
