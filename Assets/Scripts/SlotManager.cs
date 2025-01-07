using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SlotManager : MonoBehaviour
{
    private List<ISlot> slots;

    private void Awake()
    {
        slots = GetComponentsInChildren<ISlot>().ToList();
    }

    public bool SetScrewSlot(Screw screw)
    {
        if (slots.All(s => s.slotScrew != null))
        {
            Debug.Log("All slots filled");
            return false;
        }

        int sameColorIndex = slots.FindLastIndex(s => s.slotScrew != null && s.slotScrew.ScrewColor == screw.ScrewColor);

        if (sameColorIndex != -1)
        {
            int insertIndex = sameColorIndex + 1;

            ShiftSlots(insertIndex);

            slots[insertIndex].SetSlotPosition(screw);

            return true;
        }
        else
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].slotScrew == null)
                {
                    slots[i].SetSlotPosition(screw);
                    return true;
                }
            }
        }

        return false;
    }

    private void ShiftSlots(int insertIndex)
    {
        while (slots[insertIndex].slotScrew != null)
        {
            if (slots[insertIndex + 1].slotScrew != null)
                ShiftSlots(insertIndex + 1);

            slots[insertIndex + 1].ShiftSlotPosition(slots[insertIndex].slotScrew);
            slots[insertIndex].slotScrew = null;
        }
    }
}
