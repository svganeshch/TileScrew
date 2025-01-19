using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public UIDocument gameOverScreen;

    public UnityEvent gameOverEvent;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        gameOverEvent = new UnityEvent();

        gameOverEvent.AddListener(OnGameOverEvent);
    }

    private void OnGameOverEvent()
    {
        GameManager.currentGameState = GameState.Paused;
        gameOverScreen.rootVisualElement.style.visibility = Visibility.Visible;
    }
}
