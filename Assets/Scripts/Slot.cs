using DG.Tweening;
using System;
using UnityEngine;

public class Slot : MonoBehaviour, ISlot
{
    public float moveSpeed = 0.5f;
    public float shiftSpeed = 0.15f;

    private float transistionSpeed = 0;
    private Screw _slotScrew;
    public Screw slotScrew { get => _slotScrew; set => _slotScrew = value; }

    public void SetSlotPosition(Screw screw, bool isShift = false, Action OnCompleteCallback = null)
    {
        if (isShift)
        {
            transistionSpeed = shiftSpeed;
        }
        else
        {
            transistionSpeed = moveSpeed;
        }

        screw.transform.DOMove(transform.position, transistionSpeed)
            .SetEase(Ease.InQuad)
            .OnComplete(() => OnCompleteCallback?.Invoke());

        _slotScrew = screw;
    }
}
