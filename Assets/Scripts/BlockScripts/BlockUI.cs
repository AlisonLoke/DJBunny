using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BlockUI : MonoBehaviour
{
    private List<BlockCellPulse> blockCellToPulseList = new();
    [SerializeField] private List<Image> blockImages = new();
    private List<Color> originalColours = new();

    private void Awake()
    {
        SaveOriginalColours();
    }

    private void SaveOriginalColours()
    {
        originalColours.Clear();
        foreach(Image image in blockImages)
        {
            originalColours.Add(image.color);
        }
    }

    public List<Image> GetBlockCellImages()
    {
        return blockImages;    
    }

    public List<RectTransform> GetBlockCellImagesRectTransforms()
    {
        List<RectTransform> rectTransforms = new();
        foreach (Image image in blockImages)
        {
            rectTransforms.Add(image.rectTransform);
        }
        return rectTransforms;
    }

  public void ResetToOriginalColours()
    {
        for (int i = 0; i < blockImages.Count; i++)
        {
            if (blockImages[i] != null)
            {
                blockImages[i].color = originalColours[i];
            }
        }
    }

}
      



