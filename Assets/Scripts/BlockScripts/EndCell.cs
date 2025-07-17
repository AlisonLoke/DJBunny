using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EndCell : MonoBehaviour
{
    public List<EndCell> connectedEndCell = new List<EndCell>();
    public EndCell sisterEndCell = null;
    public GridCell currentGridCell;
    public bool onlyConnectToStartFinish = false;
    public bool IsEndCellOnClosedCell = false;
    [HideInInspector] public BlockSystem blockSystem;
    private RectTransform blockParent;
    private BlockUI blockUI;
    private Image cellImage;
    private GridVisual gridVisual;
    private GridData gridData;


    private BlockCellPulse myTweenAnimation;
    private GridCell closedCell;
    private Color originalColour;

    public BlockUI GetBlockUi()
    {
        return blockUI;
    }

    public void Initialise()
    {
        blockSystem = GetComponentInParent<BlockSystem>();
        blockUI = GetComponentInParent<BlockUI>();
        currentGridCell = blockSystem.SnapClosestGridCell(transform.position);
        cellImage = GetComponent<Image>();


        foreach (EndCell endCell in blockSystem.endCells)
        {
            if (endCell != this)
            {
                sisterEndCell = endCell;
            }
        }

        originalColour = cellImage.color;
    }
    public void CheckForEndCells()
    {
        connectedEndCell.Clear();

        GridCell currentCell = blockSystem.SnapClosestGridCell(transform.position);
        if (currentCell == null) { return; }


        //Check all four adjacent grid cells;
        GridCell down = ConnectionManager.instance.FindAdjacentEndCells(this, cellImage, currentCell.x + 1, currentCell.y);
        GridCell up = ConnectionManager.instance.FindAdjacentEndCells(this, cellImage, currentCell.x - 1, currentCell.y);
        GridCell right = ConnectionManager.instance.FindAdjacentEndCells(this, cellImage, currentCell.x, currentCell.y + 1);
        GridCell left = ConnectionManager.instance.FindAdjacentEndCells(this, cellImage, currentCell.x, currentCell.y - 1);

        //for debug purpose
        if (down != null)
        {
            //MakeCellBlue(down);
        }
        else if (up != null)
        {
            //MakeCellBlue(up);
        }
        else if (right != null)
        {
            //MakeCellBlue(right);
        }
        else if (left != null)
        {
            //MakeCellBlue(left);
        }
        else
        {
            MakeCellRed();
        }

        if (connectedEndCell.Count == 0)
        {
            //PulseColour();
            StartBlink("#767676", 0.5f);
        }
        else
        {
            //StopPulseColour();
            StopBlink();

        }
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

        //GetComponent<Image>().color = Color.red;
        onlyConnectToStartFinish = false;
        connectedEndCell.Clear();
    }

    //public void MakeCellBlue(GridCell cell)
    //{

    //    //GetComponent<Image>().color = Color.blue;

    //}

    public void ConnectedToStartAndFinish(GridCell cell)
    {

        //GetComponent<Image>().color = Color.yellow;
        onlyConnectToStartFinish = true;
        connectedEndCell.Clear();
        //StopPulseColour();
        StopBlink();

    }
    public void EndCellOnClosedCell()
    {
        //find closest gridcell to end cell's current position

        currentGridCell = blockSystem.SnapClosestGridCell(transform.position);
        //
        if (currentGridCell == null || LevelManager.Instance.gridData == null)
        {
            return;
        }

        //way to ask, is the current end cell in the list of closed cells?
        Vector2Int currentEndCellPos = new Vector2Int(currentGridCell.x, currentGridCell.y);

        //Checks whether the list of closed cells contains the coordinates of the cell the EndCell is on.
        if (LevelManager.Instance.gridData.closedGridSpace.Contains(currentEndCellPos))
        {
            IsEndCellOnClosedCell = true;
            Debug.Log($"EndCell is on a closed cell at ({currentEndCellPos.x}, {currentEndCellPos.y})");

        }
        else
        {
            IsEndCellOnClosedCell = false;
        }
    }


    public void StartBlink(string hexColour, float duration = 0.5f)
    {
        StopBlink();

        //turn hex value into colour
        if (!ColorUtility.TryParseHtmlString(hexColour, out Color targetColour))
        {
            Debug.LogWarning("Invalid hex color string: " + hexColour);
            return;
        }
        //Color originalColour = cellImage.color;

        // create the tween animation
        Tween loopPulseAnim = cellImage.DOColor(targetColour, duration).SetLoops(-1, LoopType.Yoyo);

        // store the tween animation
        BlockCellPulse newBlockCellPulse = new BlockCellPulse();
        newBlockCellPulse.pulseAnimation = loopPulseAnim;
        newBlockCellPulse.BlockCellToPulse = cellImage;
        newBlockCellPulse.originalColour = originalColour;
        myTweenAnimation = newBlockCellPulse;
    }

    public void StopBlink()
    {
        if (myTweenAnimation == null) { return; }
        myTweenAnimation.pulseAnimation.Kill();
        myTweenAnimation.BlockCellToPulse.color = myTweenAnimation.originalColour;
        myTweenAnimation = null;
    }
}
