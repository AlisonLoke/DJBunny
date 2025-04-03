using System.Collections.Generic;
using UnityEngine;

public class ConnectionSystem : MonoBehaviour
{
    public static ConnectionSystem instance;

    public List<RectTransform> placedBlocks;
    public List<EndCell> endCells;

    private void Awake()
    {
        instance = this;
    }

    public void CheckConnectionsForAllEndCells()
    {
        //List<EndCell> endCells = new List<EndCell>();

        //foreach(RectTransform block in placedBlocks)
        //{
        //    endCells.AddRange(block.GetComponent<BlockSystem>().endCells);
        //}

        foreach(EndCell endCell in endCells)
        {
            endCell.CheckForEndCells();
        }
    }
}
