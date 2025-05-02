using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndCell : MonoBehaviour
{
    public List<EndCell> connectedEndCell = new List<EndCell>();
    public EndCell sisterEndCell = null;
    public GridCell currentGridCell;
    public bool onlyConnectToStartFinish = false;
    [HideInInspector] public BlockSystem blockSystem;
    private RectTransform blockParent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void Initialise()
    {
        blockSystem = GetComponentInParent<BlockSystem>();
        currentGridCell = blockSystem.SnapClosestGridCell(transform.position);   

        foreach( EndCell endCell in blockSystem.endCells)
        {
            if( endCell != this )
            {
                sisterEndCell = endCell;    
            }
        }
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
            MakeCellBlue(down);
        }
        else if (up != null)
        {
            MakeCellBlue(up);
        }
        else if (right != null)
        {
            MakeCellBlue(right);
        }
        else if (left != null)
        {
            MakeCellBlue(left);
        }
        else
        {
            MakeCellRed();
        }
    }

    private GridCell FindAdjacentEndCell(int x, int y)
    {
       
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
            // If endcell is on start/finish = true, skip endcell connection
            if (endCell.onlyConnectToStartFinish)
            {
                continue;
            }
            //if this cell or its sister has a connection, dont't make more, skip
            //if(connectedEndCell.Count>0 || (sisterEndCell != null && sisterEndCell.connectedEndCell.Count > 0)) { continue; }

            ////if other end cell or it's sister is already connected, skip
            //if (endCell.connectedEndCell.Count > 0 || (endCell.sisterEndCell != null && endCell.sisterEndCell.connectedEndCell.Count > 0)) { continue; } 


            // Get the grid cell for this end cell
            GridCell endCellGridCell = blockSystem.SnapClosestGridCell(endCell.transform.position);

            // Check if the coordinates match the adjacent position we're looking for
            if (endCellGridCell != null && endCellGridCell.x == x && endCellGridCell.y == y)
            {
                


                // Found a match - establish connection
                if (!endCell.connectedEndCell.Contains(this))
                {
                    endCell.connectedEndCell.Add(this);
                }

                if(!connectedEndCell.Contains(endCell))
                {
                    this.connectedEndCell.Add(endCell);
                }

                return endCellGridCell;
            }
        }

        return null;
    }
    public void UpdateGridPosition()//debug function
    {
        if (blockSystem == null)
        {
            blockSystem = GetComponentInParent<BlockSystem>();
            if (blockSystem == null)
            {
                Debug.LogError($"EndCell {name} cannot find BlockSystem!");
                return;
            }
        }

        currentGridCell = blockSystem.SnapClosestGridCell(transform.position);

        if (currentGridCell != null)
        {
            Debug.Log($"EndCell {name} updated to grid position: ({currentGridCell.x}, {currentGridCell.y})");
        }
        else
        {
            Debug.LogWarning($"EndCell {name} could not find a grid cell!");
        }
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
    public void MakeCellBlue(GridCell cell)
    {

        GetComponent<Image>().color = Color.blue;
   
    }

    public void MakeCellYellow(GridCell cell)
    {

        GetComponent<Image>().color = Color.yellow;
        onlyConnectToStartFinish = true;
        connectedEndCell.Clear();

    }
}
