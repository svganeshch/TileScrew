using UnityEngine;

public interface ITile
{
    public GameObject gameObject { get; }
    public int tileLayer { get; set; }
    public bool State { get; set; }
    public void SetTileState(bool state);
}
