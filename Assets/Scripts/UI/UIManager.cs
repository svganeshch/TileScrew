using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public UIDocument gameOverScreen;
    public UIDocument levelDoneScreen;

    public UnityEvent<int> onLevelChangeEvent;
    public UnityEvent onLevelDoneEvent;
    public UnityEvent gameOverEvent;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        onLevelChangeEvent = new UnityEvent<int>();
        onLevelDoneEvent = new UnityEvent();
        gameOverEvent = new UnityEvent();

        onLevelDoneEvent.AddListener(ShowLevelDoneScreen);
        gameOverEvent.AddListener(OnGameOverEvent);
    }

    private void ShowLevelDoneScreen()
    {
        GameManager.currentGameState = GameState.Paused;
        levelDoneScreen.rootVisualElement.style.visibility = Visibility.Visible;
    }

    private void OnGameOverEvent()
    {
        GameManager.currentGameState = GameState.Paused;
        gameOverScreen.rootVisualElement.style.visibility = Visibility.Visible;
    }
}
