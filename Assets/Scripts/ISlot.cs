public interface ISlot
{
    public Screw slotScrew { get; set; }

    public void SetSlotPosition(Screw screw);
    public void ShiftSlotPosition(Screw screw);
}
