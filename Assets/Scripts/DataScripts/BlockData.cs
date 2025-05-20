using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockData", menuName = "Scriptable Objects/BlockData")]
public class BlockData : ScriptableObject
{
    [Header("Block Type")]
    [SerializeField] private string BlockName;
    [SerializeField] private GameObject BlockPrefab;
    [SerializeField] private List<Vector2Int> BlockCoordinatesList;

    [Header("Music")]
    //[SerializeField] private AudioClip audioClip;
    //public AudioClip AudioClip => audioClip;
    public AK.Wwise.Event Instruments = null; 
 

    [Header("Genre")]
    [SerializeField] private string genre;

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
