using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollingBackGround : MonoBehaviour
{
    [SerializeField] private List<RawImage> bgImage;
    [SerializeField] private float x, y;

   private void Update()
    {
        foreach (RawImage image in bgImage)
        {

            image.uvRect = new Rect(image.uvRect.position + new Vector2(x, y) * Time.deltaTime, image.uvRect.size);
        }
    }
}
