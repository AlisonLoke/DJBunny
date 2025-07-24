using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager instance;

    [SerializeField] private ConnectionSystem[] connectionSystems;

    public event System.Action<int> onValidPathCompleted;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            foreach (ConnectionSystem connections in connectionSystems)
            {
                connections.onValidPathCompleted += count => onValidPathCompleted?.Invoke(count);
            }
        }
        else
        {
            Destroy(gameObject);
        }

    }
    public ConnectionSystem GetConnectionSystemByType(ConnectCellType type)
    {
        return connectionSystems.FirstOrDefault(connectionSystem => connectionSystem.ConnectionType == type);
    }

    public GridCell FindAdjacentEndCells(EndCell fromEndCell, Image blockCellImage, int x, int y)
    {
        foreach (ConnectionSystem connections in connectionSystems)
        {
            GridCell foundCell = connections.FindAdjacentEndCells(fromEndCell, blockCellImage, x, y);
            if (foundCell != null)
            {
                return foundCell;
            }
        }
        return null;
    }

    public void AddAllBlockUI(BlockUI blockUI)
    {
        foreach (ConnectionSystem connections in connectionSystems)
        {
            if (blockUI != null)
            {

                connections.allBlockUIs.Add(blockUI);
            }

        }
    }
    public void RemoveAllBlockUI(BlockUI blockUI)
    {
        foreach (ConnectionSystem connections in connectionSystems)
        {
            if (blockUI != null && connections.allBlockUIs.Contains(blockUI))
            {
                connections.allBlockUIs.Remove(blockUI);
            }
        }
    }
    public void PreviewCurrentPath()
    {
        foreach (ConnectionSystem connections in connectionSystems)
        {
            connections.PreviewCurrentPath();
        }
    }
    public void CheckConnectionsForAllEndCells()
    {
        foreach (ConnectionSystem connection in connectionSystems)
        {
            connection.CheckConnectionsForAllEndCells();
        }
    }
    public void ClearBlockPulses()
    {
        foreach (ConnectionSystem connection in connectionSystems)
        {
            connection.ClearBlockPulses();
        }
    }
    public void RegisterEndCells(EndCell[] endCells)
    {
        foreach (ConnectionSystem connections in connectionSystems)
        {
            if (!connections.endCells.Contains(endCells[0]))
            {
                //Debug.Log($"Adding {endCells.Length} EndCells to connection system");
                connections.endCells.AddRange(endCells);
            }

        }
    }
    public void Remove(EndCell endCell)
    {
        foreach (ConnectionSystem connections in connectionSystems)
        {
            connections.endCells.Remove(endCell);
        }
    }
    public void AddPlacedBlocks(RectTransform thisBlock)
    {
        foreach (ConnectionSystem connections in connectionSystems)
        {

            connections.placedBlocks.Add(thisBlock);
        }
    }
    public void RemovePlacedBlock(RectTransform thisBlock)
    {
        foreach (ConnectionSystem connections in connectionSystems)
        {
            if (!connections.placedBlocks.Contains(thisBlock)) { return; }

            connections.placedBlocks.Remove(thisBlock);
        }
    }

    public void ResetCompletedPaths()
    {
        foreach (ConnectionSystem connections in connectionSystems)
        {
            connections.ResetCompletedPath();
        }
    }
    //Call this event at the BlockSystem script
    //Also have a second line rendererer
    public void CheckPathsAreCompleted()
    {
        foreach (ConnectionSystem connections in connectionSystems)
        {
            if (!connections.pathIsComplete)
            {
                return;
            }
        }

        ShowPathsAreCompleted();
    }

    private void ShowPathsAreCompleted()
    {
        foreach (ConnectionSystem connections in connectionSystems)
        {
            connections.ShowPathComplete();
        }
    }
}
