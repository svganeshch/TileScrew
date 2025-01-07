using DG.Tweening;
using UnityEngine;

public class Slot : MonoBehaviour, ISlot
{
    private Screw _slotScrew;
    public Screw slotScrew { get => _slotScrew; set => _slotScrew = value; }

    public void SetSlotPosition(Screw screw)
    {
        screw.transform.DOMove(transform.position, 0.5f).SetEase(Ease.InQuad);
        _slotScrew = screw;
    }

    public void ShiftSlotPosition(Screw screw)
    {
        screw.transform.DOMove(transform.position, 0.15f).SetEase(Ease.InQuad);
        _slotScrew = screw;
    }
}
