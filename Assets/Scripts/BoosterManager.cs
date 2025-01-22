using System.Collections;
using UnityEngine;

public class BoosterManager : MonoBehaviour
{
    public static BoosterManager Instance;

    TileGridGenerator tileGridGenerator;
    SlotManager slotManager;

    public Tile previousTile;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        tileGridGenerator = GameManager.Instance.tileGridGenerator;
        slotManager = GameManager.Instance.slotManager;
    }

    public void MagnetBooster()
    {
        if (slotManager.currentSlotManagerState == SlotManagerState.Matching) return;

        var tiles = tileGridGenerator.tiles;
        var slots = slotManager.slots;

        Color colorToPull = Color.black;
        int emptySlotCount = 0;
        int count = 0;
        int tilesToPull = 0;

        for (int i = 0; i < slots.Count - 1; i++)
        {
            var firstScrew = slots[0].slotScrew;
            var slotScrew = slots[i].slotScrew;
            var nextScrew = slots[i + 1].slotScrew;

            if (slotScrew != null && nextScrew != null)
            {
                if (slotScrew.ScrewColor == nextScrew.ScrewColor)
                {
                    colorToPull = slots[i].slotScrew.ScrewColor;
                    tilesToPull = 1;
                    break;
                }
            }

            if (firstScrew != null)
            {
                
                colorToPull = firstScrew.ScrewColor;
                tilesToPull = 2;
            }

            foreach (var slot in slots)
            {
                if (slot.slotScrew == null)
                {
                    emptySlotCount++;
                }
            }
            if (emptySlotCount < tilesToPull) return;
        }

        foreach ( var tileValuePair in tiles )
        {
            var layerTiles = tileValuePair.Value;

            foreach ( var layerTile in layerTiles )
            {
                if (count == tilesToPull)
                {
                    return;
                }

                if ( layerTile.screw.ScrewColor == colorToPull)
                {
                    layerTile.SetTileState(true);
                    layerTile.tileIceManager.isIceTile = false;
                    layerTile.OnTouch();

                    count++;
                }
            }
        }
    }

    public void ShuffleLevelBooster()
    {
        StartCoroutine(GameManager.Instance.ReloadLevel());
    }

    public void Undo()
    {
        if (previousTile != null)
        {
            previousTile.TileReset();

            previousTile = null;
        }
    }

    public void DrillExtraSlot()
    {
        GameManager.Instance.slotManager.EnableExtraSlot();
    }
}
