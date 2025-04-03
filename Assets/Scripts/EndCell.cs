using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndCell : MonoBehaviour
{
    public List<EndCell> connectedEndCell = null;
    [HideInInspector] public BlockSystem blockSystem;
    private RectTransform blockParent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Start()
    {
        blockSystem = GetComponentInParent<BlockSystem>();
    }
    public void CheckForEndCells()
    {
        GridCell currentCell = blockSystem.SnapClosestGridCell(transform.position);
        if (currentCell == null) { return; }


        //Check all four adjacent grid cells;
        GridCell down = FindAdjacentEndCell(currentCell.x + 1, currentCell.y);
        GridCell up = FindAdjacentEndCell(currentCell.x - 1, currentCell.y);
        GridCell right = FindAdjacentEndCell(currentCell.x, currentCell.y + 1);
        GridCell left = FindAdjacentEndCell(currentCell.x, currentCell.y - 1);


        if (down != null)
        {
            MakeCellGreen(down);
        }
        else if (up != null)
        {
            MakeCellGreen(up);
        }
        else if (right != null)
        {
            MakeCellGreen(right);
        }
        else if (left != null)
        {
            MakeCellGreen(left);
        }
        else
        {
            MakeCellRed();
        }
    }

    private GridCell FindAdjacentEndCell(int x, int y)
    {
        // get a list of all placed blocks
        // from each block, get the list of EndCells
        // add these EndCells to a list of all EndCells
        // iterate through all EndCells
        // if EndCell is THIS EndCell, ignore it
        // if EndCell is on same Block as THIS EndCell, ignore it
        // for each EndCell, check if coordinates are orthogonal to THIS EndCell
        List<RectTransform> placedBlocks = ConnectionSystem.instance.placedBlocks;

        List<EndCell> allEndCells = new List<EndCell>();

        foreach (RectTransform placedBlockRect in placedBlocks)
        {
            BlockSystem placedBlockSystem = placedBlockRect.GetComponent<BlockSystem>();
            if (placedBlockSystem != null && placedBlockSystem != blockSystem) // Skip the current block
            {
                EndCell[] endCellsInBlock = placedBlockSystem.endCells;
                allEndCells.AddRange(endCellsInBlock);
            }
        }



        foreach (EndCell endCell in allEndCells)
        {
            // Skip if this is the current EndCell or if it's already connected
            if (endCell == this /*|| endCell.connectedEndCell != null */)
                continue;

            // Get the grid cell for this end cell
            GridCell endCellGridCell = blockSystem.SnapClosestGridCell(endCell.transform.position);

            // Check if the coordinates match the adjacent position we're looking for
            if (endCellGridCell != null && endCellGridCell.x == x && endCellGridCell.y == y)
            {
                // Found a match - establish connection
                endCell.connectedEndCell.Add(this);
                return endCellGridCell;
            }
        }

        return null;
    }

    public void ClearConnections()
    {
        MakeCellRed();
    }


    public void MakeCellRed()
    {
    
        GetComponent<Image>().color = Color.red;

        connectedEndCell.Clear();
    }
    public void MakeCellGreen(GridCell cell)
    {
       
        GetComponent<Image>().color = Color.green;
       
      
    }
}
