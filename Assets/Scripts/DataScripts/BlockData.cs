using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockData", menuName = "Scriptable Objects/BlockData")]
public class BlockData : ScriptableObject
{
    //public enum BlockGenre
    //{
    //    Drums,
    //    Bass,
    //    Piano
    //}

    [Header("Block Type")]
    [SerializeField] private string BlockName;
    [SerializeField] private GameObject BlockPrefab;
    [SerializeField] private List<Vector2Int> BlockCoordinatesList;

    [Header("Music")]
    public AK.Wwise.Event PlayInstrument = null;
    public AK.Wwise.Event MuteInstrument = null;


    [Header("Genre")]
    [Tooltip("Select the genre of music this block represents")]
    [SerializeField] private BlockGenre genre;
    public BlockGenre GetGenre => genre;

    public void RotationCoordinates()
    {
        List<Vector2Int> rotatedCoordinates = new List<Vector2Int>();

        foreach (Vector2Int coordinates in BlockCoordinatesList)
        {
            // rotate the block 90 degrees
            Vector2Int rotated = new Vector2Int(-coordinates.x, coordinates.y);
            rotatedCoordinates.Add(rotated);
        }

        BlockCoordinatesList = rotatedCoordinates;
    }

  
}
