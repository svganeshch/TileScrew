using DG.Tweening;
using UnityEngine;

public class Slot : MonoBehaviour, ISlot
{
    private Screw _slotScrew;
    public Screw slotScrew { get => _slotScrew; }

    public void SetSlot(Screw screw)
    {
        screw.transform.DOMove(transform.position, 0.5f).SetEase(Ease.InQuad);
        _slotScrew = screw;
    }
}
