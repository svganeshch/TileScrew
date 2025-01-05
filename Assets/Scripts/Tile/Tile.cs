using UnityEngine;

public class Tile : MonoBehaviour, ITile, ITouch
{
    public Color disabledColor;

    private Screw screw;
    private SpriteRenderer spriteRenderer;
    private bool state = true;

    public bool State { get => state; set => state = value; }

    private void Awake()
    {
        screw = GetComponentInChildren<Screw>();
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

    public void OnTouch()
    {
        if (!state) return;
    }
}