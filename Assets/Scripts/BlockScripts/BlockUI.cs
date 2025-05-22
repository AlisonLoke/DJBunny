using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BlockUI : MonoBehaviour
{
    private List<BlockCellPulse> blockCellToPulseList = new();

   

    public void BlockColourPulse(Image pulsingBlockCell, string hexColour, float duration = 0.5f)
    {
      

        //We check if a block cell to pulse already exists for this image
        foreach(BlockCellPulse thisBlockCell in blockCellToPulseList)
        {
            //if block cell is already pulsing
            if(thisBlockCell.BlockCellToPulse == pulsingBlockCell)
            {
                //If does, return so we don't start the pulse
                return;
            }
        }

        //turn hex value into colour
        if (!ColorUtility.TryParseHtmlString(hexColour, out Color targetColour))
        {
            Debug.LogWarning("Invalid hex color string: " + hexColour);
            return;
        }

    

        
        Tween loopPulseAnim = pulsingBlockCell.DOColor(targetColour, duration).SetLoops(-1, LoopType.Yoyo);

        BlockCellPulse newBlockCellPulse = new BlockCellPulse();
    
        newBlockCellPulse.pulseAnimation = loopPulseAnim;
    
        newBlockCellPulse.BlockCellToPulse = pulsingBlockCell;
    
        blockCellToPulseList.Add(newBlockCellPulse);
    }

    public void StopColourPulse(Image pulsingBlock)
    {
        //Check our list of tween image pairs
        foreach (BlockCellPulse thisBlockCell in blockCellToPulseList)
        {
            //Find the pair which contains this image, so we can therefore access its linked tween
            if (thisBlockCell.BlockCellToPulse == pulsingBlock)
            {
                //Kill the tween
                if (thisBlockCell.pulseAnimation.IsActive())
                {
                    thisBlockCell.pulseAnimation.Kill();
                }

                if (ColorUtility.TryParseHtmlString("#FF0000", out Color redColor))
                {
                    thisBlockCell.BlockCellToPulse.color = redColor;
                }

                //Remove the tween image pair from the list
                blockCellToPulseList.Remove(thisBlockCell);
                break;
            }
        }

    }

}
      



