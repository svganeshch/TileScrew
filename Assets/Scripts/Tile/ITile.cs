using UnityEngine;

public interface ITile
{
    public GameObject gameObject { get; }
    public bool State { get; set; }
    public void SetLayer(int layer);
    public void SetTileState(bool state);
}
