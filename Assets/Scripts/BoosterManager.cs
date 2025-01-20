using System.Collections;
using UnityEngine;

public class BoosterManager : MonoBehaviour
{
    public static BoosterManager Instance;

    TileGridGenerator tileGridGenerator;

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
    }

    public void MagnetBooster()
    {
        var tiles = tileGridGenerator.tiles;

        var randomLayerTiles = tiles[Random.Range(0, tiles.Count - 1)].Value;

        var randomTile = randomLayerTiles[Random.Range(0, randomLayerTiles.Count - 1)];
        randomTile.SetTileState(true);
        randomTile.OnTouch();

        int count = 1;

        foreach ( var tileValuePair in tiles )
        {
            var layerTiles = tileValuePair.Value;

            foreach ( var layerTile in layerTiles )
            {
                if (count == 3)
                {
                    return;
                }

                if (layerTile == randomTile)
                {
                    continue;
                }

                if ( layerTile.screw.ScrewColor == randomTile.screw.ScrewColor)
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
