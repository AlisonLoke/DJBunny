using UnityEngine;
using UnityEngine.UI;

public class ScrollingBackGround : MonoBehaviour
{
    [SerializeField] private RawImage bgImage;
    [SerializeField] private float x,y;
  
    void Update()
    {
        bgImage.uvRect = new Rect(bgImage.uvRect.position + new Vector2(x, y) * Time.deltaTime,bgImage.uvRect.size);
    }
}
