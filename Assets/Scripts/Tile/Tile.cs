using DG.Tweening;
using System;
using UnityEngine;

public class Tile : MonoBehaviour, ITile, ITouch
{
    public Color disabledColor;
    public Screw screw;
    public TileIceManager tileIceManager;

    private Collider tileCollider;
    private SpriteRenderer spriteRenderer;

    private Vector3 origTilePosition;
    private Vector3 origTileScale;

    private int m_tileLayer = 0;
    private bool state = true;

    Sequence tileDropSequence;

    public int tileLayer { get => m_tileLayer; set => m_tileLayer = value; }
    public bool State { get => state; set => state = value; }

    private void Awake()
    {
        tileCollider = GetComponent<Collider>();
        screw = GetComponentInChildren<Screw>();
        tileIceManager = GetComponentInChildren<TileIceManager>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        origTilePosition = transform.position;
        origTileScale = transform.localScale;
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
        if (!state
            || tileIceManager.isIceTile
            || GameManager.Instance.tileGridGenerator.currentTileGridState == TileGridState.Generating)
        {
            transform.DOShakePosition(0.4f);

            SFXManager.Instance.PlayTileBlockedSound();
            return;
        }

        BoosterManager.Instance.previousTile = this;

        screw.transform.parent = null;
        GameManager.Instance.slotManager.EnqueueScrew(screw);
        SFXManager.Instance.PlayTilePickSound();

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

    public void TileReset()
    {
        if (tileDropSequence.active)
        {
            tileDropSequence.Kill();
        }

        transform.SetPositionAndRotation(origTilePosition, Quaternion.identity);
        transform.localScale = origTileScale;
        tileCollider.enabled = true;

        screw.transform.parent = transform;
        screw.transform.localPosition = Vector3.zero;

        GameManager.Instance.slotManager.ResetSlot(screw);
        GameManager.Instance.tileGridGenerator.OnTileUndo(this);
    }
}