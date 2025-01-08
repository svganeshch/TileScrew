using DG.Tweening;
using System;
using UnityEngine;

public class Tile : MonoBehaviour, ITile, ITouch
{
    public Color disabledColor;
    public Screw screw;

    private SpriteRenderer spriteRenderer;

    private int m_tileLayer = 0;
    private bool state = true;

    public int tileLayer { get => m_tileLayer; set => m_tileLayer = value; }
    public bool State { get => state; set => state = value; }

    private void Awake()
    {
        screw = GetComponentInChildren<Screw>();

        spriteRenderer = GetComponent<SpriteRenderer>();
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

        GameManager.Instance.slotManager.SetScrewSlot(screw);
        //StartCoroutine(
        //        GameManager.Instance.slotManager.SetScrewSlot(screw));

        DropTile();   
    }

    private void DropTile()
    {
        transform.DOMove(Vector3.down * 10f, 0.5f)
            .SetEase(Ease.OutQuad)
            .OnComplete(
                () => GameManager.Instance.tileGridGenerator.OnTileRemoved(this)
             );
    }
}