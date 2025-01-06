using System.Linq;
using UnityEngine;

public class SlotManager : MonoBehaviour
{
    public ISlot[] slots;

    private void Awake()
    {
        slots = GetComponentsInChildren<ISlot>();
    }

    public void SetScrewSlot(Screw screw)
    {
        foreach (var slot in slots)
        {
            if (slot.slotScrew == null)
            {
                slot.SetSlot(screw);
                break;
            }
        }
    }
}
