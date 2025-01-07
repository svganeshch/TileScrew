using System;

public interface ISlot
{
    public Screw slotScrew { get; set; }

    public void SetSlotPosition(Screw screw, bool isShift = false, Action validateOnComplete = null);
}
