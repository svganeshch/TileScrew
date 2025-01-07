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

            slots[insertIndex].SetSlotPosition(screw, validateOnComplete:ValidateSlots);
        }
        else
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].slotScrew == null)
                {
                    slots[i].SetSlotPosition(screw, validateOnComplete:ValidateSlots);
                    break;
                }
            }
        }

        return true;
    }

    private void ShiftSlots(int insertIndex)
    {
        while (slots[insertIndex].slotScrew != null)
        {
            if (slots[insertIndex + 1].slotScrew != null)
                ShiftSlots(insertIndex + 1);

            slots[insertIndex + 1].SetSlotPosition(slots[insertIndex].slotScrew, true);
            slots[insertIndex].slotScrew = null;
        }
    }

    private void RearrangeSlots()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].slotScrew == null)
            {
                for (int j = i + 1; j < slots.Count; j++)
                {
                    if (slots[j].slotScrew != null)
                    {
                        slots[i].SetSlotPosition(slots[j].slotScrew, true);
                        slots[j].slotScrew = null;

                        break;
                    }
                }
            }
        }

        //Debug.Log("Slots rearranged after removing matched groups");
    }

    private void ValidateSlots()
    {
        for (int i = 0; i < slots.Count - 2; i++)
        {
            if (slots[i].slotScrew != null &&
                slots[i + 1].slotScrew != null &&
                slots[i + 2].slotScrew != null &&
                slots[i].slotScrew.ScrewColor == slots[i + 1].slotScrew.ScrewColor &&
                slots[i].slotScrew.ScrewColor == slots[i + 2].slotScrew.ScrewColor)
            {
                //Debug.Log($"Found 3 screws of color {slots[i].slotScrew.ScrewColor} at slots {i}, {i + 1}, {i + 2}");

                HandleMatchingSlotGroups(i, i + 1, i + 2);
            }
        }
    }

    private void HandleMatchingSlotGroups(int index1, int index2, int index3)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (i == index1 || i == index2 || i == index3)
            {
                Destroy(slots[i].slotScrew.gameObject, 0.25f);
                slots[i].slotScrew = null;
            }
        }

        //Debug.Log($"Removed screws from slots {index1}, {index2}, {index3}");

        RearrangeSlots();
    }
}
