using UnityEngine;

public class Screw : MonoBehaviour
{
    public SpriteRenderer screwBaseRenderer;

    public void SetColor(Color color)
    {
        screwBaseRenderer.color = color;
    }
}
