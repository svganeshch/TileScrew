public interface ITile
{
    public bool State { get; set; }
    public void SetLayer(int layer);
    public void SetTileState(bool state);
}
