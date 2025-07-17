using NUnit.Framework.Api;
using UnityEngine;
using UnityEngine.UI;

public class GridCell : MonoBehaviour
{

    public int x;
    public int y;
    public bool isOccupied;// public for dev purposes

    //private GridData gridData;

    [SerializeField] private Color Blue;
    [SerializeField] private Color Green;
    [SerializeField] private Color Red;
    [SerializeField] private Image image;
    [SerializeField] private GameObject connectCellImage;
    [SerializeField] private GameObject doubleConnectCellImage;
    [SerializeField] private GameObject closedCellImage;
   [SerializeField] private Color gridCellColour;
    public bool startCell = false;
    public bool finishCell = false;
    public bool IsCloseCell = false;
    public ConnectCellType connectCellType;



    private void Start()
    {
        gridCellColour = image.color;
    }


    public void SetCoordinates(int x, int y)
    {

        this.x = x; // "I'm at this spot (left-to-right)"
        this.y = y; // "I'm at this spot (top-to-bottom)"
        this.isOccupied = false; // "At first, I'm empty!"
    }

    public bool CanPlaceBlock()
    {
        return isOccupied == false;
    }

    public void GridCellOccupied()
    {

        isOccupied = true;
        image.color = Blue;
       
    }

    public void GridCellFree()
    {

        isOccupied = false;
        //image.color = Green;
        image.color = gridCellColour;
    }

    public void Highlight()
    {
        image.color = Red;
    }

    public void UnHighlight()
    {
        if (isOccupied) { return; }
        //image.color = Green;
        image.color = gridCellColour;
    }

    public void ConnectCellVisual(bool isStartCell, ConnectCellType newConnectCellType)
    {
        connectCellType = newConnectCellType;
        if (isStartCell)
        {
            startCell = true;
        }
        else
        {
            finishCell = true;
        }

        switch (connectCellType)
        {
            case ConnectCellType.Primary:
                connectCellImage.SetActive(true);
                break;
            case ConnectCellType.Secondary:
                doubleConnectCellImage.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void ClosedCellVisual()
    {

        IsCloseCell = true;
        closedCellImage.SetActive(true);
    }
}