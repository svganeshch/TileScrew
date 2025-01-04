using UnityEngine;

public class Tile : MonoBehaviour, ITile
{
    public Color disabledColor;

    private SpriteRenderer spriteRenderer;
    private bool state = true;

    public bool State { get => state; set => state = value; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetLayer(int layer)
    {
        spriteRenderer.sortingOrder += layer;
    }

    public void SetTileState(bool tileState)
    {
        if (tileState)
        {
            spriteRenderer.color = Color.white;
        }
        else
        {
            spriteRenderer.color = disabledColor;
        }

        state = tileState;
    }
}