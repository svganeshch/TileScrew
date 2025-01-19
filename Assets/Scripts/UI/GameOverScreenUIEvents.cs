using UnityEngine;
using UnityEngine.UIElements;

public class GameOverScreenUIEvents : MonoBehaviour
{
    private UIDocument document;

    Button replayButton;

    private void Awake()
    {
        document = GetComponent<UIDocument>();

        if (!document.enabled) document.enabled = true;

        replayButton = document.rootVisualElement.Q("ReplayButton") as Button;
        replayButton.RegisterCallback<ClickEvent>(OnReplayButton);

        document.rootVisualElement.style.visibility = Visibility.Hidden;
    }

    private void OnReplayButton(ClickEvent clickEvent)
    {
        StartCoroutine(GameManager.Instance.ReloadLevel());
        document.rootVisualElement.style.visibility = Visibility.Hidden;
    }
}
