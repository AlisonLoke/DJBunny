using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BlockSystem : MonoBehaviour, IPointerClickHandler
{
    private static BlockSystem selectedBlock = null;
    private Vector3 blockOriginPos;
    private GridCell gridCell;
    private GridVisual gridVisual;
    private float timeSinceLastRotation = Mathf.Infinity;
    private bool isSnappedToGrid = false;
    private bool isFollowingMouse = false;
    private GameObject audioObject;

    public RectTransform gridParent;
    [SerializeField] private RectTransform blockParentRect;
    [SerializeField] private RectTransform[] blockRectransforms;
    [SerializeField] private BlockData blockData;
    [SerializeField] private float delayBetweenRotations = 1f;
    //public List<RectTransform> placedBlocks;
    public EndCell[] endCells;


    private void Start()
    {
        gridVisual = gridParent.GetComponent<GridVisual>();
        endCells = GetComponentsInChildren<EndCell>();

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

        // Follow mouse if selected
        if (isFollowingMouse)
        {
            Vector2 pointerPosition = Mouse.current.position.ReadValue();
            blockParentRect.position = pointerPosition;
            HighlightCells();
        }
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
            if (audioObject != null)
            {
                Destroy(audioObject);
            }
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

        if (IsAnyBlockOutsideGrid())
        {
            ResetBlockToOrigin();
            UnhighlightAllCells();
            return;
        }

        GridCell snapClosestGridCell = SnapClosestGridCell(blockParentRect.position);
        BlockPlacement(snapClosestGridCell);
        Debug.Log("PLAYING MUSIC");

        //audioObject = AudioManager.instance.PlayMusic(blockData.AudioClip);
        //AudioManager.instance.drumSoundTest.Post(gameObject);
        audioObject = AudioManager.instance.PlayMusic(AudioManager.instance.drumSoundTest);
    }


    private void BeginPickUp()
    {
        selectedBlock = this;
        isFollowingMouse = true;

        if (!isSnappedToGrid)
        {
            blockOriginPos = blockParentRect.position;
            return;
        }

        RemoveFromGrid();
    }

    private void RemoveFromGrid()
    {
        isSnappedToGrid = false;
        UnMarkBlockCellAsOccupied();
        RemovePlaceBlockFromList(blockParentRect);
        ConnectionSystem.instance.CheckConnectionsForAllEndCells();

        ClearEndCells();
        if (audioObject != null)
        {
            Destroy(audioObject);
        }
    }

    private void ResetBlockToOrigin()
    {

        blockParentRect.position = blockOriginPos;
        blockParentRect.rotation = Quaternion.identity;



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



    private void BlockPlacement(GridCell snapClosestGridCell)
    {



        if (snapClosestGridCell != null)// if we found grid cell that is close
        {
            Debug.Log(snapClosestGridCell.x + "," + snapClosestGridCell.y);
            gridCell = snapClosestGridCell.GetComponent<GridCell>();

            bool allBlocksCanPlace = AllBlockCellsCanPlace();

            if (gridCell != null && allBlocksCanPlace)
            {
                blockParentRect.position = snapClosestGridCell.GetComponent<RectTransform>().position;

                MarkBlockCellsAsOccupied();
                isSnappedToGrid = true;
                AddPlaceBlockToList(blockParentRect);

                if (!ConnectionSystem.instance.endCells.Contains(endCells[0]))
                {
                    Debug.Log($"Adding {endCells.Length} EndCells to connection system");
                    ConnectionSystem.instance.endCells.AddRange(endCells);
                }



                // Establish connections for each end cell
                foreach (EndCell cell in endCells)
                {
                    cell.UpdateGridPosition();
                    cell.CheckForEndCells();
                }
                //Debug.Log(">> Triggering path check after placing block");
                ConnectionSystem.instance.CheckConnectionsForAllEndCells();
            }
            else
            {
                //Debug.Log("Block placement failed - grid cell occupied or invalid");
                blockParentRect.position = blockOriginPos;// no grid cell occupied so return to origin pos
            }

        }

        if (gridCell == null)
        {

            blockParentRect.position = blockOriginPos;// no grid cell found so return origin pos
        }

        if (IsBlockOutsideGrid(blockParentRect))
        {
            blockParentRect.position = blockOriginPos;
            blockParentRect.rotation = Quaternion.identity; // rotate UI of block
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
        ConnectionSystem.instance.placedBlocks.Add(thisBlock);

    }

    public void RemovePlaceBlockFromList(RectTransform thisBlock)
    {
        if (!ConnectionSystem.instance.placedBlocks.Contains(thisBlock))
        {
            return;
        }
        ConnectionSystem.instance.placedBlocks.Remove(thisBlock);

        foreach (EndCell cell in endCells)
        {
            ConnectionSystem.instance.endCells.Remove(cell);
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
