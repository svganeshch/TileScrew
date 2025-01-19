using UnityEngine;

public class TileIceManager : MonoBehaviour
{
    public bool isIceTile;

    public SpriteRenderer[] tileIceSprites;

    private int iceBreakCount = 0;

    public void EnableIce()
    {
        foreach (var sprite in tileIceSprites)
        {
            sprite.enabled = true;
        }

        isIceTile = true;
    }

    public void BreakIce()
    {
        if (iceBreakCount + 1 >= tileIceSprites.Length)
        {
            isIceTile = false;
        }

        tileIceSprites[iceBreakCount].enabled = false;
        iceBreakCount++;

        SFXManager.Instance.PlayTileIceCrackSound();
    }
}
