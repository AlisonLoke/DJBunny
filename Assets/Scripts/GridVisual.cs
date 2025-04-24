using System.Collections.Generic;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class GridVisual : MonoBehaviour
{

    [SerializeField] private GridData gridData;
    [SerializeField] private GameObject gridCellPrefab;
    [SerializeField] private RectTransform gridParent;
    public GridCell[] cells;
    public event System.Action OnGridGenerated;
    //public ConnectCellType currentConnectType = ConnectCellType.None;   
    //public enum ConnectCellType
    //{
    //    None,
    //    Start,
    //    Finish
    //}

    //public delegate void OnGridGeneratedHandler();
    //public event OnGridGeneratedHandler OnGridGenerated;



    private void Start()
    {
        GenerateGridVisual();

    }

    private void Update()
    {
        MouseClick();
    }

    private void GenerateGridVisual()
    {
        //access gridParent's grid layout component
        GridLayoutGroup gridLayout = gridParent.GetComponent<GridLayoutGroup>();

        if (gridLayout == null)
        {
            gridLayout = gridParent.gameObject.AddComponent<GridLayoutGroup>();
            //adjust grid box size just like in Unity
            gridLayout.cellSize = new Vector2(100, 100); // Adjust grid box size
            gridLayout.spacing = new Vector2(8, 8); // Adjust spacing
        }


        List <GridCell> tempCell = new List <GridCell>();

        for (int currentGridBox = 0; currentGridBox < gridData.x * gridData.y; currentGridBox++)
        {
            int xAxis = currentGridBox / gridData.x;  // Get the row index
            int yAxis = currentGridBox % gridData.x;  // Get the column index

            GameObject newGridCell = Instantiate(gridCellPrefab, gridParent);
            newGridCell.name = $"GridCell: ({xAxis}, {yAxis})";  // Assign name based on coordinates

            AssignCoordinatesToGridCell(xAxis, yAxis, newGridCell);

            GridCell gridCell = newGridCell.GetComponent<GridCell>();
            if(gridCell != null)
            {
                tempCell.Add(gridCell); 
            }
        }

        cells = tempCell.ToArray(); //list -> array

        OnGridGenerated?.Invoke();
    }


    public void AssignCoordinatesToGridCell(int xAxis, int yAxis, GameObject newGridCell)
    {
        GridCell gridCell = newGridCell.GetComponent<GridCell>();
        if (gridCell != null)// if the gridcell component exists on gridcell prefab
        {
            gridCell.SetCoordinates(xAxis, yAxis); // assign coordinate values on the grid.
            if(xAxis == gridData.startCellCoordinates.x && yAxis == gridData.startCellCoordinates.y)
            {
                gridCell.ConnectCellVisual(true, false);//specify start and end here
                //currentConnectType = ConnectCellType.Start;
     
            }

            if(xAxis == gridData.finishCellCoordinates.x && yAxis == gridData.finishCellCoordinates.y )
            {
                gridCell.ConnectCellVisual(false, true);
                 
                //currentConnectType = ConnectCellType.Finish;
            }
        }
    }


    public void MouseClick()
    {
        //Check if left mouse button is being pressed
        if (Mouse.current.leftButton.isPressed)
        {
            //Directly finds UI elements under the mouse cursor without needing a rect transform manually
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                // GET CURRENT MOUSE POS IN SCREEN COORDINATES (X,Y) AND STORE IT IN POINTER DATA
                position = Mouse.current.position.ReadValue()
            };

            //CREATES EMPTY LIST TO STORE OBJECTS HIT BY RAYCAST
            List<RaycastResult> objectHit = new List<RaycastResult>();

            //Shoots a UI raycast from the mouse position and Fills results with all UI elements under the cursor (topmost first).
            EventSystem.current.RaycastAll(pointerData, objectHit);



            //CHECK IF ANY UI ELEMENT WAS CLICKED
            if (objectHit.Count > 0)
            {
                //OBJECTHIT[0] GETS TOPMOST UI OBJ UNDER CURSOR AND EXTRACTS ACTUAL GAMEOBJECT WITH .GAMEOBJECT
                GameObject clickedObject = objectHit[0].gameObject;

                Debug.Log($"Clicked on: {clickedObject.name}");
            }
        }



    }

    public Vector4 GetGridBoundaries()
    {
        // Get actual width and height based on scale
        //float width = gridParent.rect.width * gridParent.lossyScale.x;
        //float height = gridParent.rect.height * gridParent.lossyScale.y;

        GridLayoutGroup layoutGroup = gridParent.GetComponent<GridLayoutGroup>();

        // Apply the lossyScale to account for any scaling of the parent
        float cellWidth = layoutGroup.cellSize.x * gridParent.lossyScale.x;
        float cellHeight = layoutGroup.cellSize.y * gridParent.lossyScale.y;
        float spacingX = layoutGroup.spacing.x * gridParent.lossyScale.x;
        float spacingY = layoutGroup.spacing.y * gridParent.lossyScale.y;

        // Calculate total width and height with scaled values
        float width = (cellWidth * gridData.x) + (spacingX * (gridData.x - 1));
        float height = (cellHeight * gridData.y) + (spacingY * (gridData.y - 1));

        float left = gridParent.position.x - (width / 2);
        float right = gridParent.position.x + (width / 2);
        float bottom = gridParent.position.y - (height / 2);
        float top = gridParent.position.y + (height / 2);

        return new Vector4(left, right, bottom, top);
    }

}



