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
    [HideInInspector] public BlockSystem blockSystem;
    private RectTransform blockParent;
    private BlockUI blockUI;
    private Image cellImage;

    private BlockCellPulse myTweenAnimation;


    // Start is called once before the first execution of Update after the MonoBehaviour is created

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

    }
    public void CheckForEndCells()
    {
        connectedEndCell.Clear();

        GridCell currentCell = blockSystem.SnapClosestGridCell(transform.position);
        if (currentCell == null) { return; }


        //Check all four adjacent grid cells;
        GridCell down = ConnectionSystem.instance.FindAdjacentEndCells(this, cellImage, currentCell.x + 1, currentCell.y);
        GridCell up = ConnectionSystem.instance.FindAdjacentEndCells(this, cellImage, currentCell.x - 1, currentCell.y);
        GridCell right = ConnectionSystem.instance.FindAdjacentEndCells(this, cellImage, currentCell.x, currentCell.y + 1);
        GridCell left = ConnectionSystem.instance.FindAdjacentEndCells(this, cellImage, currentCell.x, currentCell.y - 1);

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
            StartBlink("#FF8A8A", 0.5f);
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

    public void MakeCellBlue(GridCell cell)
    {

        //GetComponent<Image>().color = Color.blue;

    }

    public void MakeCellYellow(GridCell cell)
    {

        //GetComponent<Image>().color = Color.yellow;
        onlyConnectToStartFinish = true;
        connectedEndCell.Clear();
        //StopPulseColour();
        StopBlink();

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

        // create the tween animation
        Tween loopPulseAnim = cellImage.DOColor(targetColour, duration).SetLoops(-1, LoopType.Yoyo);

        // store the tween animation
        BlockCellPulse newBlockCellPulse = new BlockCellPulse();
        newBlockCellPulse.pulseAnimation = loopPulseAnim;
        newBlockCellPulse.BlockCellToPulse = cellImage;
        myTweenAnimation = newBlockCellPulse;
    }

    public void StopBlink()
    {
        if (myTweenAnimation == null) { return; }
        myTweenAnimation.pulseAnimation.Kill();
        myTweenAnimation.BlockCellToPulse.color = Color.red;
        myTweenAnimation = null;
    }
}
