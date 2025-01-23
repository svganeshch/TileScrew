using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public SaveData saveData;

    private void Start()
    {
        UIManager.Instance.onLevelChangeEvent.AddListener(SaveCurrentData);
    }

    public void SaveCurrentData(int level)
    {
        saveData.currentLevel = level;
    }
}
