using UnityEngine;

public class Screw : MonoBehaviour
{
    public SpriteRenderer screwBaseRenderer;

    private Color screwColor;
    public Color ScrewColor { get => screwColor; }

    public void SetColor(Color color)
    {
        screwBaseRenderer.color = color;
        screwColor = color;
    }
}
