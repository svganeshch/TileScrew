using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public LevelManager levelManager;
    public SlotManager slotManager;
    public TileGridGenerator tileGridGenerator;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        levelManager.GenerateLevel(tileGridGenerator);
    }

    public void NextLevel()
    {
        tileGridGenerator.ClearGrid();
        levelManager.GenerateNextLevel(tileGridGenerator);
    }
}
