using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SlotManager : MonoBehaviour
{
    private List<ISlot> slots;
    private List<int> matchingSlotIndexs = new List<int>();

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

            bool willCauseMatch = CheckForMatch(insertIndex, screw);

            slots[insertIndex].SetSlotPosition(screw, OnCompleteCallback: willCauseMatch ? HandleMatchingSlotGroupsCallback : null);
        }
        else
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].slotScrew == null)
                {
                    slots[i].SetSlotPosition(screw);
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

    private bool CheckForMatch(int insertIndex, Screw screw)
    {
        var originalScrew = slots[insertIndex].slotScrew;
        slots[insertIndex].slotScrew = screw;

        for (int i = 0; i < slots.Count - 2; i++)
        {
            if (slots[i].slotScrew != null &&
                slots[i + 1].slotScrew != null &&
                slots[i + 2].slotScrew != null &&
                slots[i].slotScrew.ScrewColor == slots[i + 1].slotScrew.ScrewColor &&
                slots[i].slotScrew.ScrewColor == slots[i + 2].slotScrew.ScrewColor)
            {
                matchingSlotIndexs.Add(i);
                matchingSlotIndexs.Add(i + 1);
                matchingSlotIndexs.Add(i + 2);

                return true;
            }
        }

        slots[insertIndex].slotScrew = originalScrew;
        return false;     
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

    private void HandleMatchingSlotGroupsCallback()
    {
        foreach (int matchingIndex in matchingSlotIndexs)
        {
            Destroy(slots[matchingIndex].slotScrew.gameObject, 0.25f);
            slots[matchingIndex].slotScrew = null;
        }

        RearrangeSlots();
    }
}
