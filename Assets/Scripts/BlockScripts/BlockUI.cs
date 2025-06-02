using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BlockUI : MonoBehaviour
{
    private List<BlockCellPulse> blockCellToPulseList = new();
    [SerializeField] private List<Image> blockImages = new();
    public List<Image> GetBlockCellImages()
    {
        List<Image> validImages = new List<Image>();
        foreach (Image blockImage in blockImages)
        {
            Image img = blockImage.GetComponent<Image>();
            if (img != null)
            {
                validImages.Add(img);
            }
        }
        return validImages;
    }

  

}
      



