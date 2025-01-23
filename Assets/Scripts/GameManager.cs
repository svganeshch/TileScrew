using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public BackgroundManager backgroundManager;
    public LevelManager levelManager;
    public SaveManager saveManager;
    public SlotManager slotManager;
    public TileGridGenerator tileGridGenerator;

    public static GameState currentGameState;

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

        currentGameState = GameState.Active;
    }

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        //Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;

        levelManager.currentLevel = saveManager.saveData.currentLevel;
        UIManager.Instance.onLevelChangeEvent.Invoke(levelManager.currentLevel);

        levelManager.GenerateLevel(tileGridGenerator);
    }

    public IEnumerator ReloadLevel()
    {
        yield return StartCoroutine(levelManager.ClearLevel());

        levelManager.GenerateLevel(tileGridGenerator);
    }

    public IEnumerator LoadNextLevel()
    {
        yield return StartCoroutine(levelManager.ClearLevel());

        levelManager.GenerateNextLevel(tileGridGenerator);
    }
}
