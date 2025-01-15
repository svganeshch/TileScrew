using DG.Tweening;
using System;
using UnityEngine;

public class Tile : MonoBehaviour, ITile, ITouch
{
    public Color disabledColor;
    public Screw screw;

    private Collider tileCollider;
    private SpriteRenderer spriteRenderer;

    private int m_tileLayer = 0;
    private bool state = true;

    Sequence tileDropSequence;

    public int tileLayer { get => m_tileLayer; set => m_tileLayer = value; }
    public bool State { get => state; set => state = value; }

    private void Awake()
    {
        tileCollider = GetComponent<Collider>();
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
        if (!state || GameManager.Instance.tileGridGenerator.currentTileGridState == TileGridState.Generating)
        {
            transform.DOShakePosition(0.4f);
            return;
        }

        screw.transform.parent = null;
        GameManager.Instance.slotManager.EnqueueScrew(screw);

        DropTile();
    }

    private void DropTile()
    {
        tileCollider.enabled = false;
        tileDropSequence = DOTween.Sequence();

        tileDropSequence.Insert(0, transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.5f));

        tileDropSequence.Insert(0, transform.DOLocalRotate(new Vector3(0, 0, 360), 0.5f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(3));

        tileDropSequence.Insert(0.25f, transform.DOMove(transform.position + Vector3.up * 2f, 0.2f)
            .SetEase(Ease.InQuad));

        tileDropSequence.Insert(0.5f, transform.DOMove(transform.position + Vector3.down * 20f, 1f)
            .SetEase(Ease.InOutQuad));

        tileDropSequence.InsertCallback(0.1f, () => GameManager.Instance.tileGridGenerator.OnTileRemoved(this));
    }
}