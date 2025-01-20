using DG.Tweening;
using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SlotManager : MonoBehaviour
{
    public List<Slot> slots;
    private List<int> matchingSlotIndexs = new List<int>();

    private Sequence shiftSequence;
    private Sequence rearrangeSequence;

    private Queue<Func<IEnumerator>> screwQueue = new Queue<Func<IEnumerator>>();
    private bool isProcessingQueue = false;

    private void Awake()
    {
        slots = GetComponentsInChildren<Slot>(includeInactive: true).ToList();

        GameObject tempScrewObject = new GameObject("TempScrew");
        var tempScrew = tempScrewObject.AddComponent<Screw>();

        slots[^1].slotScrew = tempScrew;
    }

    public void EnqueueScrew(Screw screw, Action OnCompleteCallback = null)
    {
        screwQueue.Enqueue(() => SetScrewSlot(screw, OnCompleteCallback));
        ProcessScrewQueue();
    }

    private void ProcessScrewQueue()
    {
        if (isProcessingQueue || screwQueue.Count == 0)
            return;

        isProcessingQueue = true;
        StartCoroutine(ProcessNextScrew());
    }

    private IEnumerator ProcessNextScrew()
    {
        if (screwQueue.Count > 0)
        {
            var nextScrewAction = screwQueue.Dequeue();
            yield return StartCoroutine(nextScrewAction());
        }

        isProcessingQueue = false;
        ProcessScrewQueue();
    }

    private IEnumerator SetScrewSlot(Screw screw, Action OnCompleteCallback)
    {
        int sameColorIndex = slots.FindLastIndex(s => s.slotScrew != null && s.slotScrew.ScrewColor == screw.ScrewColor);

        if (sameColorIndex != -1)
        {
            int insertIndex = sameColorIndex + 1;

            if (slots[insertIndex].slotScrew != null)
            {
                yield return StartCoroutine(ShiftSlots(insertIndex));
            }

            bool willCauseMatch = CheckForMatch(insertIndex, screw);

            Tween screwTween = slots[insertIndex].SetSlotPositionTween(screw);
            yield return screwTween.WaitForCompletion();

            if (willCauseMatch)
            {
                BoosterManager.Instance.previousTile = null;

                yield return new WaitUntil(() => HandleMatchingSlotGroupsCallback());
                yield return StartCoroutine(RearrangeSlots());
            }
        }
        else
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].slotScrew == null)
                {
                    Tween screwTween = slots[i].SetSlotPositionTween(screw);
                    yield return screwTween.WaitForCompletion();
                    break;
                }
            }
        }

        if (slots.All(s => s.slotScrew != null))
        {
            UIManager.Instance.gameOverEvent.Invoke();
            Debug.Log("All slots filled");
        }

        OnCompleteCallback?.Invoke();
    }

    private IEnumerator ShiftSlots(int insertIndex)
    {
        shiftSequence = DOTween.Sequence();

        List<KeyValuePair<Screw, int>> screwsToShift = new List<KeyValuePair<Screw, int>>();

        for (int i = insertIndex; i < slots.Count - 1; i++)
        {
            if (slots[i].slotScrew != null)
            {
                screwsToShift.Add(new KeyValuePair<Screw, int>(slots[i].slotScrew, i));
            }
        }

        slots[insertIndex].slotScrew = null;

        for (int i = screwsToShift.Count - 1; i >= 0; i--)
        {
            Tween shiftTween = slots[screwsToShift[i].Value + 1].SetSlotPositionTween(screwsToShift[i].Key, true);
            shiftSequence.Append(shiftTween);
        }

        yield return shiftSequence.WaitForCompletion();
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

    private IEnumerator RearrangeSlots()
    {
        rearrangeSequence = DOTween.Sequence();

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].slotScrew == null)
            {
                for (int j = i + 1; j < slots.Count; j++)
                {
                    if (slots[j].slotScrew != null)
                    {
                        Tween rearrangeTween = slots[i].SetSlotPositionTween(slots[j].slotScrew, true);
                        slots[j].slotScrew = null;

                        rearrangeSequence.Append(rearrangeTween);
                        break;
                    }
                }
            }
        }

        yield return rearrangeSequence.WaitForCompletion();
    }

    private bool HandleMatchingSlotGroupsCallback()
    {
        foreach (int matchingIndex in matchingSlotIndexs)
        {
            Destroy(slots[matchingIndex].slotScrew.gameObject);
            slots[matchingIndex].slotScrew = null;

            slots[matchingIndex].slotVFX.Play();
        }

        SFXManager.Instance.PlayScrewsMatchedSound();

        matchingSlotIndexs.Clear();

        return true;
    }

    public void EnableExtraSlot()
    {
        var extraSlot = slots[^1];

        extraSlot.gameObject.SetActive(true);
        extraSlot.slotScrew = null;

        transform.position = new Vector3(-0.5f, transform.position.y, transform.position.z);
    }

    public bool ClearAllSlots()
    {
        foreach (var slot in slots)
        {
            if (slot.slotScrew != null)
            {
                Destroy(slot.slotScrew.gameObject);
                slot.slotScrew = null;
            }
        }

        return true;
    }

    public void ResetSlot(Screw screw)
    {
        foreach (var slot in slots)
        {
            if (slot.slotScrew == screw)
            {
                slot.slotScrew = null;
                break;
            }
        }
    }
}
