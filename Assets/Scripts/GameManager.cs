using System.Collections;
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
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        //Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;

        levelManager.GenerateLevel(tileGridGenerator);
    }

    public IEnumerator LoadNextLevel()
    {
        bool tileCleared = tileGridGenerator.ClearGrid();

        yield return new WaitUntil(() => tileCleared);

        levelManager.GenerateNextLevel(tileGridGenerator);
    }
}
