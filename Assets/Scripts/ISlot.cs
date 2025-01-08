using DG.Tweening;
using System;

public interface ISlot
{
    public Screw slotScrew { get; set; }

    public Tween SetSlotPositionTween(Screw screw, bool isShift = false, Action OnCompleteCallback = null);
}
